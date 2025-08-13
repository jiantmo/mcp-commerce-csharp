# D365 Commerce MCP Server

A C# based Model Context Protocol (MCP) server for Microsoft Dynamics 365 Commerce. This server provides tools for interacting with D365 Commerce customer data through the MCP protocol.

## Features

- **Customer Search Tool**: Search for customers using various criteria including account number, email, phone, or general search text
- **MCP Protocol Compliance**: Fully implements the MCP 2025-06-18 specification
- **Structured Logging**: Built-in logging for debugging and monitoring (writes to stderr to avoid corrupting JSON-RPC messages)
- **Extensible Architecture**: Easy to add new tools and controllers
- **Proper Error Handling**: Comprehensive error handling for both protocol and tool execution errors

## Tools Available

### customer_search

Searches for customers in D365 Commerce based on provided search criteria.

**Parameters:**
- `customerSearchCriteria` (required): Object containing search parameters
  - `searchText` (optional): General search text to find customers
  - `accountNumber` (optional): Customer account number
  - `email` (optional): Customer email address
  - `phone` (optional): Customer phone number
  - `extensionProperties` (optional): Array of additional properties
- `queryResultSettings` (optional): Paging and sorting settings
  - `paging`: Object with `skip` (0-∞) and `top` (1-1000) properties
  - `sorting`: Object with `field` and `isDescending` properties

**Returns:**
PageResult containing matching GlobalCustomer records with properties like:
- PartyNumber
- RecordId
- AccountNumber
- FullName
- FullAddress
- Phone
- Email
- CustomerTypeValue
- And more...

## Project Structure

```
McpCommerceServer/
├── Controllers/
│   └── CustomerController.cs      # Customer-related operations
├── Models/
│   └── DataModels.cs             # D365 Commerce data models
├── Mcp/
│   └── McpModels.cs              # MCP protocol models
├── Services/
│   └── McpServer.cs              # Main MCP server implementation
├── Program.cs                     # Application entry point
└── McpCommerceServer.csproj      # Project file
```

## MCP Compliance

This server implements the **MCP 2025-06-18** specification with:

- ✅ **Protocol Version**: 2025-06-18
- ✅ **Initialization Flow**: Proper `initialize` → `initialized` notification sequence
- ✅ **Tool Capabilities**: Declares `tools` capability with `listChanged` support
- ✅ **JSON-RPC 2.0**: Compliant message format
- ✅ **Structured Tool Definitions**: Includes `name`, `title`, `description`, and detailed `inputSchema`
- ✅ **Error Handling**: Both protocol errors and tool execution errors
- ✅ **Logging Safety**: Writes only to stderr to avoid corrupting STDIO transport

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Visual Studio Code or Visual Studio

### Building the Project

```bash
dotnet build McpCommerceServer.csproj
```

### Running the Server

```bash
dotnet run --project McpCommerceServer.csproj
```

The server will start and listen for MCP protocol messages on standard input/output.

### Publishing

```bash
dotnet publish McpCommerceServer.csproj -c Release -o ./publish --self-contained false
```

## Implementation Status

- ✅ Basic MCP server infrastructure
- ✅ Customer search tool structure
- ⚠️ Customer search logic (placeholder implementation)
- ❌ D365 Commerce API integration
- ❌ Authentication and authorization
- ❌ Additional customer tools (CreateEntity, UpdateEntity, etc.)

## Next Steps

1. **Implement D365 Commerce Integration**: Add actual API calls to D365 Commerce services
2. **Add Authentication**: Implement proper authentication for D365 Commerce
3. **Expand Customer Tools**: Add the remaining customer controller methods
4. **Error Handling**: Enhance error handling and validation
5. **Testing**: Add unit and integration tests
6. **Configuration**: Add configuration management for connection strings and settings

## Configuration

The application uses .NET's built-in configuration system. You can add configuration through:

- `appsettings.json`
- Environment variables
- Command line arguments

Example configuration structure:
```json
{
  "D365Commerce": {
    "BaseUrl": "https://your-d365-commerce-instance.com",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "TenantId": "your-tenant-id"
  }
}
```

## License

This project is licensed under the MIT License.
