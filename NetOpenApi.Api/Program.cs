
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