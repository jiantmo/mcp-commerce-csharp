using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using McpCommerceServer.Services;
using System.Text.Json;

namespace McpCommerceServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = true;
            });

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // Add Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "MCP Commerce Server API",
                Version = "v1",
                Description = "Remote Model Context Protocol (MCP) server for D365 Commerce"
            });
        });

        // Add HTTP client
        builder.Services.AddHttpClient();

        // Add custom services
        builder.Services.AddSingleton<ProductService>();
        builder.Services.AddSingleton<JsonRpcHandler>();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MCP Commerce Server API v1");
                c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
            });
        }

        app.UseCors("AllowAll");
        app.UseRouting();

        // Map controllers
        app.MapControllers();

        // MCP Protocol endpoints
        app.MapPost("/mcp", async ([FromBody] object request, [FromServices] JsonRpcHandler handler) =>
        {
            try
            {
                var jsonRequest = JsonSerializer.Serialize(request);
                var response = await handler.HandleRequestAsync(jsonRequest);
                return Results.Content(response, "application/json");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error processing MCP request: {ex.Message}");
            }
        })
        .WithName("HandleMcpRequest")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Handle MCP Protocol Request",
            Description = "Process Model Context Protocol JSON-RPC requests"
        });

        // Health check endpoint
        app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
            .WithName("HealthCheck")
            .WithOpenApi();

        // Server info endpoint
        app.MapGet("/info", ([FromServices] IConfiguration config) =>
        {
            return Results.Ok(new
            {
                serverName = config["MCP:ServerName"] ?? "D365 Commerce MCP Server",
                version = config["MCP:Version"] ?? "1.0.0",
                protocolVersion = config["MCP:ProtocolVersion"] ?? "2024-11-05",
                transport = "http",
                capabilities = new
                {
                    tools = new { listChanged = false },
                    resources = new { subscribe = false, listChanged = false }
                }
            });
        })
        .WithName("ServerInfo")
        .WithOpenApi();

        app.Run();
    }
}