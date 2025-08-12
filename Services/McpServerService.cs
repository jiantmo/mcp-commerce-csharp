using System.Text;
using Microsoft.Extensions.Logging;

namespace McpCommerceServer.Services;

public class McpServerService
{
    private readonly ILogger<McpServerService> _logger;
    private readonly JsonRpcHandler _jsonRpcHandler;

    public McpServerService(ILogger<McpServerService> logger, JsonRpcHandler jsonRpcHandler)
    {
        _logger = logger;
        _jsonRpcHandler = jsonRpcHandler;
    }

    public async Task StartAsync()
    {
        _logger.LogInformation("Starting MCP D365 Commerce Server...");
        
        await Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    var input = await ReadLineAsync();
                    if (string.IsNullOrEmpty(input))
                        continue;

                    _logger.LogDebug("Received request: {Request}", input);

                    var response = await _jsonRpcHandler.HandleRequestAsync(input);
                    
                    _logger.LogDebug("Sending response: {Response}", response);
                    
                    await Console.Out.WriteLineAsync(response);
                    await Console.Out.FlushAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing request");
                }
            }
        });
    }

    private async Task<string> ReadLineAsync()
    {
        var buffer = new char[1];
        var result = new StringBuilder();
        var input = Console.In;

        int ch;
        while ((ch = await input.ReadAsync(buffer, 0, 1)) > 0)
        {
            var character = buffer[0];
            if (character == '\n')
                break;
            
            if (character != '\r')
                result.Append(character);
        }

        return result.ToString();
    }
}