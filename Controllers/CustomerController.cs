using McpCommerceServer.Models;
using McpCommerceServer.Mcp;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace McpCommerceServer.Controllers
{
    public class CustomerController
    {
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Searches for customers based on the provided search criteria
        /// </summary>
        /// <param name="customerSearchCriteria">The search criteria for finding customers</param>
        /// <param name="queryResultSettings">Paging and sorting settings for the results</param>
        /// <returns>PageResult containing matching GlobalCustomer records</returns>
        public Task<McpToolCallResult> CustomerSearchAsync(
            CustomerSearchCriteria customerSearchCriteria, 
            QueryResultSettings? queryResultSettings = null)
        {
            try
            {
                _logger.LogInformation("CustomerSearch called with criteria: {Criteria}", 
                    JsonSerializer.Serialize(customerSearchCriteria));

                // TODO: Implement actual customer search logic
                // This would typically involve:
                // 1. Validating the search criteria
                // 2. Calling D365 Commerce API or database
                // 3. Applying paging and sorting
                // 4. Returning the results

                var result = new PageResult<GlobalCustomer>
                {
                    Results = new List<GlobalCustomer>(),
                    TotalCount = 0,
                    HasNextPage = false,
                    HasPreviousPage = false
                };

                var response = new McpToolCallResult
                {
                    Content = new List<McpContent>
                    {
                        new McpContent
                        {
                            Type = "text",
                            Text = JsonSerializer.Serialize(result, new JsonSerializerOptions 
                            { 
                                WriteIndented = true 
                            })
                        }
                    },
                    IsError = false
                };

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching for customers");
                
                return Task.FromResult(new McpToolCallResult
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
                });
            }
        }

        /// <summary>
        /// Gets the tool definition for customer_search
        /// </summary>
        /// <returns>MCP tool definition for customer_search</returns>
        public McpToolDefinition GetCustomerSearchToolDefinition()
        {
            return new McpToolDefinition
            {
                Name = "customer_search",
                Title = "Customer Search",
                Description = "Search for customers in D365 Commerce based on search criteria. Supports filtering by account number, email, phone, or general search text with paging and sorting options.",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        customerSearchCriteria = new
                        {
                            type = "object",
                            description = "Search criteria for finding customers",
                            properties = new
                            {
                                searchText = new { type = "string", description = "General search text to find customers" },
                                accountNumber = new { type = "string", description = "Customer account number" },
                                email = new { type = "string", description = "Customer email address" },
                                phone = new { type = "string", description = "Customer phone number" },
                                extensionProperties = new
                                {
                                    type = "array",
                                    description = "Additional search properties",
                                    items = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            key = new { type = "string", description = "Property key" },
                                            value = new { type = "object", description = "Property value" }
                                        },
                                        required = new[] { "key", "value" }
                                    }
                                }
                            }
                        },
                        queryResultSettings = new
                        {
                            type = "object",
                            description = "Paging and sorting settings for search results",
                            properties = new
                            {
                                paging = new
                                {
                                    type = "object",
                                    description = "Pagination settings",
                                    properties = new
                                    {
                                        skip = new { type = "integer", minimum = 0, description = "Number of results to skip (default: 0)" },
                                        top = new { type = "integer", minimum = 1, maximum = 1000, description = "Number of results to return (default: 50, max: 1000)" }
                                    }
                                },
                                sorting = new
                                {
                                    type = "object",
                                    description = "Sorting settings",
                                    properties = new
                                    {
                                        field = new { type = "string", description = "Field to sort by (e.g., 'FullName', 'AccountNumber', 'Email')" },
                                        isDescending = new { type = "boolean", description = "Sort in descending order (default: false)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "customerSearchCriteria" }
                }
            };
        }
    }
}
