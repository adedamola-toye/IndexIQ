public class Document{
    public  string id;
    public string FileName {get; set;};
    public string ContentText {get; set;};
    public Dictionary<string, int> TermFrequency {get; set;};
    public DateTime UploadedTimestamp {get; set;};
}