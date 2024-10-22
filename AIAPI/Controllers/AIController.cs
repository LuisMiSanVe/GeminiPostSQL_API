using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Text.Json;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;

namespace AIAPI.Controllers
{
    [Tags("AI DATABASE ASSISTANT")]
    [Route("[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        // Gemini API Data
        public static string endpoint = "https://generativelanguage.googleapis.com"; // Resource
        public static string uri = "/v1beta/models/gemini-1.5-flash-latest:generateContent?key="; // Model URI
        public static string apikey = ""; // API Key

        // Database Data
        public static string database = "Host=localhost;Username=username;Password=password;Database=database";

        [HttpPost]
        [Route("AIDatabaseMapping")]
        [SwaggerOperation(Summary = "AI Database Mapping", Description = "The database is mapped in Dictionaries and an AI scans it then processes the request the user sends.\n" +
                                                                         "The larger the database, the longer it takes to load. If it's too big it might throw OutOfMemoryException. In these cases is recomendable to use the Value limits.\n" +
                                                                         "- Request: the request that the AI will try to do.\n" +
                                                                         "- ValueRowLimit: Limit of how many rows of values is going to scan the AI. If '0' is set it scans all rows (Not recommended)\n" +
                                                                         "- ShowsSql: At the end of the result, the AI will generate the SQL you would need to get the data of the request.")]
        public string AIDatabaseHelper(string Request, int ValueRowLimit, bool ShowsSQL) {
            // Connects to the database
            var connection = new NpgsqlConnection(database);

            string result = "";
            string sql = "\"";

            if (ShowsSQL)
                sql = ", check in the database with a query like this:\" equivalent PostgreSQL query. ";

            if (connection != null)
            {
                connection.Open();

                // OBTAIN DB
                // Tables
                var tablesDB = new NpgsqlCommand("SELECT CONCAT(table_schema, '.', table_name) AS full_table_name FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND table_name NOT LIKE 'pg_%' AND table_name NOT LIKE 'sql_%' ORDER BY full_table_name;", connection).ExecuteReader();
                // Table           Column(Type)       Values
                Dictionary<string, Dictionary<string, List<string>>> tables = new Dictionary<string, Dictionary<string, List<string>>>();

                while (tablesDB.Read())
                {
                    if (!tables.ContainsKey(tablesDB.GetString(0)))
                        //         Name                     Columns
                        tables.Add(tablesDB.GetString(0), null);
                }
                tablesDB.Close();
                // Columns
                foreach (string tableName in tables.Keys)
                {
                    var columnsDB = new NpgsqlCommand("SELECT c.column_name, c.data_type, CASE WHEN tc.constraint_type = 'PRIMARY KEY' THEN 'PK' WHEN tc.constraint_type = 'FOREIGN KEY' THEN 'FK' ELSE '' END AS key_type " +
                                                      "FROM information_schema.columns c " +
                                                      "LEFT JOIN information_schema.key_column_usage kcu ON c.table_schema = kcu.table_schema AND c.table_name = kcu.table_name AND c.column_name = kcu.column_name " +
                                                      "LEFT JOIN information_schema.table_constraints tc ON kcu.constraint_name = tc.constraint_name AND kcu.table_schema = tc.table_schema AND kcu.table_name = tc.table_name " +
                                                      "WHERE c.table_schema = '" + tableName.Substring(0, tableName.IndexOf('.')) + "' AND c.table_name = '" + tableName.Remove(0, tableName.IndexOf('.') + 1) + "'" +
                                                      "ORDER BY c.column_name;", connection).ExecuteReader();

                    Dictionary<string, List<string>> columns = new Dictionary<string, List<string>>();

                    while (columnsDB.Read())
                    {
                        string columnInfo = columnsDB.GetString(0) + "(" + columnsDB.GetString(1) + ")";
                        if (!columnsDB.GetString(2).Equals(""))
                            columnInfo = columnsDB.GetString(0) + "(" + columnsDB.GetString(1) + ") (" + columnsDB.GetString(2) + ")";

                        if (!columns.ContainsKey(columnInfo))
                        {   //      Name(Type)(Key)  Values
                            columns.Add(columnInfo, null);

                            tables[tableName] = columns;
                        }
                    }
                    columnsDB.Close();
                    // Values
                    string limit = "";
                    if (ValueRowLimit > 0)
                        limit = " LIMIT " + ValueRowLimit;

                    foreach (string columnName in columns.Keys)
                    {
                        var valuesDB = new NpgsqlCommand("SELECT " + columnName.Substring(0, columnName.IndexOf('(')) + " FROM " + tableName + limit, connection).ExecuteReader();

                        List<string> values = new List<string>();

                        while (valuesDB.Read())
                        {
                            if (!values.Contains(valuesDB.GetValue(0).ToString()))
                            {   //          Value
                                values.Add(valuesDB.GetValue(0).ToString());

                                columns[columnName] = values;
                            }
                        }
                        valuesDB.Close();
                    }

                }
                var opcions = new JsonSerializerOptions
                {
                    WriteIndented = true // JSON format
                };

                string json = System.Text.Json.JsonSerializer.Serialize(tables, opcions);

                // Creates context to modify AI's behavior
                string context = "You're a database assistant, I'll send you requests and you'll return me the data in a table format, don't use more words, just return the table and at the end put: \"This response is not 100% accurate" +
                                 sql +
                                 "This is the database: " +
                                 json +
                                 "\nAnd this is my request: ";

                // I create the request
                var Client = new RestClient(endpoint);
                var request = new RestRequest(uri + apikey, Method.Post);
                request.AddHeader("Content-Type", "application/json");

                var body = new AIRequest();
                body.contents = new Content[] { new Content() { parts = new Part[] { new Part() { text = context + Request } } } };

                var jsonstring = JsonConvert.SerializeObject(body);

                request.AddJsonBody(jsonstring);
                // Sends the request to the service
                var response = Client.Post(request);
                var resp = JsonDocument.Parse(response.Content);
                // It extracts the AI's response from the 'Text' field
                result = resp.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
            }
            return result;
        }

        [HttpPost]
        [Route("AIDatabaseSQL")]
        [SwaggerOperation(Summary = "AI Database SQL Generator", Description = "The database structure is mapped in Dictionaries and an AI scans it then generates an SQL to fulfill the user's request which is executed on the PostgreSQL server.\n" +
                                                                               "The larger the database, the longer it takes to load.\n" +
                                                                               "- Request: the request that the AI will try to do.\n")]
        public string AIDatabaseHelperSQL(string Request)
        {
            // Connects to the database
            var connection = new NpgsqlConnection(database);

            string result = "";
            
            if (connection != null)
            {
                connection.Open();

                // OBTAIN DB
                // Tables
                var tablesDB = new NpgsqlCommand("SELECT CONCAT(table_schema, '.', table_name) AS full_table_name " +
                                                 "FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND table_name NOT LIKE 'pg_%' AND table_name NOT LIKE 'sql_%' " +
                                                 "ORDER BY full_table_name;", connection).ExecuteReader();
                // Table           Column(Type)
                Dictionary<string, List<string>> tables = new Dictionary<string, List<string>>();
                
                while (tablesDB.Read())
                {
                    if (!tables.ContainsKey(tablesDB.GetString(0)))
                        //         Name                   Columns
                        tables.Add(tablesDB.GetString(0), null);
                }
                tablesDB.Close();
                // Columns
                foreach (string tableName in tables.Keys)
                {
                    var columnsDB = new NpgsqlCommand("SELECT c.column_name, c.data_type, CASE WHEN tc.constraint_type = 'PRIMARY KEY' THEN 'PK' WHEN tc.constraint_type = 'FOREIGN KEY' THEN 'FK' ELSE '' END AS key_type " +
                                                      "FROM information_schema.columns c " +
                                                      "LEFT JOIN information_schema.key_column_usage kcu ON c.table_schema = kcu.table_schema AND c.table_name = kcu.table_name AND c.column_name = kcu.column_name " +
                                                      "LEFT JOIN information_schema.table_constraints tc ON kcu.constraint_name = tc.constraint_name AND kcu.table_schema = tc.table_schema AND kcu.table_name = tc.table_name " +
                                                      "WHERE c.table_schema = '" + tableName.Substring(0, tableName.IndexOf('.')) + "' AND c.table_name = '" + tableName.Remove(0, tableName.IndexOf('.') + 1) + "'" +
                                                      "ORDER BY c.column_name;", connection).ExecuteReader();
                
                    List<string> columns = new List<string>();
                
                    while (columnsDB.Read())
                    {
                        string columnInfo = columnsDB.GetString(0) + "(" + columnsDB.GetString(1) + ")";
                        if (!columnsDB.GetString(2).Equals(""))
                            columnInfo = columnsDB.GetString(0) + "(" + columnsDB.GetString(1) + ") (" + columnsDB.GetString(2) + ")";
                
                        if (!columns.Contains(columnInfo))
                        {   //      Name(Type)(Key)
                            columns.Add(columnInfo);
                
                            tables[tableName] = columns;
                        }
                    }
                    columnsDB.Close();
                }
                var opcions = new JsonSerializerOptions
                {
                    WriteIndented = true // JSON format
                };

                string json = System.Text.Json.JsonSerializer.Serialize(tables, opcions);

                // Creates context to modify AI's behavior
                string context = "You're a database assistant, I'll send you requests and you'll return a PostgeSQL query to do my request and if what I request can't be found on the database, tell me, but don't use more words. " +
                                 "This is the database: " +
                                 json +
                                 "\nAnd this is my request: ";

                // I create the request
                var Client = new RestClient(endpoint);
                var request = new RestRequest(uri + apikey, Method.Post);
                request.AddHeader("Content-Type", "application/json");

                var body = new AIRequest();
                body.contents = new Content[] { new Content() { parts = new Part[] { new Part() { text = context + Request } } } };

                var jsonstring = JsonConvert.SerializeObject(body);

                request.AddJsonBody(jsonstring);
                // Sends the request to the service
                var response = Client.Post(request);
                var resp = JsonDocument.Parse(response.Content);
                // It extracts the AI's response from the 'Text' field                                                                                             and I remove the SQL Code style the AI adds
                string generatedSql = resp.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString().Replace("```sql", "").Replace("```", "");
                try
                {
                    var resultBBDD = new NpgsqlCommand(generatedSql, connection).ExecuteReader();
                    var resultList = new List<Dictionary<string, object>>();

                    while (resultBBDD.Read())
                    {
                        Dictionary<string, object> resultDic = new Dictionary<string, object>();

                        for (int i = 0; i < resultBBDD.FieldCount; i++)
                        {
                            resultDic[resultBBDD.GetName(i)] = resultBBDD.GetValue(i);
                        }

                        resultList.Add(resultDic);
                    }
                    resultBBDD.Close();

                    result = System.Text.Json.JsonSerializer.Serialize(resultList, opcions) + "\nGenerated query: " + generatedSql;
                }
                catch (Exception e) {
                    result = "An error was thrown while running the generated query (" + generatedSql + ")\n" + e.StackTrace;
                }
            }
            return result;
        }
    }
}
