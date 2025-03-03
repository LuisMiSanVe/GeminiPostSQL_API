using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo{Title = "GeminiPostSQL REST API", Version = "v1", Description = "REST API with AI-Assisted Endpoints" });

    c.EnableAnnotations();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {   // Aditional options
        options.DefaultModelsExpandDepth(-1); // Removes the model list at the end of the page
        // Add the documents to the dropdown
        options.DocumentTitle = "GeminiPostSQL API"; // Browser Tab Name
        options.EnableFilter(); // Creates an Text box to filter Controllers
        options.DisplayRequestDuration(); // Shows the runtime ms on every Endpoint
        // Deafault state of the Endpoints (Expanded or Hiden)
        //options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.Full); 
        //options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        // Header Custom content
        options.HeadContent = "<link rel=\"icon\" type=\"image/png\" href=\"/Img/Favicon.png\">" // Favicon (wwwroot/Img/...)
                            + "<span class=\"header\">&emsp;Generated with Swashbuckle's Swagger UI</span>"; // Custom Header Text
        options.InjectStylesheet("/Style/style.css"); // Custom CSS (wwwroot/Style/...)
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
