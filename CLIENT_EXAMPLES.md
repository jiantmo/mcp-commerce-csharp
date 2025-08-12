# MCP Remote Client Examples

This document provides examples of how to interact with the remote MCP Commerce server.

## Prerequisites

The MCP Commerce server should be running on `http://localhost:8080`.

Start the server with:
```bash
dotnet run
```

## HTTP Client Examples

### 1. JavaScript/Browser Client

```javascript
class McpCommerceClient {
    constructor(baseUrl = 'http://localhost:8080/api/mcp') {
        this.baseUrl = baseUrl;
        this.sessionId = null;
    }

    async initialize() {
        const response = await fetch(`${this.baseUrl}/initialize`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                id: '1',
                params: { protocolVersion: '2024-11-05' }
            })
        });
        const result = await response.json();
        this.sessionId = result.id;
        return result;
    }

    async listTools() {
        return await this.makeRequest('/tools/list', {});
    }

    async searchProducts(searchText, channelId = 1, catalogId = 1) {
        return await this.callTool('products_search_by_text', {
            channelId,
            catalogId,
            searchText
        });
    }

    async getProduct(recordId, channelId = 1) {
        return await this.callTool('products_get_by_id', {
            recordId,
            channelId
        });
    }

    async callTool(toolName, arguments) {
        return await this.makeRequest('/tools/call', {
            name: toolName,
            arguments
        });
    }

    async makeRequest(endpoint, params) {
        const response = await fetch(`${this.baseUrl}${endpoint}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                id: Date.now().toString(),
                params
            })
        });
        return await response.json();
    }
}

// Usage example
async function example() {
    const client = new McpCommerceClient();
    
    // Initialize session
    await client.initialize();
    console.log('MCP session initialized');
    
    // List available tools
    const tools = await client.listTools();
    console.log('Available tools:', tools);
    
    // Search for products
    const products = await client.searchProducts('laptop');
    console.log('Products found:', products);
    
    // Get specific product
    const product = await client.getProduct(1);
    console.log('Product details:', product);
}
```

### 2. Python Client

```python
import requests
import json
from typing import Dict, Any, Optional

class McpCommerceClient:
    def __init__(self, base_url: str = "http://localhost:8080/api/mcp"):
        self.base_url = base_url
        self.session_id: Optional[str] = None
        self.session = requests.Session()
    
    def initialize(self) -> Dict[str, Any]:
        """Initialize MCP session"""
        response = self.session.post(
            f"{self.base_url}/initialize",
            json={
                "id": "1",
                "params": {"protocolVersion": "2024-11-05"}
            }
        )
        result = response.json()
        self.session_id = result.get('id')
        return result
    
    def list_tools(self) -> Dict[str, Any]:
        """List available tools"""
        return self._make_request('/tools/list', {})
    
    def search_products(self, search_text: str, channel_id: int = 1, catalog_id: int = 1) -> Dict[str, Any]:
        """Search for products by text"""
        return self.call_tool('products_search_by_text', {
            'channelId': channel_id,
            'catalogId': catalog_id,
            'searchText': search_text
        })
    
    def get_product(self, record_id: int, channel_id: int = 1) -> Dict[str, Any]:
        """Get product by ID"""
        return self.call_tool('products_get_by_id', {
            'recordId': record_id,
            'channelId': channel_id
        })
    
    def call_tool(self, tool_name: str, arguments: Dict[str, Any]) -> Dict[str, Any]:
        """Call a specific tool"""
        return self._make_request('/tools/call', {
            'name': tool_name,
            'arguments': arguments
        })
    
    def _make_request(self, endpoint: str, params: Dict[str, Any]) -> Dict[str, Any]:
        """Make a request to the MCP server"""
        response = self.session.post(
            f"{self.base_url}{endpoint}",
            json={
                "id": str(int(time.time() * 1000)),
                "params": params
            }
        )
        response.raise_for_status()
        return response.json()

# Usage example
if __name__ == "__main__":
    import time
    
    client = McpCommerceClient()
    
    # Initialize session
    init_result = client.initialize()
    print("MCP session initialized:", init_result)
    
    # List available tools
    tools = client.list_tools()
    print("Available tools:", json.dumps(tools, indent=2))
    
    # Search for products
    products = client.search_products('laptop')
    print("Products found:", json.dumps(products, indent=2))
    
    # Get specific product
    product = client.get_product(1)
    print("Product details:", json.dumps(product, indent=2))
```

### 3. cURL Examples

```bash
# Initialize session
curl -X POST http://localhost:8080/api/mcp/initialize \
  -H "Content-Type: application/json" \
  -d '{
    "id": "1",
    "params": {
      "protocolVersion": "2024-11-05"
    }
  }'

# List tools
curl -X POST http://localhost:8080/api/mcp/tools/list \
  -H "Content-Type: application/json" \
  -d '{"id": "2"}'

# Search products by text
curl -X POST http://localhost:8080/api/mcp/tools/call \
  -H "Content-Type: application/json" \
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

# Get product by ID
curl -X POST http://localhost:8080/api/mcp/tools/call \
  -H "Content-Type: application/json" \
  -d '{
    "id": "4",
    "params": {
      "name": "products_get_by_id",
      "arguments": {
        "recordId": 1,
        "channelId": 1
      }
    }
  }'

# Advanced product search
curl -X POST http://localhost:8080/api/mcp/tools/call \
  -H "Content-Type: application/json" \
  -d '{
    "id": "5",
    "params": {
      "name": "products_search",
      "arguments": {
        "searchCriteria": {
          "channelId": 1,
          "catalogId": 1,
          "categoryId": 1,
          "searchText": "gaming",
          "includeProductsFromSubcategories": true
        },
        "querySettings": {
          "top": 10,
          "skip": 0
        }
      }
    }
  }'
```

### 4. Health and Capabilities Endpoints

```bash
# Health check
curl http://localhost:8080/api/mcp/health

# Server capabilities
curl http://localhost:8080/api/mcp/capabilities

# Swagger UI (open in browser)
# http://localhost:8080
```

### 5. PowerShell Client

```powershell
# PowerShell client example
$baseUrl = "http://localhost:8080/api/mcp"

function Invoke-McpRequest {
    param(
        [string]$Endpoint,
        [hashtable]$Params = @{},
        [string]$Id = [System.Guid]::NewGuid().ToString()
    )
    
    $body = @{
        id = $Id
        params = $Params
    } | ConvertTo-Json -Depth 10
    
    $response = Invoke-RestMethod -Uri "$baseUrl$Endpoint" -Method POST -Body $body -ContentType "application/json"
    return $response
}

# Initialize session
$initResult = Invoke-McpRequest -Endpoint "/initialize" -Params @{
    protocolVersion = "2024-11-05"
}
Write-Host "Initialized: $($initResult | ConvertTo-Json)"

# List tools
$tools = Invoke-McpRequest -Endpoint "/tools/list"
Write-Host "Tools: $($tools | ConvertTo-Json -Depth 5)"

# Search products
$products = Invoke-McpRequest -Endpoint "/tools/call" -Params @{
    name = "products_search_by_text"
    arguments = @{
        channelId = 1
        catalogId = 1
        searchText = "laptop"
    }
}
Write-Host "Products: $($products | ConvertTo-Json -Depth 5)"
```

## Integration with MCP Clients

### Claude Desktop Integration

For Claude Desktop, configure in `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "d365-commerce-remote": {
      "transport": {
        "type": "http",
        "baseUrl": "http://localhost:8080/api/mcp"
      }
    }
  }
}
```

### Custom MCP Client

For custom MCP clients that support HTTP transport, use:

- **Base URL**: `http://localhost:8080/api/mcp`
- **Transport**: HTTP POST with JSON payload
- **Protocol**: JSON-RPC 2.0
- **Content-Type**: `application/json`

## Error Handling

The server returns standard HTTP status codes and JSON-RPC error responses:

```json
{
  "jsonrpc": "2.0",
  "id": "1",
  "error": {
    "code": -32603,
    "message": "Internal error",
    "data": "Additional error details"
  }
}
```

Common error codes:
- `-32700`: Parse error
- `-32600`: Invalid request
- `-32601`: Method not found
- `-32602`: Invalid params
- `-32603`: Internal error
