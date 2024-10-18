# AI-Assisted REST API for PostgreSQL
This REST API uses  Google's AI 'Gemini 1.5 Flash' to make queries to PostgreSQL databases.\
The AI interprets natural lenguage into SQL queries in two different methods each one with it's good and bad things.
## Prerquisites
To make this API work, you'll need a PostgreSQL Server and an Gemini API Key.
> [!NOTE]
> I'll use pgAdmin to build the PostgreSQL Server.

## Setup
If you don't have it, download [pgAdmin 4 on the offical website](https://www.pgadmin.org/download/).\
Now make the PostgreSQL Server and create a database with a few tables and insert values.\
*Follow this guide*\
Then it's time to get your Gemini API Key, go to [Google AI Studio](https://aistudio.google.com/app/apikey), make sure you're logged in your Google account and press the blue button that says 'Create API key' then follow the steps until your Google Cloud Project is made and you have your API key, <b>make sure to save it in a save place</b>.\
Google lets you use this API for free without adding any billing information, but obviously, with some limitations.\
In Google AI Studio you can monitor the AI's usage clicking in 'View usage data' on the 'Plan' column where your proyects are displayed. I recommend monitoring the 'Quote and system limits' tab and ordering for 'actual usage percentage' because is the one that more information gives you.\
You have all you need to make the API work.\
You just need to replace the default values of the 'database' string on `AIAPI/Controllers/AIController.cs` with your database information and put your API Key on the 'apiKey' string.\
## About the REST API
The API have one Controller with two endpoints:
- AIDatabaseMapping.
It maps the whole database on a JSON that Gemini analyces and returns a table with the requested data.\
Because of the nature of the method, larger databases may overload the system's power, to prevent that, there is a parameter to limit how many rows will Gemini learn of.\
The data returned by AIDatabaseMapping is all AI generated, so some data may be made up. To check it there is a parameter to make the AI create the equivalent SQL query to retrieve that data from the PostgreSQL Server for you to try it by yourself and see if it's made up or not.
- AIDatabaseSQL.
It maps the structure of the database on a JSON that Gemini analyces to create an SQL query that is ran by the PostgreSQL Server which returns the requested data.\
It doesn't map the database's values so the Token usage is lower and the data returned is more trustful as it comes from the PostgreSQL Server itself. But it doesnt prevent AI made up data at all, sometimes the SQL query will fail because of non-existing columns, case in which the result will return the query used to detect the error.
## Techonogies used
- Programming Lenguage: C#
- Framework: ASP.NET Core (Project made with .Net 8.0 Framework)
- NuGets:
  - Swashbuckle.AapNetCore (6.4.0)
  - Swashbuckle.AspNetCore.Annotations (6.6.2)
  - Npgsql (8.0.5)
  - RestSharp (112.1.0)
  - Newtonsoft.Json (13.0.3)
- Other:
  - PostgreSQL (16.3)
  - pgAdmin 4 (8.9)
  - Gemini API key (1.5 Flash)
- Recommended IDE: Visual Studio 2022
