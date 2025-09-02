
//The input the user gives. It's what is parsed before running the search
namespace IndexIQ.API.Models
{
    public class SearchQuery
    {
        public required string QueryText { get; set; } //raw input
        public List<string>? Tokens { get; set; } //Break down the query into tokens
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}