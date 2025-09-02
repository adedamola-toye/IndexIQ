namespace IndexIQ.API.Models
{
    public class Document
{
    public required string id;
    public required string FileName { get; set; }
    public required string ContentText { get; set; }
    public required Dictionary<string, int> TermFrequency { get; set; }
    public DateTime UploadedTimestamp { get; set; }
}
}
