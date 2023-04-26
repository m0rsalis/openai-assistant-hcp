
using Microsoft.AspNetCore.Mvc;
using NetOpenApi.Api.Models;

namespace NetOpenApi.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<OpenAiConnector>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapGet("/", async (HttpContext context) =>
            {
                // Read the content of the HTML file
                var htmlContent = await File.ReadAllTextAsync("chat.html");

                // Set the content type header to indicate it's HTML
                context.Response.Headers["Content-Type"] = "text/html";

                // Write the HTML content to the response body
                await context.Response.WriteAsync(htmlContent);
            });

            app.MapPost("/chat", async (HttpContext httpContext, [FromBody] ChatRequest request, OpenAiConnector openAiConnector) =>
            {
                return await openAiConnector.ProcessChatRequest(request);
            })
            .WithName("Chat")
            .WithOpenApi();

            app.Run();
        }
    }
}