namespace IndexIQ.API.Models
{
    public class Document
    {
        public string Id{ get; set; }
        public string FileName { get; set; }
        public string ContentText { get; set; }
        public Dictionary<string, int> TermFrequency { get; set; }
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

