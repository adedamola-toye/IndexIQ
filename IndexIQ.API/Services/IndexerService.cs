using System.Text.RegularExpressions;
using IndexIQ.API.Models;
using IndexIQ.API.Utilities;
using System.Collections.Generic;
using Porter2Stemmer;

namespace IndexIQ.API.Services
{
    public class IndexerService : IIndexerService
    {
        private readonly Dictionary<string, Dictionary<string, int>> _invertedIndex;

        //stemmer instance
        private readonly EnglishPorter2Stemmer _stemmer = new EnglishPorter2Stemmer();


        public IndexerService()
        {
            _invertedIndex = new Dictionary<string, Dictionary<string, int>>();
        }

        public void IndexDocument(Document doc)
        {
            // Tokenize
            var tokens = Regex.Split(doc.ContentText.ToLower(), @"\W+");
            doc.TermFrequency = new Dictionary<string, int>();


            foreach (var token in tokens)
            {
                if (string.IsNullOrWhiteSpace(token)) continue;

                if (StopWords.IsStopWord(token)) continue; // ignore stop words
                var stemmedToken = _stemmer.Stem(token);
                string stemmedTokenKey = stemmedToken.Value;

                if (!_invertedIndex.ContainsKey(stemmedTokenKey))
                {
                    _invertedIndex[stemmedTokenKey] = new Dictionary<string, int>();
                }

                if (!_invertedIndex[stemmedTokenKey].ContainsKey(doc.Id))
                {
                    _invertedIndex[stemmedTokenKey][doc.Id] = 0;
                }

                _invertedIndex[stemmedTokenKey][doc.Id]++;

                if (!doc.TermFrequency.ContainsKey(stemmedTokenKey))
                {
                    doc.TermFrequency[stemmedTokenKey] = 0;
                }
                doc.TermFrequency[stemmedTokenKey]++;
            }
        }

        public void IndexDocuments(IEnumerable<Document> documents)
        {
            foreach (var doc in documents)
            {
                IndexDocument(doc);
            }
        }

        public Dictionary<string, Dictionary<string, int>> GetIndex()
        {
            return _invertedIndex;
        }

        public void ClearIndex()
        {
            _invertedIndex.Clear();
        }

        
    }
}
