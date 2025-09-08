using IndexIQ.API.Models;
using IndexIQ.API.Utilities;
using System.Text.RegularExpressions;

namespace IndexIQ.API.Services
{
    public class SearchService : ISearchService
    {
        private readonly IIndexerService _indexerService;
        private readonly IRankingService _rankingService;
        private readonly List<Document> _documents; // store indexed docs

        public SearchService(IIndexerService indexerService, IRankingService rankingService)
        {
            _indexerService = indexerService;
            _rankingService = rankingService;
            _documents = new List<Document>();
        }

        public async Task<List<QueryResult>> SearchAsync(SearchQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.QueryText))
                return new List<QueryResult>();

            // 1. Normalize + tokenize query
            var tokens = Tokenize(query.QueryText);
            query.Tokens = tokens;

            // 2. Get inverted index
            var invertedIndex =  _indexerService.GetIndex();

            // 3. Candidate docs = filter by tokens (all docs that have any query token)
            var candidateDocs = _documents.Where(d =>
                d.TermFrequency.Keys.Any(t => tokens.Contains(t))).ToList();

            if (!candidateDocs.Any())
                return new List<QueryResult>();

            // 4. Rank using TF-IDF cosine similarity
            var results = _rankingService.Rank(tokens, candidateDocs, invertedIndex);

            return results;
        }

        private List<string> Tokenize(string input)
        {
            var tokens = Regex.Split(input.ToLowerInvariant(), @"\W+")
                              .Where(t => !string.IsNullOrWhiteSpace(t) && !StopWords.IsStopWord(t))
                              .ToList();
            return tokens;
        }

        // Extra: allow adding docs (will be used in UploadController)
        public void AddDocuments(IEnumerable<Document> docs)
        {
            _documents.AddRange(docs);
            _indexerService.IndexDocuments(docs);
        }
    }
}
