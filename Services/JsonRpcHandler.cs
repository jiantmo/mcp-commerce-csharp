using System.Text.Json;
using Microsoft.Extensions.Logging;
using McpCommerceServer.Models;

namespace McpCommerceServer.Services;

public class JsonRpcHandler
{
    private readonly ILogger<JsonRpcHandler> _logger;
    private readonly ProductService _productService;

    public JsonRpcHandler(ILogger<JsonRpcHandler> logger, ProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    public async Task<string> HandleRequestAsync(string jsonRequest)
    {
        try
        {
            var message = JsonSerializer.Deserialize<McpMessage>(jsonRequest);
            if (message == null)
            {
                return CreateErrorResponse(null, -32700, "Parse error");
            }

            return message.Method switch
            {
                "initialize" => await HandleInitializeAsync(message),
                "tools/list" => await HandleToolsListAsync(message),
                "tools/call" => await HandleToolsCallAsync(message),
                "resources/list" => await HandleResourcesListAsync(message),
                "resources/read" => await HandleResourcesReadAsync(message),
                _ => CreateErrorResponse(message.Id, -32601, "Method not found")
            };
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error");
            return CreateErrorResponse(null, -32700, "Parse error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error handling request");
            return CreateErrorResponse(null, -32603, "Internal error");
        }
    }

    private async Task<string> HandleInitializeAsync(McpMessage message)
    {
        var response = new McpMessage
        {
            Id = message.Id,
            Result = new InitializeResponse
            {
                Capabilities = new ServerCapabilities
                {
                    Tools = new ToolsCapability { ListChanged = false },
                    Resources = new ResourcesCapability { Subscribe = false, ListChanged = false }
                }
            }
        };

        return JsonSerializer.Serialize(response);
    }

    private async Task<string> HandleToolsListAsync(McpMessage message)
    {
        var tools = new object[]
        {
            // Core Product Search and Retrieval APIs
            new
            {
                name = "products_search",
                description = "Search for products using advanced ProductSearchCriteria with OData query support - D365 Commerce CSU Products/Search",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        productSearchCriteria = new
                        {
                            type = "object",
                            description = "ProductSearchCriteria object based on D365 Commerce metadata",
                            properties = new
                            {
                                channelId = new { type = "number", description = "Channel identifier" },
                                catalogId = new { type = "number", description = "Catalog identifier" },
                                categoryId = new { type = "number", description = "Category identifier" },
                                searchCondition = new { type = "string", description = "Search condition" },
                                searchText = new { type = "string", description = "Search text" },
                                includeProductsFromSubcategories = new { type = "boolean", description = "Include products from subcategories" },
                                skipVariantExpansion = new { type = "boolean", description = "Skip variant expansion" }
                            }
                        },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "QueryResultSettings for paging and sorting",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        top = new { type = "number", description = "Number of results to return (default: 50)" },
                                        skip = new { type = "number", description = "Number of results to skip (default: 0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "productSearchCriteria" }
                }
            },
            new
            {
                name = "products_get_by_id",
                description = "Get a SimpleProduct by its record identifier - D365 Commerce CSU Products/GetById",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        recordId = new { type = "number", description = "Product record identifier" },
                        channelId = new { type = "number", description = "Channel identifier" }
                    },
                    required = new[] { "recordId", "channelId" }
                }
            },
            new
            {
                name = "products_get_by_ids",
                description = "Get multiple products by their record identifiers - D365 Commerce CSU Products/GetByIds",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        channelId = new { type = "number", description = "Channel identifier" },
                        productIds = new
                        {
                            type = "array",
                            items = new { type = "number" },
                            description = "Array of product record identifiers"
                        },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "QueryResultSettings for paging and sorting",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        top = new { type = "number", description = "Number of results to return (default: 50)" },
                                        skip = new { type = "number", description = "Number of results to skip (default: 0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "channelId", "productIds" }
                }
            },
            new
            {
                name = "products_get_recommended",
                description = "Get product recommendations - D365 Commerce CSU Products/GetRecommendedProducts",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        productIds = new
                        {
                            type = "array",
                            items = new { type = "number" },
                            description = "Array of product record identifiers"
                        },
                        customerAccountNumber = new { type = "string", description = "Customer account number (optional)" },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "QueryResultSettings for paging and sorting",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        top = new { type = "number", description = "Number of results to return (default: 50)" },
                                        skip = new { type = "number", description = "Number of results to skip (default: 0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "productIds" }
                }
            },
            new
            {
                name = "products_compare",
                description = "Compare products for detailed analysis - D365 Commerce CSU Products/Compare",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        channelId = new { type = "number", description = "Channel identifier" },
                        catalogId = new { type = "number", description = "Catalog identifier" },
                        productIds = new
                        {
                            type = "array",
                            items = new { type = "number" },
                            description = "Array of product record identifiers to compare"
                        },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "QueryResultSettings for paging and sorting",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        top = new { type = "number", description = "Number of results to return (default: 50)" },
                                        skip = new { type = "number", description = "Number of results to skip (default: 0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "channelId", "catalogId", "productIds" }
                }
            },
            new
            {
                name = "products_search_by_category",
                description = "Search products within a specific category - D365 Commerce CSU Products/SearchByCategory",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        channelId = new { type = "number", description = "Channel identifier" },
                        catalogId = new { type = "number", description = "Catalog identifier" },
                        categoryId = new { type = "number", description = "Category identifier" },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "QueryResultSettings for paging and sorting",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        top = new { type = "number", description = "Number of results to return (default: 50)" },
                                        skip = new { type = "number", description = "Number of results to skip (default: 0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "channelId", "catalogId", "categoryId" }
                }
            },
            new
            {
                name = "products_search_by_text",
                description = "Search products using text search - D365 Commerce CSU Products/SearchByText",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        channelId = new { type = "number", description = "Channel identifier" },
                        catalogId = new { type = "number", description = "Catalog identifier" },
                        searchText = new { type = "string", description = "Search text to find products" },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "QueryResultSettings for paging and sorting",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        top = new { type = "number", description = "Number of results to return (default: 50)" },
                                        skip = new { type = "number", description = "Number of results to skip (default: 0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "channelId", "catalogId", "searchText" }
                }
            },
            
            // Search Enhancement APIs
            new
            {
                name = "products_get_search_suggestions",
                description = "Get recommended search phrases based on partial search text - D365 Commerce CSU Products/GetSearchSuggestions",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        channelId = new { type = "number", description = "Channel identifier" },
                        catalogId = new { type = "number", description = "Catalog identifier" },
                        searchText = new { type = "string", description = "Partial search text" },
                        hitPrefix = new { type = "string", description = "Prefix for highlighting (optional)" },
                        hitSuffix = new { type = "string", description = "Suffix for highlighting (optional)" },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "QueryResultSettings for paging and sorting",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        top = new { type = "number", description = "Number of results to return (default: 10)" },
                                        skip = new { type = "number", description = "Number of results to skip (default: 0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "channelId", "catalogId", "searchText" }
                }
            },
            new
            {
                name = "products_get_refiners_by_category",
                description = "Get product refiners available for category products - D365 Commerce CSU Products/GetRefinersByCategory",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        catalogId = new { type = "number", description = "Catalog identifier" },
                        categoryId = new { type = "number", description = "Category identifier" },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "QueryResultSettings for paging and sorting",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        top = new { type = "number", description = "Number of results to return (default: 50)" },
                                        skip = new { type = "number", description = "Number of results to skip (default: 0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "catalogId", "categoryId" }
                }
            },
            new
            {
                name = "products_get_refiners_by_text",
                description = "Get product refiners available for text search results - D365 Commerce CSU Products/GetRefinersByText",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        catalogId = new { type = "number", description = "Catalog identifier" },
                        searchText = new { type = "string", description = "Search text" },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "QueryResultSettings for paging and sorting",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        top = new { type = "number", description = "Number of results to return (default: 50)" },
                                        skip = new { type = "number", description = "Number of results to skip (default: 0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "catalogId", "searchText" }
                }
            },
            
            // Product Variant and Dimension APIs
            new
            {
                name = "products_get_dimension_values",
                description = "Get dimension values for a product based on specified requirements - D365 Commerce CSU Products/GetDimensionValues",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        recordId = new { type = "number", description = "Product record identifier" },
                        channelId = new { type = "number", description = "Channel identifier" },
                        dimension = new { type = "number", description = "Dimension type (e.g., Color=1, Size=2, Style=3, Config=4)" },
                        matchingDimensionValues = new
                        {
                            type = "array",
                            items = new { type = "object" },
                            description = "Array of existing dimension values to match against"
                        },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "QueryResultSettings for paging and sorting",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        top = new { type = "number", description = "Number of results to return (default: 50)" },
                                        skip = new { type = "number", description = "Number of results to skip (default: 0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "recordId", "channelId", "dimension" }
                }
            },
            new
            {
                name = "products_get_variants_by_dimension_values",
                description = "Get product variations based on specified dimension requirements - D365 Commerce CSU Products/GetVariantsByDimensionValues",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        recordId = new { type = "number", description = "Product record identifier" },
                        channelId = new { type = "number", description = "Channel identifier" },
                        matchingDimensionValues = new
                        {
                            type = "array",
                            items = new { type = "object" },
                            description = "Array of dimension values to match for finding variants"
                        },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "QueryResultSettings for paging and sorting",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        top = new { type = "number", description = "Number of results to return (default: 50)" },
                                        skip = new { type = "number", description = "Number of results to skip (default: 0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "recordId", "channelId", "matchingDimensionValues" }
                }
            },
            
            // Product Information APIs
            new
            {
                name = "products_get_attribute_values",
                description = "Get attribute values of the specified product - D365 Commerce CSU Products/GetAttributeValues",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        recordId = new { type = "number", description = "Product record identifier" },
                        channelId = new { type = "number", description = "Channel identifier" },
                        catalogId = new { type = "number", description = "Catalog identifier" },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "QueryResultSettings for paging and sorting",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        top = new { type = "number", description = "Number of results to return (default: 50)" },
                                        skip = new { type = "number", description = "Number of results to skip (default: 0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "recordId", "channelId", "catalogId" }
                }
            },
            new
            {
                name = "products_get_price",
                description = "Get the price of a product in context of the current customer - D365 Commerce CSU Products/GetPrice",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        recordId = new { type = "number", description = "Product record identifier" },
                        customerAccountNumber = new { type = "string", description = "Customer account number (optional)" },
                        unitOfMeasureSymbol = new { type = "string", description = "Unit of measure symbol (optional)" }
                    },
                    required = new[] { "recordId" }
                }
            },
            new
            {
                name = "products_get_availability",
                description = "Get available inventory for given list of items for given channel and customer - D365 Commerce CSU Products/GetProductAvailabilities",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        itemIds = new
                        {
                            type = "array",
                            items = new { type = "number" },
                            description = "Array of item identifiers"
                        },
                        channelId = new { type = "number", description = "Channel identifier" },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "QueryResultSettings for paging and sorting",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        top = new { type = "number", description = "Number of results to return (default: 50)" },
                                        skip = new { type = "number", description = "Number of results to skip (default: 0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "itemIds", "channelId" }
                }
            },
            new
            {
                name = "products_get_media_locations",
                description = "Get media locations for the specified product - D365 Commerce CSU Products/GetMediaLocations",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        recordId = new { type = "number", description = "Product record identifier" },
                        channelId = new { type = "number", description = "Channel identifier" },
                        catalogId = new { type = "number", description = "Catalog identifier" },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "QueryResultSettings for paging and sorting",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        top = new { type = "number", description = "Number of results to return (default: 50)" },
                                        skip = new { type = "number", description = "Number of results to skip (default: 0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "recordId", "channelId", "catalogId" }
                }
            },
            new
            {
                name = "products_get_units_of_measure",
                description = "Get units of measure for the specified product - D365 Commerce CSU Products/GetUnitsOfMeasure",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        recordId = new { type = "number", description = "Product record identifier" },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "QueryResultSettings for paging and sorting",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        top = new { type = "number", description = "Number of results to return (default: 50)" },
                                        skip = new { type = "number", description = "Number of results to skip (default: 0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "recordId" }
                }
            }
        };

        var response = new McpMessage
        {
            Id = message.Id,
            Result = new { tools }
        };

        return JsonSerializer.Serialize(response);
    }

    private async Task<string> HandleToolsCallAsync(McpMessage message)
    {
        try
        {
            var request = JsonSerializer.Deserialize<JsonElement>(message.Params?.ToString() ?? "{}");
            var toolName = request.GetProperty("name").GetString();
            var arguments = request.GetProperty("arguments");

            var result = toolName switch
            {
                // Core Product Search and Retrieval APIs
                "products_search" => await _productService.SearchProductsAsync(arguments),
                "products_get_by_id" => await _productService.GetProductByIdAsync(arguments),
                "products_get_by_ids" => await _productService.GetProductsByIdsAsync(arguments),
                "products_get_recommended" => await _productService.GetRecommendedProductsAsync(arguments),
                "products_compare" => await _productService.CompareProductsAsync(arguments),
                "products_search_by_category" => await _productService.SearchProductsByCategoryAsync(arguments),
                "products_search_by_text" => await _productService.SearchProductsByTextAsync(arguments),
                
                // Search Enhancement APIs
                "products_get_search_suggestions" => await _productService.GetSearchSuggestionsAsync(arguments),
                "products_get_refiners_by_category" => await _productService.GetRefinersByCategoryAsync(arguments),
                "products_get_refiners_by_text" => await _productService.GetRefinersByTextAsync(arguments),
                
                // Product Variant and Dimension APIs
                "products_get_dimension_values" => await _productService.GetDimensionValuesAsync(arguments),
                "products_get_variants_by_dimension_values" => await _productService.GetVariantsByDimensionValuesAsync(arguments),
                
                // Product Information APIs
                "products_get_attribute_values" => await _productService.GetAttributeValuesAsync(arguments),
                "products_get_price" => await _productService.GetProductPriceAsync(arguments),
                "products_get_availability" => await _productService.GetProductAvailabilitiesAsync(arguments),
                "products_get_media_locations" => await _productService.GetMediaLocationsAsync(arguments),
                "products_get_units_of_measure" => await _productService.GetUnitsOfMeasureAsync(arguments),
                
                _ => throw new InvalidOperationException($"Unknown tool: {toolName}")
            };

            var response = new McpMessage
            {
                Id = message.Id,
                Result = new
                {
                    content = new[]
                    {
                        new
                        {
                            type = "text",
                            text = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true })
                        }
                    }
                }
            };

            return JsonSerializer.Serialize(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing tool");
            return CreateErrorResponse(message.Id, -32603, $"Tool execution error: {ex.Message}");
        }
    }

    private async Task<string> HandleResourcesListAsync(McpMessage message)
    {
        var resources = new object[0];

        var response = new McpMessage
        {
            Id = message.Id,
            Result = new { resources }
        };

        return JsonSerializer.Serialize(response);
    }

    private async Task<string> HandleResourcesReadAsync(McpMessage message)
    {
        return CreateErrorResponse(message.Id, -32601, "No resources available");
    }

    private string CreateErrorResponse(object? id, int code, string message)
    {
        var response = new McpMessage
        {
            Id = id,
            Error = new McpError
            {
                Code = code,
                Message = message
            }
        };

        return JsonSerializer.Serialize(response);
    }
}