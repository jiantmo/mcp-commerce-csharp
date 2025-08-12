using Microsoft.AspNetCore.Mvc;
using McpCommerceServer.Services;
using System.Text.Json;

namespace McpCommerceServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class McpController : ControllerBase
{
    private readonly JsonRpcHandler _jsonRpcHandler;
    private readonly ILogger<McpController> _logger;
    private readonly IConfiguration _configuration;

    public McpController(JsonRpcHandler jsonRpcHandler, ILogger<McpController> logger, IConfiguration configuration)
    {
        _jsonRpcHandler = jsonRpcHandler;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Handle MCP JSON-RPC requests
    /// </summary>
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> HandleRequest([FromBody] JsonElement request)
    {
        try
        {
            _logger.LogDebug("Received MCP request: {Request}", request.ToString());

            var jsonRequest = request.ToString();
            var response = await _jsonRpcHandler.HandleRequestAsync(jsonRequest);

            _logger.LogDebug("Sending MCP response: {Response}", response);

            return Content(response, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MCP request");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get server capabilities and information
    /// </summary>
    [HttpGet]
    [Route("capabilities")]
    public IActionResult GetCapabilities()
    {
        var capabilities = new
        {
            protocolVersion = _configuration["MCP:ProtocolVersion"] ?? "2024-11-05",
            serverName = _configuration["MCP:ServerName"] ?? "D365 Commerce MCP Server",
            version = _configuration["MCP:Version"] ?? "1.0.0",
            transport = "http",
            capabilities = new
            {
                tools = new { listChanged = false },
                resources = new { subscribe = false, listChanged = false }
            },
            endpoints = new
            {
                mcp = "/api/mcp",
                capabilities = "/api/mcp/capabilities",
                health = "/api/mcp/health"
            }
        };

        return Ok(capabilities);
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet]
    [Route("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            serverName = _configuration["MCP:ServerName"] ?? "D365 Commerce MCP Server",
            version = _configuration["MCP:Version"] ?? "1.0.0"
        });
    }

    /// <summary>
    /// Initialize MCP session (equivalent to stdio initialize)
    /// </summary>
    [HttpPost]
    [Route("initialize")]
    public async Task<IActionResult> Initialize([FromBody] JsonElement initRequest)
    {
        try
        {
            // Create a proper MCP initialize message
            var mcpRequest = new
            {
                jsonrpc = "2.0",
                id = initRequest.TryGetProperty("id", out var idProp) ? idProp.ToString() : "1",
                method = "initialize",
                @params = initRequest.TryGetProperty("params", out var paramsProp) ? paramsProp : new JsonElement()
            };

            var jsonRequest = JsonSerializer.Serialize(mcpRequest);
            var response = await _jsonRpcHandler.HandleRequestAsync(jsonRequest);

            return Content(response, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during MCP initialization");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// List available tools
    /// </summary>
    [HttpPost]
    [Route("tools/list")]
    public async Task<IActionResult> ListTools([FromBody] JsonElement? request = null)
    {
        try
        {
            var mcpRequest = new
            {
                jsonrpc = "2.0",
                id = request?.TryGetProperty("id", out var idProp) == true ? idProp.ToString() : "1",
                method = "tools/list",
                @params = new { }
            };

            var jsonRequest = JsonSerializer.Serialize(mcpRequest);
            var response = await _jsonRpcHandler.HandleRequestAsync(jsonRequest);

            return Content(response, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing tools");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Call a specific tool
    /// </summary>
    [HttpPost]
    [Route("tools/call")]
    public async Task<IActionResult> CallTool([FromBody] JsonElement toolCall)
    {
        try
        {
            var mcpRequest = new
            {
                jsonrpc = "2.0",
                id = toolCall.TryGetProperty("id", out var idProp) ? idProp.ToString() : "1",
                method = "tools/call",
                @params = toolCall.TryGetProperty("params", out var paramsProp) ? paramsProp : toolCall
            };

            var jsonRequest = JsonSerializer.Serialize(mcpRequest);
            var response = await _jsonRpcHandler.HandleRequestAsync(jsonRequest);

            return Content(response, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling tool");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// List available resources
    /// </summary>
    [HttpPost]
    [Route("resources/list")]
    public async Task<IActionResult> ListResources([FromBody] JsonElement? request = null)
    {
        try
        {
            var mcpRequest = new
            {
                jsonrpc = "2.0",
                id = request?.TryGetProperty("id", out var idProp) == true ? idProp.ToString() : "1",
                method = "resources/list",
                @params = new { }
            };

            var jsonRequest = JsonSerializer.Serialize(mcpRequest);
            var response = await _jsonRpcHandler.HandleRequestAsync(jsonRequest);

            return Content(response, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing resources");
            return BadRequest(new { error = ex.Message });
        }
    }
}
