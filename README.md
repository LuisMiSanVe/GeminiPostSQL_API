> [See in spanish/Ver en español](https://github.com/LuisMiSanVe/AIPostgreAssistant/tree/spanish)
# 🤖 AI-Assisted REST API for PostgreSQL
[![image](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://dotnet.microsoft.com/en-us/languages/csharp)
[![image](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)](https://dotnet.microsoft.com/en-us/learn/dotnet/what-is-dotnet)
[![image](https://img.shields.io/badge/postgres-%23316192.svg?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![image](https://img.shields.io/badge/json-5E5C5C?style=for-the-badge&logo=json&logoColor=white)](https://www.newtonsoft.com/json)
[![image](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=Swagger&logoColor=white)](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
[![image](https://img.shields.io/badge/Google%20Gemini-8E75B2?style=for-the-badge&logo=googlegemini&logoColor=white)](https://aistudio.google.com/app/apikey)
[![image](https://img.shields.io/badge/Visual_Studio-5C2D91?style=for-the-badge&logo=visual%20studio&logoColor=white)](https://visualstudio.microsoft.com/)

>[!NOTE]
> This is the REST API version meant for servers or to use Swagger as client interface. There is a [WinForms](https://github.com/LuisMiSanVe/AIPostgreAssistant/tree/winforms) version meant for client use only.

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
Simply replace the default values in the 'database' string in [AIAPI/Controllers/AIController.cs](https://github.com/LuisMiSanVe/AI_DB_REST_API/blob/main/AIAPI/Controllers/AIController.cs) with your database information, and place your API Key in the 'apiKey' string.

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
- Programming Language: [C#](https://dotnet.microsoft.com/en-us/languages/csharp)
- Framework: [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet) (Project built with [.Net](https://dotnet.microsoft.com/en-us/learn/dotnet/what-is-dotnet) 8.0 Framework)
- NuGet Packages:
  - [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) (6.4.0)
  - [Swashbuckle.AspNetCore.Annotations](https://github.com/domaindrivendev/Swashbuckle.AspNetCore?tab=readme-ov-file#swashbuckleaspnetcoreannotations) (6.6.2)
  - [Npgsql](https://www.npgsql.org/) (8.0.5)
  - [RestSharp](https://restsharp.dev/) (112.1.0)
  - [Newtonsoft.Json](https://www.newtonsoft.com/json) (13.0.3)
- Other Tools:
  - [PostgreSQL](https://www.postgresql.org/) (16.3)
  - [pgAdmin 4](https://www.pgadmin.org/) (8.9)
  - Gemini API Key (1.5 Flash)
- Recommended IDE: [Visual Studio](https://visualstudio.microsoft.com/) 2022
