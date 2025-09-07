namespace IndexIQ.API.Models
{
    public class Document
    {
        public required string Id{ get; set; }
        public required string FileName { get; set; }
        public required string ContentText { get; set; }
        public required Dictionary<string, int> TermFrequency { get; set; }
        public DateTime UploadedTimestamp { get; set; }

        public Document(string id, string fileName, string contentText)
        {
            Id = id;
            FileName = fileName;
            ContentText = contentText;
            TermFrequency = new Dictionary<string, int>();
            UploadedTimestamp = DateTime.Now;
        }
    }
}

