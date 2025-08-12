using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using McpCommerceServer.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace McpCommerceServer.Services;

/// <summary>
/// Product Service following D365 Commerce Scale Unit (CSU) Product Controller API patterns.
/// Implements all Product controller endpoints as documented in Microsoft Dynamics 365 Commerce API reference.
/// </summary>
public class ProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _commerceApiPath;

    public ProductService(ILogger<ProductService> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _baseUrl = configuration["D365Commerce:BaseUrl"] ?? "https://your-d365-commerce-instance.com";
        _commerceApiPath = "/Commerce/Products";
        
        // Configure HTTP client defaults
        _httpClient.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("D365Commerce:Timeout", 30));
    }

    /// <summary>
    /// Sets appropriate Commerce headers for the request based on the operation type.
    /// Supports Employee, Customer, Anonymous, and Application roles.
    /// </summary>
    /// <param name="role">Commerce role for the operation</param>
    private void SetCommerceHeaders(string role = "Anonymous")
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Commerce-Role", role);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "MCP-Commerce-Server/1.0");
        
        // Add OData headers for better compatibility
        _httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
        _httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
    }

    /// <summary>
    /// Searches for products using advanced ProductSearchCriteria with OData query support.
    /// Maps to: POST /Commerce/Products/Search
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> SearchProductsAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/Search operation");
        
        var searchCriteria = JsonSerializer.Deserialize<ProductSearchCriteria>(
            arguments.GetProperty("productSearchCriteria").GetRawText());
        
        var querySettings = arguments.TryGetProperty("queryResultSettings", out var qs) 
            ? JsonSerializer.Deserialize<QueryResultSettings>(qs.GetRawText())
            : new QueryResultSettings { Paging = new PagingInfo { Top = 50, Skip = 0 } };

        var url = $"{_baseUrl}{_commerceApiPath}/Search";
        
        var requestBody = new
        {
            productSearchCriteria = searchCriteria,
            queryResultSettings = querySettings
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/Search", response);
    }

    /// <summary>
    /// Gets a SimpleProduct by its record identifier.
    /// Maps to: GET /Commerce/Products/GetById(recordId,channelId)
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> GetProductByIdAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/GetById operation");
        
        var recordId = arguments.GetProperty("recordId").GetInt64();
        var channelId = arguments.GetProperty("channelId").GetInt64();

        var url = $"{_baseUrl}{_commerceApiPath}/GetById(recordId={recordId},channelId={channelId})";
        
        SetCommerceHeaders("Anonymous");
        
        var response = await _httpClient.GetAsync(url);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/GetById", response);
    }

    /// <summary>
    /// Gets a collection of products based on channel identifier and record identifiers.
    /// Maps to: POST /Commerce/Products/GetByIds
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> GetProductsByIdsAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/GetByIds operation");
        
        var channelId = arguments.GetProperty("channelId").GetInt64();
        var productIds = JsonSerializer.Deserialize<long[]>(
            arguments.GetProperty("productIds").GetRawText()) ?? Array.Empty<long>();
        
        var querySettings = arguments.TryGetProperty("queryResultSettings", out var qs) 
            ? JsonSerializer.Deserialize<QueryResultSettings>(qs.GetRawText())
            : new QueryResultSettings { Paging = new PagingInfo { Top = 50, Skip = 0 } };

        var url = $"{_baseUrl}{_commerceApiPath}/GetByIds";
        
        var requestBody = new
        {
            channelId,
            productIds,
            queryResultSettings = querySettings
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/GetByIds", response);
    }

    /// <summary>
    /// Retrieves a collection of SimpleProduct recommendations given a collection of product identifiers.
    /// Maps to: POST /Commerce/Products/GetRecommendedProducts
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> GetRecommendedProductsAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/GetRecommendedProducts operation");
        
        var productIds = JsonSerializer.Deserialize<long[]>(
            arguments.GetProperty("productIds").GetRawText()) ?? Array.Empty<long>();
        var customerAccountNumber = arguments.TryGetProperty("customerAccountNumber", out var can) 
            ? can.GetString() : null;
        
        var querySettings = arguments.TryGetProperty("queryResultSettings", out var qs) 
            ? JsonSerializer.Deserialize<QueryResultSettings>(qs.GetRawText())
            : new QueryResultSettings { Paging = new PagingInfo { Top = 50, Skip = 0 } };

        var url = $"{_baseUrl}{_commerceApiPath}/GetRecommendedProducts";
        
        var requestBody = new
        {
            productIds,
            customerAccountNumber,
            queryResultSettings = querySettings
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/GetRecommendedProducts", response);
    }

    /// <summary>
    /// Compares products for detailed analysis.
    /// Maps to: POST /Commerce/Products/Compare
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> CompareProductsAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/Compare operation");
        
        var channelId = arguments.GetProperty("channelId").GetInt64();
        var catalogId = arguments.GetProperty("catalogId").GetInt64();
        var productIds = JsonSerializer.Deserialize<long[]>(
            arguments.GetProperty("productIds").GetRawText()) ?? Array.Empty<long>();
        
        var querySettings = arguments.TryGetProperty("queryResultSettings", out var qs) 
            ? JsonSerializer.Deserialize<QueryResultSettings>(qs.GetRawText())
            : new QueryResultSettings { Paging = new PagingInfo { Top = 50, Skip = 0 } };

        var url = $"{_baseUrl}{_commerceApiPath}/Compare";
        
        var requestBody = new
        {
            channelId,
            catalogId,
            productIds,
            queryResultSettings = querySettings
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/Compare", response);
    }

    /// <summary>
    /// Searches for products that belong to a category directly or via its child categories.
    /// Maps to: POST /Commerce/Products/SearchByCategory
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> SearchProductsByCategoryAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/SearchByCategory operation");
        
        var channelId = arguments.GetProperty("channelId").GetInt64();
        var catalogId = arguments.GetProperty("catalogId").GetInt64();
        var categoryId = arguments.GetProperty("categoryId").GetInt64();
        
        var querySettings = arguments.TryGetProperty("queryResultSettings", out var qs) 
            ? JsonSerializer.Deserialize<QueryResultSettings>(qs.GetRawText())
            : new QueryResultSettings { Paging = new PagingInfo { Top = 50, Skip = 0 } };

        var url = $"{_baseUrl}{_commerceApiPath}/SearchByCategory";
        
        var requestBody = new
        {
            channelId,
            catalogId,
            categoryId,
            queryResultSettings = querySettings
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/SearchByCategory", response);
    }

    /// <summary>
    /// Searches for products that are associated to the given search text.
    /// Maps to: POST /Commerce/Products/SearchByText
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> SearchProductsByTextAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/SearchByText operation");
        
        var channelId = arguments.GetProperty("channelId").GetInt64();
        var catalogId = arguments.GetProperty("catalogId").GetInt64();
        var searchText = arguments.GetProperty("searchText").GetString() ?? string.Empty;
        
        var querySettings = arguments.TryGetProperty("queryResultSettings", out var qs) 
            ? JsonSerializer.Deserialize<QueryResultSettings>(qs.GetRawText())
            : new QueryResultSettings { Paging = new PagingInfo { Top = 50, Skip = 0 } };

        var url = $"{_baseUrl}{_commerceApiPath}/SearchByText";
        
        var requestBody = new
        {
            channelId,
            catalogId,
            searchText,
            queryResultSettings = querySettings
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/SearchByText", response);
    }

    /// <summary>
    /// Gets recommended search phrases based on a (partial) search text.
    /// Maps to: POST /Commerce/Products/GetSearchSuggestions
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> GetSearchSuggestionsAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/GetSearchSuggestions operation");
        
        var channelId = arguments.GetProperty("channelId").GetInt64();
        var catalogId = arguments.GetProperty("catalogId").GetInt64();
        var searchText = arguments.GetProperty("searchText").GetString() ?? string.Empty;
        var hitPrefix = arguments.TryGetProperty("hitPrefix", out var hp) ? hp.GetString() : string.Empty;
        var hitSuffix = arguments.TryGetProperty("hitSuffix", out var hs) ? hs.GetString() : string.Empty;
        
        var querySettings = arguments.TryGetProperty("queryResultSettings", out var qs) 
            ? JsonSerializer.Deserialize<QueryResultSettings>(qs.GetRawText())
            : new QueryResultSettings { Paging = new PagingInfo { Top = 10, Skip = 0 } };

        var url = $"{_baseUrl}{_commerceApiPath}/GetSearchSuggestions";
        
        var requestBody = new
        {
            channelId,
            catalogId,
            searchText,
            hitPrefix,
            hitSuffix,
            queryResultSettings = querySettings
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/GetSearchSuggestions", response);
    }

    /// <summary>
    /// Gets the product refiner(s) available for the given category product(s).
    /// Maps to: POST /Commerce/Products/GetRefinersByCategory
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> GetRefinersByCategoryAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/GetRefinersByCategory operation");
        
        var catalogId = arguments.GetProperty("catalogId").GetInt64();
        var categoryId = arguments.GetProperty("categoryId").GetInt64();
        
        var querySettings = arguments.TryGetProperty("queryResultSettings", out var qs) 
            ? JsonSerializer.Deserialize<QueryResultSettings>(qs.GetRawText())
            : new QueryResultSettings { Paging = new PagingInfo { Top = 50, Skip = 0 } };

        var url = $"{_baseUrl}{_commerceApiPath}/GetRefinersByCategory";
        
        var requestBody = new
        {
            catalogId,
            categoryId,
            queryResultSettings = querySettings
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/GetRefinersByCategory", response);
    }

    /// <summary>
    /// Gets the product refiner(s) available for product(s) resulting from searching the given text.
    /// Maps to: POST /Commerce/Products/GetRefinersByText
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> GetRefinersByTextAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/GetRefinersByText operation");
        
        var catalogId = arguments.GetProperty("catalogId").GetInt64();
        var searchText = arguments.GetProperty("searchText").GetString() ?? string.Empty;
        
        var querySettings = arguments.TryGetProperty("queryResultSettings", out var qs) 
            ? JsonSerializer.Deserialize<QueryResultSettings>(qs.GetRawText())
            : new QueryResultSettings { Paging = new PagingInfo { Top = 50, Skip = 0 } };

        var url = $"{_baseUrl}{_commerceApiPath}/GetRefinersByText";
        
        var requestBody = new
        {
            catalogId,
            searchText,
            queryResultSettings = querySettings
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/GetRefinersByText", response);
    }

    /// <summary>
    /// Gets the dimension values for a product based on the specified requirements.
    /// Maps to: POST /Commerce/Products/GetDimensionValues
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> GetDimensionValuesAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/GetDimensionValues operation");
        
        var recordId = arguments.GetProperty("recordId").GetInt64();
        var channelId = arguments.GetProperty("channelId").GetInt64();
        var dimension = arguments.GetProperty("dimension").GetInt32();
        var matchingDimensionValues = arguments.TryGetProperty("matchingDimensionValues", out var mdv) 
            ? JsonSerializer.Deserialize<object[]>(mdv.GetRawText()) ?? Array.Empty<object>()
            : Array.Empty<object>();
        
        var querySettings = arguments.TryGetProperty("queryResultSettings", out var qs) 
            ? JsonSerializer.Deserialize<QueryResultSettings>(qs.GetRawText())
            : new QueryResultSettings { Paging = new PagingInfo { Top = 50, Skip = 0 } };

        var url = $"{_baseUrl}{_commerceApiPath}/GetDimensionValues";
        
        var requestBody = new
        {
            recordId,
            channelId,
            dimension,
            matchingDimensionValues,
            queryResultSettings = querySettings
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/GetDimensionValues", response);
    }

    /// <summary>
    /// Gets the variations of a product based on the specified requirements.
    /// Maps to: POST /Commerce/Products/GetVariantsByDimensionValues
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> GetVariantsByDimensionValuesAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/GetVariantsByDimensionValues operation");
        
        var recordId = arguments.GetProperty("recordId").GetInt64();
        var channelId = arguments.GetProperty("channelId").GetInt64();
        var matchingDimensionValues = arguments.TryGetProperty("matchingDimensionValues", out var mdv) 
            ? JsonSerializer.Deserialize<object[]>(mdv.GetRawText()) ?? Array.Empty<object>()
            : Array.Empty<object>();
        
        var querySettings = arguments.TryGetProperty("queryResultSettings", out var qs) 
            ? JsonSerializer.Deserialize<QueryResultSettings>(qs.GetRawText())
            : new QueryResultSettings { Paging = new PagingInfo { Top = 50, Skip = 0 } };

        var url = $"{_baseUrl}{_commerceApiPath}/GetVariantsByDimensionValues";
        
        var requestBody = new
        {
            recordId,
            channelId,
            matchingDimensionValues,
            queryResultSettings = querySettings
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/GetVariantsByDimensionValues", response);
    }

    /// <summary>
    /// Gets the attribute values of the specified product.
    /// Maps to: POST /Commerce/Products/GetAttributeValues
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> GetAttributeValuesAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/GetAttributeValues operation");
        
        var recordId = arguments.GetProperty("recordId").GetInt64();
        var channelId = arguments.GetProperty("channelId").GetInt64();
        var catalogId = arguments.GetProperty("catalogId").GetInt64();
        
        var querySettings = arguments.TryGetProperty("queryResultSettings", out var qs) 
            ? JsonSerializer.Deserialize<QueryResultSettings>(qs.GetRawText())
            : new QueryResultSettings { Paging = new PagingInfo { Top = 50, Skip = 0 } };

        var url = $"{_baseUrl}{_commerceApiPath}/GetAttributeValues";
        
        var requestBody = new
        {
            recordId,
            channelId,
            catalogId,
            queryResultSettings = querySettings
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/GetAttributeValues", response);
    }

    /// <summary>
    /// Gets the price of a product in context of the current customer.
    /// Maps to: GET /Commerce/Products/GetPrice
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> GetProductPriceAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/GetPrice operation");
        
        var recordId = arguments.GetProperty("recordId").GetInt64();
        var customerAccountNumber = arguments.TryGetProperty("customerAccountNumber", out var can) 
            ? can.GetString() : null;
        var unitOfMeasureSymbol = arguments.TryGetProperty("unitOfMeasureSymbol", out var uom) 
            ? uom.GetString() : null;

        var url = $"{_baseUrl}{_commerceApiPath}/GetPrice";
        
        var requestBody = new
        {
            recordId,
            customerAccountNumber,
            unitOfMeasureSymbol
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/GetPrice", response);
    }

    /// <summary>
    /// Get available inventory for given list of items for given channel and customer.
    /// Maps to: POST /Commerce/Products/GetProductAvailabilities
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> GetProductAvailabilitiesAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/GetProductAvailabilities operation");
        
        var itemIds = JsonSerializer.Deserialize<long[]>(
            arguments.GetProperty("itemIds").GetRawText()) ?? Array.Empty<long>();
        var channelId = arguments.GetProperty("channelId").GetInt64();
        
        var querySettings = arguments.TryGetProperty("queryResultSettings", out var qs) 
            ? JsonSerializer.Deserialize<QueryResultSettings>(qs.GetRawText())
            : new QueryResultSettings { Paging = new PagingInfo { Top = 50, Skip = 0 } };

        var url = $"{_baseUrl}{_commerceApiPath}/GetProductAvailabilities";
        
        var requestBody = new
        {
            itemIds,
            channelId,
            queryResultSettings = querySettings
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/GetProductAvailabilities", response);
    }

    /// <summary>
    /// Gets the media locations for the specified product.
    /// Maps to: POST /Commerce/Products/GetMediaLocations
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> GetMediaLocationsAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/GetMediaLocations operation");
        
        var recordId = arguments.GetProperty("recordId").GetInt64();
        var channelId = arguments.GetProperty("channelId").GetInt64();
        var catalogId = arguments.GetProperty("catalogId").GetInt64();
        
        var querySettings = arguments.TryGetProperty("queryResultSettings", out var qs) 
            ? JsonSerializer.Deserialize<QueryResultSettings>(qs.GetRawText())
            : new QueryResultSettings { Paging = new PagingInfo { Top = 50, Skip = 0 } };

        var url = $"{_baseUrl}{_commerceApiPath}/GetMediaLocations";
        
        var requestBody = new
        {
            recordId,
            channelId,
            catalogId,
            queryResultSettings = querySettings
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/GetMediaLocations", response);
    }

    /// <summary>
    /// Gets the unit(s) of measure for the specified product.
    /// Maps to: POST /Commerce/Products/GetUnitsOfMeasure
    /// Roles: Employee, Customer, Anonymous, Application
    /// </summary>
    public async Task<object> GetUnitsOfMeasureAsync(JsonElement arguments)
    {
        _logger.LogInformation("Executing Products/GetUnitsOfMeasure operation");
        
        var recordId = arguments.GetProperty("recordId").GetInt64();
        
        var querySettings = arguments.TryGetProperty("queryResultSettings", out var qs) 
            ? JsonSerializer.Deserialize<QueryResultSettings>(qs.GetRawText())
            : new QueryResultSettings { Paging = new PagingInfo { Top = 50, Skip = 0 } };

        var url = $"{_baseUrl}{_commerceApiPath}/GetUnitsOfMeasure";
        
        var requestBody = new
        {
            recordId,
            queryResultSettings = querySettings
        };

        SetCommerceHeaders("Anonymous");

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content) ?? new { error = "No data returned" };
        }
        
        return await HandleErrorResponse("Products/GetUnitsOfMeasure", response);
    }

    /// <summary>
    /// Centralized error handling for HTTP responses.
    /// Logs appropriate error messages and returns structured error information.
    /// </summary>
    private async Task<object> HandleErrorResponse(string operation, HttpResponseMessage response)
    {
        _logger.LogError("{Operation} operation failed: {StatusCode} - {ReasonPhrase}", 
            operation, response.StatusCode, response.ReasonPhrase);
        
        var errorContent = await response.Content.ReadAsStringAsync();
        
        var errorResponse = new
        {
            error = true,
            operation,
            statusCode = (int)response.StatusCode,
            reasonPhrase = response.ReasonPhrase,
            details = errorContent,
            timestamp = DateTime.UtcNow
        };

        // For client errors (4xx), return the error object instead of throwing
        if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
        {
            return errorResponse;
        }

        // For server errors (5xx), throw an exception
        throw new HttpRequestException($"ProductService {operation} failed: {response.StatusCode} - {response.ReasonPhrase}. Details: {errorContent}");
    }
}