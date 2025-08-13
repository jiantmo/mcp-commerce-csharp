# Test MCP Server

This folder contains test scripts and examples for testing the D365 Commerce MCP Server.

## Testing the Server

### 1. Start the Server

```bash
dotnet run --project McpCommerceServer.csproj
```

### 2. Test with MCP Client

You can test the server using any MCP client or by sending JSON-RPC messages directly.

#### Initialize the Server

```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "initialize",
  "params": {
    "protocolVersion": "2025-06-18",
    "capabilities": {},
    "clientInfo": {
      "name": "test-client",
      "version": "1.0.0"
    }
  }
}
```

#### Send Initialized Notification

After receiving the initialize response, send:

```json
{
  "jsonrpc": "2.0",
  "method": "notifications/initialized"
}
```

#### List Available Tools

```json
{
  "jsonrpc": "2.0",
  "id": 2,
  "method": "tools/list"
}
```

#### Call Customer Search Tool

```json
{
  "jsonrpc": "2.0",
  "id": 3,
  "method": "tools/call",
  "params": {
    "name": "customer_search",
    "arguments": {
      "customerSearchCriteria": {
        "searchText": "john doe",
        "email": "john.doe@example.com"
      },
      "queryResultSettings": {
        "paging": {
          "skip": 0,
          "top": 10
        },
        "sorting": {
          "field": "FullName",
          "isDescending": false
        }
      }
    }
  }
}
```

### Expected Responses

#### Initialize Response
```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "result": {
    "protocolVersion": "2025-06-18",
    "capabilities": {
      "tools": {
        "listChanged": true
      }
    },
    "serverInfo": {
      "name": "D365CommerceServer",
      "title": "D365 Commerce MCP Server",
      "version": "1.0.0"
    }
  }
}
```

#### Tools List Response
```json
{
  "jsonrpc": "2.0",
  "id": 2,
  "result": {
    "tools": [
      {
        "name": "customer_search",
        "title": "Customer Search",
        "description": "Search for customers in D365 Commerce based on search criteria...",
        "inputSchema": {
          "type": "object",
          "properties": {
            "customerSearchCriteria": {
              "type": "object",
              "description": "Search criteria for finding customers",
              "properties": {
                "searchText": {
                  "type": "string",
                  "description": "General search text to find customers"
                }
              }
            }
          },
          "required": ["customerSearchCriteria"]
        }
      }
    ]
  }
}
```

#### Customer Search Response
```json
{
  "jsonrpc": "2.0",
  "id": 3,
  "result": {
    "content": [
      {
        "type": "text",
        "text": "{\n  \"Results\": [],\n  \"TotalCount\": 0,\n  \"HasNextPage\": false,\n  \"HasPreviousPage\": false\n}"
      }
    ],
    "isError": false
  }
}
```

Note: The current implementation returns empty results as the actual D365 Commerce integration is not yet implemented.
