# D365 Commerce Remote MCP Server

A **Remote** Model Context Protocol (MCP) server implementation for Microsoft Dynamics 365 Commerce, built with C# and .NET 8. This server provides HTTP-based MCP protocol support, allowing AI assistants and other clients to interact with D365 Commerce data over HTTP/HTTPS instead of stdio.

## Overview

This remote MCP server provides an HTTP API interface to D365 Commerce data and functionality, allowing AI assistants and other clients to interact with commerce data through the standardized MCP protocol over HTTP transport.

## Features

### Transport
- **HTTP/HTTPS**: Remote access via RESTful API endpoints
- **JSON-RPC over HTTP**: Standard MCP protocol over HTTP transport
- **CORS Support**: Cross-origin requests for web-based clients
- **Swagger UI**: Interactive API documentation at the root URL

### Tools
- `products_search` - Advanced product search with D365 metadata criteria
- `products_get_by_id` - Retrieve product information by ID
- `products_get_by_ids` - Retrieve multiple products by IDs
- `products_get_recommended` - Get product recommendations
- `products_compare` - Compare products for analysis
- `products_search_by_category` - Search products within a category
- `products_search_by_text` - Text-based product search

### Resources
- `d365://products` - Access to product catalog
- `d365://customers` - Access to customer database
- `d365://orders` - Access to order history

### API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/` | GET | Swagger UI Documentation |
| `/api/mcp` | POST | Main MCP JSON-RPC endpoint |
| `/api/mcp/capabilities` | GET | Server capabilities and info |
| `/api/mcp/health` | GET | Health check |
| `/api/mcp/initialize` | POST | Initialize MCP session |
| `/api/mcp/tools/list` | POST | List available tools |
| `/api/mcp/tools/call` | POST | Execute a tool |
| `/api/mcp/resources/list` | POST | List available resources |

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- Access to D365 Commerce instance (for production use)

### Building and Running

1. Build the project:
```bash
dotnet build
```

2. Run the server:
```bash
dotnet run
```

3. The server will start and listen on:
   - HTTP: `http://localhost:8080`
   - HTTPS: `https://localhost:8081` (development)

4. Access the interactive API documentation at: `http://localhost:8080`

### Configuration

Update `appsettings.json` to configure:
- **HTTP/HTTPS URLs**: Server binding addresses
- **D365 Commerce**: Connection settings
- **Logging**: Log levels and providers
- **CORS**: Cross-origin request policies

## Remote MCP Client Integration

### HTTP-based MCP Clients

Configure your MCP client to use HTTP transport:

```json
{
  "mcpServers": {
    "d365-commerce-remote": {
      "transport": "http",
      "baseUrl": "http://localhost:8080/api/mcp",
      "headers": {
        "Content-Type": "application/json"
      }
    }
  }
}
```

### Direct HTTP API Usage

You can also interact directly with the HTTP API:

```bash
# Initialize session
curl -X POST http://localhost:8080/api/mcp/initialize 
  -H "Content-Type: application/json" 
  -d '{"id": "1", "params": {"protocolVersion": "2024-11-05"}}'

# List tools
curl -X POST http://localhost:8080/api/mcp/tools/list 
  -H "Content-Type: application/json" 
  -d '{"id": "2"}'

# Call a tool
curl -X POST http://localhost:8080/api/mcp/tools/call 
  -H "Content-Type: application/json" 
  -d '{
    "id": "3",
    "params": {
      "name": "products_search_by_text",
      "arguments": {
        "channelId": 1,
        "catalogId": 1,
        "searchText": "laptop"
      }
    }
  }'
```

### Web-based Integration

For web applications, use fetch or axios:

```javascript
// Initialize MCP session
const initResponse = await fetch('http://localhost:8080/api/mcp/initialize', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    id: '1',
    params: { protocolVersion: '2024-11-05' }
  })
});

// Search products
const searchResponse = await fetch('http://localhost:8080/api/mcp/tools/call', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    id: '2',
    params: {
      name: 'products_search_by_text',
      arguments: {
        channelId: 1,
        catalogId: 1,
        searchText: 'laptop'
      }
    }
  })
});
```

## Sample Data

The server includes sample data for development and testing:
- 4 sample products across Electronics, Sports, and Home categories
- 3 sample customers with different loyalty levels
- 2 sample orders with order items
- Store inventory data for 2 stores

## Architecture

- **Program.cs** - ASP.NET Core Web API setup with MCP endpoints
- **Controllers/McpController.cs** - HTTP endpoints for MCP protocol
- **Services/JsonRpcHandler.cs** - JSON-RPC message handling and routing
- **Services/ProductService.cs** - D365 Commerce Product operations
- **Models/** - Data models for MCP messages and D365 entities

## Development

The server follows modern ASP.NET Core and D365 Commerce conventions:
- **ASP.NET Core Web API** - HTTP server framework
- **Swagger/OpenAPI** - Interactive API documentation
- **CORS support** - Cross-origin resource sharing
- **Dependency injection** - Service management
- **Configuration providers** - Environment-based configuration
- **Structured logging** - Comprehensive logging with Serilog patterns

## Production Deployment

For production use:

1. **Configuration**: Update `appsettings.Production.json` with:
   - Production D365 Commerce Scale Unit URLs
   - HTTPS-only binding
   - Appropriate CORS policies
   - Production logging levels

2. **Security**: 
   - Configure authentication/authorization
   - Set up API keys or JWT tokens
   - Implement rate limiting
   - Use HTTPS with proper certificates

3. **Hosting**:
   - Deploy to Azure App Service, AWS, or IIS
   - Configure load balancing for high availability
   - Set up monitoring and health checks

4. **Docker Deployment**:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY publish/ .
EXPOSE 8080
ENTRYPOINT ["dotnet", "McpCommerceServer.dll"]
```

Example Docker run:
```bash
docker run -p 8080:8080 -e ASPNETCORE_URLS=http://+:8080 mcp-commerce-server
```

## Migration from stdio MCP

If migrating from stdio-based MCP:

1. **Client Changes**: Update client configuration from stdio to HTTP transport
2. **Endpoint Mapping**: Use `/api/mcp` for main protocol endpoint  
3. **Message Format**: Same JSON-RPC format, but over HTTP POST
4. **Session Management**: HTTP is stateless, no persistent session needed
5. **Error Handling**: HTTP status codes + JSON-RPC error responses