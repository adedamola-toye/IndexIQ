namespace IndexIQ.API.Models
{
    public class QueryResult
    {
        public required string DocumentId { get; set; }
        public string? FileName { get; set; }
        public required double Score { get; set; } //Relevance score
        public string? Snippet { get; set; } //A snippet of the content where the query matched
    }
}