# D365 Commerce MCP Server - Setup Guide

This guide explains how to register and use the D365 Commerce MCP Server with Claude Desktop.

## Prerequisites

1. **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **Claude Desktop Application** - [Download here](https://claude.ai/desktop)
3. **D365 Commerce Environment** (optional for testing) - Can use the sandbox at `https://d365commerceret.sandbox.operations.dynamics.com`

## Building the MCP Server

1. **Clone or download the project:**
   ```bash
   cd C:\github\mcp-commerce-csharp
   ```

2. **Build the project:**
   ```bash
   dotnet build McpCommerceServer.csproj
   ```

3. **Test run (optional):**
   ```bash
   dotnet run --project McpCommerceServer.csproj
   ```

## Registering with Claude Desktop

### Step 1: Locate Claude Desktop Configuration

Claude Desktop stores its configuration in different locations depending on your OS:

- **Windows:** `%APPDATA%\Claude\claude_desktop_config.json`
- **macOS:** `~/Library/Application Support/Claude/claude_desktop_config.json`
- **Linux:** `~/.config/Claude/claude_desktop_config.json`

### Step 2: Configure the MCP Server

Add the following configuration to your `claude_desktop_config.json` file:

```json
{
  "mcpServers": {
    "d365-commerce": {
      "command": "dotnet",
      "args": [
        "run", 
        "--project", 
        "C:\\github\\mcp-commerce-csharp\\McpCommerceServer.csproj"
      ],
      "env": {
        "D365Commerce__BaseUrl": "https://d365commerceret.sandbox.operations.dynamics.com"
      }
    }
  }
}
```

**Important:** Update the path in `args` to match your actual project location.

### Step 3: Alternative Configuration (Production)

For production deployment, you can build and publish the application first:

```bash
dotnet publish McpCommerceServer.csproj -c Release -o ./publish
```

Then use this configuration:

```json
{
  "mcpServers": {
    "d365-commerce": {
      "command": "C:\\github\\mcp-commerce-csharp\\publish\\McpCommerceServer.exe",
      "args": [],
      "env": {
        "D365Commerce__BaseUrl": "https://your-d365-commerce-instance.com"
      }
    }
  }
}
```

### Step 4: Restart Claude Desktop

After updating the configuration:
1. **Completely quit** Claude Desktop (check system tray)
2. **Restart** Claude Desktop
3. **Verify** the MCP server appears in Claude's available tools

## Configuration Options

### Environment Variables

You can configure the following environment variables:

```json
"env": {
  "D365Commerce__BaseUrl": "https://your-d365-commerce-instance.com",
  "Logging__LogLevel__Default": "Information",
  "ASPNETCORE_ENVIRONMENT": "Production"
}
```

### appsettings.json Configuration

Alternatively, update `appsettings.json` in the project:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "D365Commerce": {
    "BaseUrl": "https://your-d365-commerce-instance.com",
    "ApiVersion": "v1",
    "Timeout": 30
  }
}
```

## Available MCP Tools

Once registered, Claude will have access to these D365 Commerce tools:

### 1. `products_search`
Search for products using advanced criteria:
```json
{
  "searchCriteria": {
    "channelId": 1,
    "catalogId": 1,
    "searchText": "laptop",
    "includeProductsFromSubcategories": true
  }
}
```

### 2. `products_get_by_id`
Get a specific product by ID:
```json
{
  "recordId": 12345,
  "channelId": 1
}
```

### 3. `products_get_by_ids`
Get multiple products by their IDs:
```json
{
  "channelId": 1,
  "productIds": [12345, 67890, 11111]
}
```

### 4. `products_get_recommended`
Get product recommendations:
```json
{
  "recommendationCriteria": {
    "productIds": [12345],
    "catalogId": 1,
    "customerAccountNumber": "CUST001"
  }
}
```

### 5. `products_compare`
Compare multiple products:
```json
{
  "channelId": 1,
  "catalogId": 1,
  "productIds": [12345, 67890]
}
```

### 6. `products_search_by_category`
Search products in a specific category:
```json
{
  "channelId": 1,
  "catalogId": 1,
  "categoryId": 100
}
```

### 7. `products_search_by_text`
Text-based product search:
```json
{
  "channelId": 1,
  "catalogId": 1,
  "searchText": "wireless headphones"
}
```

## Testing the Integration

1. **Start a conversation** with Claude Desktop
2. **Ask Claude** to search for products: 
   > "Can you search for laptops in the D365 Commerce system?"
3. **Claude should respond** with a list of available MCP tools and execute the search

## Troubleshooting

### Common Issues

**1. MCP Server not appearing in Claude:**
- Check that Claude Desktop is completely restarted
- Verify the JSON configuration syntax is correct
- Ensure the file paths in the configuration exist

**2. Build errors:**
- Verify .NET 8.0 SDK is installed: `dotnet --version`
- Run `dotnet restore` to ensure packages are installed

**3. Runtime errors:**
- Check Claude Desktop logs (varies by OS)
- Verify D365 Commerce URL is accessible
- Ensure authentication headers are properly configured

### Logging

To enable detailed logging, update your configuration:

```json
"env": {
  "Logging__LogLevel__Default": "Debug",
  "Logging__LogLevel__McpCommerceServer": "Debug"
}
```

### Development Mode

For development, you can run the server manually to see debug output:

```bash
dotnet run --project McpCommerceServer.csproj
```

Then interact with it via stdin/stdout to test MCP protocol messages.

## Next Steps

1. **Customize** the D365Commerce BaseUrl for your environment
2. **Add authentication** if required by your D365 Commerce instance
3. **Extend** the ProductService with additional D365 Commerce operations
4. **Configure logging** and monitoring for production use

## Support

For issues with:
- **This MCP Server:** Check the project README and issues
- **Claude Desktop:** Visit [Claude Documentation](https://docs.anthropic.com)
- **MCP Protocol:** Visit [Model Context Protocol Documentation](https://modelcontextprotocol.io)