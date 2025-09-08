namespace IndexIQ.API.Models
{
    public class QueryResult
    {
        public string DocumentId { get; set; }
        public string? FileName { get; set; }
        public double Score { get; set; } //Relevance score to know which document matches the query most. 
        public string? Snippet { get; set; } //A snippet of the content where the query matched

        public QueryResult(string documentId, string? fileName, double score, string? snippet = null)
        {
            DocumentId = documentId;
            FileName = fileName;
            Score = score;
            Snippet = snippet;
        }
    }
}