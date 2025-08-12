using System.Text.Json.Serialization;

namespace McpCommerceServer.Models;

public class Product
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("sku")]
    public string Sku { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("inStock")]
    public bool InStock { get; set; }

    [JsonPropertyName("stockQuantity")]
    public int StockQuantity { get; set; }
}

public class Customer
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("loyaltyLevel")]
    public string LoyaltyLevel { get; set; } = string.Empty;
}

public class Order
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("customerId")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("orderDate")]
    public DateTime OrderDate { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    [JsonPropertyName("items")]
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    [JsonPropertyName("productId")]
    public string ProductId { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }
}

public class StoreInventory
{
    [JsonPropertyName("storeId")]
    public string StoreId { get; set; } = string.Empty;

    [JsonPropertyName("productId")]
    public string ProductId { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("lastUpdated")]
    public DateTime LastUpdated { get; set; }
}

public class SimpleProduct
{
    [JsonPropertyName("recordId")]
    public long RecordId { get; set; }

    [JsonPropertyName("itemId")]
    public string ItemId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("basePrice")]
    public decimal BasePrice { get; set; }

    [JsonPropertyName("isKit")]
    public bool IsKit { get; set; }

    [JsonPropertyName("masterProductId")]
    public long MasterProductId { get; set; }

    [JsonPropertyName("productTypeValue")]
    public int ProductTypeValue { get; set; }
}

public class ProductSearchCriteria
{
    [JsonPropertyName("channelId")]
    public long ChannelId { get; set; }

    [JsonPropertyName("catalogId")]
    public long CatalogId { get; set; }

    [JsonPropertyName("categoryId")]
    public long CategoryId { get; set; }

    [JsonPropertyName("searchCondition")]
    public string SearchCondition { get; set; } = string.Empty;

    [JsonPropertyName("searchText")]
    public string SearchText { get; set; } = string.Empty;

    [JsonPropertyName("refinement")]
    public ProductRefiner? Refinement { get; set; }

    [JsonPropertyName("sorting")]
    public ProductSearchSorting? Sorting { get; set; }
}

public class ProductRefiner
{
    [JsonPropertyName("refinementCriteria")]
    public List<ProductRefinerValue> RefinementCriteria { get; set; } = new();
}

public class ProductRefinerValue
{
    [JsonPropertyName("refinerRecordId")]
    public long RefinerRecordId { get; set; }

    [JsonPropertyName("refinerSourceValue")]
    public string RefinerSourceValue { get; set; } = string.Empty;

    [JsonPropertyName("swatchColorHexCode")]
    public string SwatchColorHexCode { get; set; } = string.Empty;

    [JsonPropertyName("swatchImageUrl")]
    public string SwatchImageUrl { get; set; } = string.Empty;
}

public class ProductSearchSorting
{
    [JsonPropertyName("sortingKey")]
    public int SortingKey { get; set; }

    [JsonPropertyName("isDescending")]
    public bool IsDescending { get; set; }
}

public class QueryResultSettings
{
    [JsonPropertyName("paging")]
    public PagingInfo? Paging { get; set; }

    [JsonPropertyName("sorting")]
    public SortingInfo? Sorting { get; set; }
}

public class PagingInfo
{
    [JsonPropertyName("top")]
    public int Top { get; set; } = 50;

    [JsonPropertyName("skip")]
    public int Skip { get; set; } = 0;
}

public class SortingInfo
{
    [JsonPropertyName("isDescending")]
    public bool IsDescending { get; set; }

    [JsonPropertyName("sortingKey")]
    public string SortingKey { get; set; } = string.Empty;
}

public class RecommendationCriteria
{
    [JsonPropertyName("customerAccountNumber")]
    public string? CustomerAccountNumber { get; set; }

    [JsonPropertyName("catalogId")]
    public long CatalogId { get; set; }

    [JsonPropertyName("productIds")]
    public List<long> ProductIds { get; set; } = new();

    [JsonPropertyName("categoryIds")]
    public List<long> CategoryIds { get; set; } = new();
}

public class SearchFilter
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("filterTypeValue")]
    public int FilterTypeValue { get; set; }

    [JsonPropertyName("searchValues")]
    public List<SearchFilterValue> SearchValues { get; set; } = new();
}

public class SearchFilterValue
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("displayText")]
    public string DisplayText { get; set; } = string.Empty;
}

public class SearchCriteria
{
    [JsonPropertyName("channelId")]
    public long ChannelId { get; set; }

    [JsonPropertyName("catalogId")]
    public long CatalogId { get; set; }

    [JsonPropertyName("categoryId")]
    public long CategoryId { get; set; }

    [JsonPropertyName("searchCondition")]
    public string SearchCondition { get; set; } = string.Empty;

    [JsonPropertyName("searchText")]
    public string SearchText { get; set; } = string.Empty;

    [JsonPropertyName("searchFilters")]
    public List<SearchFilter> SearchFilters { get; set; } = new();

    [JsonPropertyName("includeProductsFromSubcategories")]
    public bool IncludeProductsFromSubcategories { get; set; }

    [JsonPropertyName("skipVariantExpansion")]
    public bool SkipVariantExpansion { get; set; }
}