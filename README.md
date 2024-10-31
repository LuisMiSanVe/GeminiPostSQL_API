> [See in spanish/Ver en español](https://github.com/LuisMiSanVe/AI_DB_REST_API/tree/spanish)
# 🤖 AI-Assisted REST API for PostgreSQL

This REST API uses Google's AI 'Gemini 1.5 Flash' to make queries to PostgreSQL databases.  
The AI interprets natural language into SQL queries using two different methods, each with its pros and cons.

## 📋 Prerequisites

To make this API work, you'll need a PostgreSQL Server and a Gemini API Key.

> [!NOTE]  
> I'll use pgAdmin to build the PostgreSQL Server.

## 🛠️ Setup

If you don't have it, download [pgAdmin 4 from the official website](https://www.pgadmin.org/download/).  
Now, create the PostgreSQL Server and set up a database with a few tables and insert values.

Next, obtain your Gemini API Key by visiting [Google AI Studio](https://aistudio.google.com/app/apikey). Ensure you're logged into your Google account, then press the blue button that says 'Create API key' and follow the steps to set up your Google Cloud Project and retrieve your API key. **Make sure to save it in a safe place**.  
Google allows free use of this API without adding billing information, but there are some limitations.

In Google AI Studio, you can monitor the AI's usage by clicking 'View usage data' in the 'Plan' column where your projects are displayed. I recommend monitoring the 'Quota and system limits' tab and sorting by 'actual usage percentage,' as it provides the most detailed information.

You now have everything needed to make the API work.  
Simply replace the default values in the 'database' string in `AIAPI/Controllers/AIController.cs` with your database information, and place your API Key in the 'apiKey' string.

## 📖 About the REST API

The API has one Controller with two endpoints:

- **AIDatabaseMapping**  
  This maps the entire database into a JSON that Gemini analyzes to return a table with the requested data.  
  Due to the nature of this method, larger databases may overwhelm the system's resources. To prevent this, there's a parameter to limit how many rows Gemini will learn from.  
  The data returned by AIDatabaseMapping is AI-generated, so some data may be fabricated. To verify this, there is a parameter that allows the AI to generate the equivalent SQL query, which you can run on the PostgreSQL Server to check the accuracy of the data.

- **AIDatabaseSQL**  
  This method maps the database structure into a JSON that Gemini analyzes to create an SQL query, which is then run by the PostgreSQL Server, returning the requested data.  
  Since this method does not map the database values, token usage is lower, and the data is more reliable because it comes directly from the PostgreSQL Server. However, it doesn't completely prevent AI-generated errors. Occasionally, the SQL query might fail due to non-existing columns, in which case the result will include the query used to detect the error.

## 💻 Technologies Used

- **Programming Language:** C#
- **Framework:** ASP.NET Core (Project built with .NET 8.0 Framework)
- **NuGet Packages:**
  - Swashbuckle.AspNetCore (6.4.0)
  - Swashbuckle.AspNetCore.Annotations (6.6.2)
  - Npgsql (8.0.5)
  - RestSharp (112.1.0)
  - Newtonsoft.Json (13.0.3)
- **Other Tools:**
  - PostgreSQL (16.3)
  - pgAdmin 4 (8.9)
  - Gemini API Key (1.5 Flash)
- **Recommended IDE:** Visual Studio 2022
