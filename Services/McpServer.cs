using McpCommerceServer.Controllers;
using McpCommerceServer.Mcp;
using McpCommerceServer.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace McpCommerceServer.Services
{
    public class McpServer
    {
        private readonly ILogger<McpServer> _logger;
        private readonly CustomerController _customerController;

        public McpServer(ILogger<McpServer> logger, CustomerController customerController)
        {
            _logger = logger;
            _customerController = customerController;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("D365 Commerce MCP Server starting...");

            try
            {
                while (true)
                {
                    var input = await Console.In.ReadLineAsync();
                    if (string.IsNullOrEmpty(input))
                        continue;

                    _logger.LogDebug("Received request: {Request}", input);

                    try
                    {
                        var request = JsonSerializer.Deserialize<McpRequest>(input);
                        if (request != null)
                        {
                            var response = await ProcessRequestAsync(request);
                            
                            // Only send response if it's not null (notifications return null)
                            if (response != null)
                            {
                                var responseJson = JsonSerializer.Serialize(response);
                                Console.WriteLine(responseJson);
                                _logger.LogDebug("Sent response: {Response}", responseJson);
                            }
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Failed to parse JSON request");
                        var errorResponse = new McpResponse
                        {
                            Id = null,
                            Error = new McpError
                            {
                                Code = -32700,
                                Message = "Parse error"
                            }
                        };
                        Console.WriteLine(JsonSerializer.Serialize(errorResponse));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MCP Server encountered an error");
            }
        }

        private async Task<McpResponse> ProcessRequestAsync(McpRequest request)
        {
            try
            {
                switch (request.Method)
                {
                    case "initialize":
                    case "notifications/initialized":
                        return HandleInitialize(request);

                    case "tools/list":
                        return HandleToolsList(request);

                    case "tools/call":
                        return await HandleToolCallAsync(request);

                    default:
                        return new McpResponse
                        {
                            Id = request.Id,
                            Error = new McpError
                            {
                                Code = -32601,
                                Message = $"Method not found: {request.Method}"
                            }
                        };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request: {Method}", request.Method);
                return new McpResponse
                {
                    Id = request.Id,
                    Error = new McpError
                    {
                        Code = -32603,
                        Message = "Internal error",
                        Data = ex.Message
                    }
                };
            }
        }

        private McpResponse HandleInitialize(McpRequest request)
        {
            _logger.LogInformation("Initializing MCP Server");
            
            return new McpResponse
            {
                Id = request.Id,
                Result = new
                {
                    protocolVersion = "2025-06-18",
                    capabilities = new
                    {
                        tools = new 
                        { 
                            listChanged = true
                        }
                    },
                    serverInfo = new
                    {
                        name = "D365CommerceServer",
                        title = "D365 Commerce MCP Server",
                        version = "1.0.0"
                    }
                }
            };
        }

        private void HandleInitialized(McpRequest request)
        {
            _logger.LogInformation("Client confirmed initialization completed");
            // This is a notification, no response needed
        }

        private McpResponse HandleToolsList(McpRequest request)
        {
            _logger.LogInformation("Listing available tools");

            var tools = new List<McpToolDefinition>
            {
                _customerController.GetCustomerSearchToolDefinition()
            };

            var result = new ListToolsResult
            {
                Tools = tools
            };

            // Handle pagination if cursor is provided in params
            if (request.Params != null)
            {
                var paramsJson = JsonSerializer.Serialize(request.Params);
                var listParams = JsonSerializer.Deserialize<Dictionary<string, object>>(paramsJson);
                
                if (listParams?.ContainsKey("cursor") == true)
                {
                    // For this simple implementation, we don't actually paginate
                    // but we acknowledge the cursor parameter
                    _logger.LogDebug("Received pagination cursor: {Cursor}", listParams["cursor"]);
                }
            }

            return new McpResponse
            {
                Id = request.Id,
                Result = result
            };
        }

        private async Task<McpResponse> HandleToolCallAsync(McpRequest request)
        {
            if (request.Params == null)
            {
                return new McpResponse
                {
                    Id = request.Id,
                    Error = new McpError
                    {
                        Code = -32602,
                        Message = "Invalid params"
                    }
                };
            }

            var toolCallJson = JsonSerializer.Serialize(request.Params);
            var toolCall = JsonSerializer.Deserialize<McpToolCallRequest>(toolCallJson);

            if (toolCall == null)
            {
                return new McpResponse
                {
                    Id = request.Id,
                    Error = new McpError
                    {
                        Code = -32602,
                        Message = "Invalid tool call format"
                    }
                };
            }

            _logger.LogInformation("Calling tool: {ToolName}", toolCall.Name);

            McpToolCallResult result;

            switch (toolCall.Name)
            {
                case "customer_search":
                    result = await HandleCustomerSearchAsync(toolCall);
                    break;

                default:
                    result = new McpToolCallResult
                    {
                        Content = new List<McpContent>
                        {
                            new McpContent
                            {
                                Type = "text",
                                Text = $"Unknown tool: {toolCall.Name}"
                            }
                        },
                        IsError = true
                    };
                    break;
            }

            return new McpResponse
            {
                Id = request.Id,
                Result = result
            };
        }

        private async Task<McpToolCallResult> HandleCustomerSearchAsync(McpToolCallRequest toolCall)
        {
            try
            {
                var customerSearchCriteria = new CustomerSearchCriteria();
                QueryResultSettings? queryResultSettings = null;

                if (toolCall.Arguments != null)
                {
                    if (toolCall.Arguments.ContainsKey("customerSearchCriteria"))
                    {
                        var criteriaJson = JsonSerializer.Serialize(toolCall.Arguments["customerSearchCriteria"]);
                        customerSearchCriteria = JsonSerializer.Deserialize<CustomerSearchCriteria>(criteriaJson) 
                            ?? new CustomerSearchCriteria();
                    }

                    if (toolCall.Arguments.ContainsKey("queryResultSettings"))
                    {
                        var settingsJson = JsonSerializer.Serialize(toolCall.Arguments["queryResultSettings"]);
                        queryResultSettings = JsonSerializer.Deserialize<QueryResultSettings>(settingsJson);
                    }
                }

                return await _customerController.CustomerSearchAsync(customerSearchCriteria, queryResultSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling customer_search tool call");
                return new McpToolCallResult
                {
                    Content = new List<McpContent>
                    {
                        new McpContent
                        {
                            Type = "text",
                            Text = $"Error: {ex.Message}"
                        }
                    },
                    IsError = true
                };
            }
        }
    }
}
