using System.ComponentModel.DataAnnotations;

namespace McpCommerceServer.Models
{
    public class QueryResultSettings
    {
        public PagingInfo? Paging { get; set; }
        public SortingInfo? Sorting { get; set; }
    }

    public class PagingInfo
    {
        public int Skip { get; set; } = 0;
        public int Top { get; set; } = 50;
    }

    public class SortingInfo
    {
        public string? Field { get; set; }
        public bool IsDescending { get; set; } = false;
    }

    public class ClientBookCustomerSearchCriteria
    {
        public bool FilterByCurrentEmployee { get; set; }
        public List<ClientBookRefinerValue>? Refinement { get; set; }
        public List<CommerceProperty>? ExtensionProperties { get; set; }
    }

    public class ClientBookRefinerValue
    {
        public string? RefinerKey { get; set; }
        public string? RefinerValue { get; set; }
    }

    public class CommerceProperty
    {
        public string? Key { get; set; }
        public object? Value { get; set; }
    }

    public class GlobalCustomer
    {
        public string? PartyNumber { get; set; }
        public long RecordId { get; set; }
        public bool IsAsyncCustomer { get; set; }
        public string? AccountNumber { get; set; }
        public string? FullName { get; set; }
        public string? FullAddress { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int CustomerTypeValue { get; set; }
        public List<MediaLocation>? Images { get; set; }
        public string? OfflineImage { get; set; }
        public bool IsB2b { get; set; }
        public List<CommerceProperty>? ExtensionProperties { get; set; }
    }

    public class MediaLocation
    {
        public string? Uri { get; set; }
        public string? AltText { get; set; }
    }

    public class PageResult<T>
    {
        public List<T> Results { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class CustomerSearchCriteria
    {
        public string? SearchText { get; set; }
        public string? AccountNumber { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public List<CommerceProperty>? ExtensionProperties { get; set; }
    }
}
