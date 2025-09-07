namespace IndexIQ.API.Utilities
{
    public static class StopWords
    {
        private static readonly HashSet<string> _stopWords = new HashSet<string>
        {
            "a", "an", "and", "are", "as", "at", "be",
            "but", "by", "for", "if", "in", "into", "is",
            "it", "no", "not", "of", "on", "or", "such",
            "that", "the", "their", "then", "there", "these",
            "they", "this", "to", "was", "will", "with"
        };

        public static bool IsStopWord(string word) => _stopWords.Contains(word);
    }
}
