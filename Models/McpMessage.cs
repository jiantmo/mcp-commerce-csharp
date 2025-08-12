using System.Text.Json.Serialization;

namespace McpCommerceServer.Models;

public class McpMessage
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("id")]
    public object? Id { get; set; }

    [JsonPropertyName("method")]
    public string? Method { get; set; }

    [JsonPropertyName("params")]
    public object? Params { get; set; }

    [JsonPropertyName("result")]
    public object? Result { get; set; }

    [JsonPropertyName("error")]
    public McpError? Error { get; set; }
}

public class McpError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public object? Data { get; set; }
}

public class InitializeRequest
{
    [JsonPropertyName("protocolVersion")]
    public string ProtocolVersion { get; set; } = "2024-11-05";

    [JsonPropertyName("capabilities")]
    public ClientCapabilities Capabilities { get; set; } = new();

    [JsonPropertyName("clientInfo")]
    public ClientInfo ClientInfo { get; set; } = new();
}

public class ClientCapabilities
{
    [JsonPropertyName("roots")]
    public RootsCapability? Roots { get; set; }

    [JsonPropertyName("sampling")]
    public object? Sampling { get; set; }
}

public class RootsCapability
{
    [JsonPropertyName("listChanged")]
    public bool ListChanged { get; set; }
}

public class ClientInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;
}

public class InitializeResponse
{
    [JsonPropertyName("protocolVersion")]
    public string ProtocolVersion { get; set; } = "2024-11-05";

    [JsonPropertyName("capabilities")]
    public ServerCapabilities Capabilities { get; set; } = new();

    [JsonPropertyName("serverInfo")]
    public ServerInfo ServerInfo { get; set; } = new();
}

public class ServerCapabilities
{
    [JsonPropertyName("tools")]
    public ToolsCapability? Tools { get; set; }

    [JsonPropertyName("resources")]
    public ResourcesCapability? Resources { get; set; }
}

public class ToolsCapability
{
    [JsonPropertyName("listChanged")]
    public bool ListChanged { get; set; }
}

public class ResourcesCapability
{
    [JsonPropertyName("subscribe")]
    public bool Subscribe { get; set; }

    [JsonPropertyName("listChanged")]
    public bool ListChanged { get; set; }
}

public class ServerInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "D365 Commerce MCP Server";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0.0";
}