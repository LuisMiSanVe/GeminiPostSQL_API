> [Ver en inglés/See in english](https://github.com/LuisMiSanVe/AIPostgreAssistant_API/tree/main)
# 🤖 REST API para PostgreSQL Asistida por IA
[![image](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://dotnet.microsoft.com/en-us/languages/csharp)
[![image](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)](https://dotnet.microsoft.com/en-us/learn/dotnet/what-is-dotnet)
[![image](https://img.shields.io/badge/postgres-%23316192.svg?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![image](https://img.shields.io/badge/json-5E5C5C?style=for-the-badge&logo=json&logoColor=white)](https://www.newtonsoft.com/json)
[![image](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=Swagger&logoColor=white)](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
[![image](https://img.shields.io/badge/Google%20Gemini-8E75B2?style=for-the-badge&logo=googlegemini&logoColor=white)](https://aistudio.google.com/app/apikey)
[![image](https://img.shields.io/badge/Visual_Studio-5C2D91?style=for-the-badge&logo=visual%20studio&logoColor=white)](https://visualstudio.microsoft.com/)

>[!NOTE]
> Esta es la REST API pensada para servidores o usar como cliente con Swagger. Hay una versión de [WinForms](https://github.com/LuisMiSanVe/GeminiPostSQL/tree/spanish) pensada solo para ser usada como cliente.

Esta REST API usa la IA de Google 'Gemini 1.5 Flash' para generar consultas a bases de datos PostgreSQL.  
La IA convierte lenguaje natural a consultas SQL usando dos métodos diferentes, cada uno con sus ventajas y desventajas.

## 📋 Prerequisitos

Para que la API funcione, necesiatarás un servidor PostgreSQL y una clave de la API de Gemini.

> [!NOTE]  
> Yo usaré pgAdmin para montar el servidor PostgreSQL.

## 🛠️ Instalación

Si no lo tienes, descarga [pgAdmin 4 desde su página ofical](https://www.pgadmin.org/download/).  
Ahora, crea el servidor y monta la base de datos con algunas tablas y valores.

Después, obten tu clave de la API de Gemini yendo aqui: [Google AI Studio](https://aistudio.google.com/app/apikey). Asegurate de tener tu sesión de Google abierta, y encontes dale al botón que dice 'Crear clave de API' y sigue los pasos para crear tu proyecto de Google Cloud y conseguir tu clave de API. **Guardala en algún sitio seguro**.  
Google permite el uso gratuito de esta API sin añadir ninguna forma de pago, pero con algunas limitaciones.

En Google AI Studio, puedes monitorizar el uso de la IA haciendo clic en 'Ver datos de uso' en la columna de 'Plan' en la tabla con todos tus proyectos. Recomiendo monitorizarla desde la pestaña de 'Cuota y límites del sistema' y ordenando por 'Porcentaje de uso actual', ya que es donde más información obtienes.

Ya tienes todo para hacer funcionar la REST API.  
Simpemente reemplaza los valores por defecto del string 'database' en [AIAPI/Controllers/AIController.cs](https://github.com/LuisMiSanVe/AI_DB_REST_API/blob/main/AIAPI/Controllers/AIController.cs) con la información de tu base de datos, y poner la clave de API en el string 'apiKey'.

## 📖 Sobre la REST API

La API tiene un Controlador con dos endpoints:

- **AIDatabaseMapping**  
  Mapea la base de datos entera en un JSON la cual Gemini analiza para devolver la tabla que correspondría la solicitud enviada.  
  Debido a la naturaleza del método, bases de datos muy grandes puedes sobrepasar los recursos del sistema. Para prevenir esto, hay parametros para limitar que tantas filas aprende Gemini.  
  Toda la información recogida está generada directamente por la IA, por lo que es posible que haya información inventada. Para verificarlo, hay un parametro que hace que la IA genere la SQL necesaria para que devuelva esa tabla, la cual puedes correr en tu servidor PostgreSQL para comprobarla vericidad de los datos.

- **AIDatabaseSQL**  
  Este método mapea la estructura de la base de datos en un JSON que Gemini analiza para crear una consulta SQL, la cual es ejecutada por el servidor PostgreSQL directamente.  
  Ya que este método no mapea los valores de la base de datos el uso de tokens es menor, y los datos que devuelve son mas fiables pues es el mismo Servidor el que los devuelve. Sin embargo, no evita completamente los errores que cometa la IA. A veces, la consulta SQL fallará debido a que la IA se inventa columnas que no existen, en ese caso el endpoint devolverá la consulta generada para que identifiques el fallo.

## 💻 Tecnologías usadas

- Lenguaje de programación: [C#](https://dotnet.microsoft.com/es-es/languages/csharp)
- Framework: [ASP.NET Core](https://dotnet.microsoft.com/es-es/apps/aspnet) (creado con el Framework [.Net](https://dotnet.microsoft.com/es-es/learn/dotnet/what-is-dotnet) 8.0)
- Paquetes NuGet:
  - [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) (6.4.0)
  - [Swashbuckle.AspNetCore.Annotations](https://github.com/domaindrivendev/Swashbuckle.AspNetCore?tab=readme-ov-file#swashbuckleaspnetcoreannotations) (6.6.2)
  - [Npgsql](https://www.npgsql.org/) (8.0.5)
  - [RestSharp](https://restsharp.dev/) (112.1.0)
  - [Newtonsoft.Json](https://www.newtonsoft.com/json) (13.0.3)
- Otras herramientas:
  - [PostgreSQL](https://www.postgresql.org/) (16.3)
  - [pgAdmin 4](https://www.pgadmin.org/) (8.9)
  - Gemini API Key (1.5 Flash)
- IDE Recomendado: [Visual Studio](https://visualstudio.microsoft.com/) 2022
