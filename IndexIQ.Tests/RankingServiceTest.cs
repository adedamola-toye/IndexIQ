using Xunit;
using System.Collections.Generic;
using System.Linq;
using IndexIQ.API.Models;
using IndexIQ.API.Services;

public class RankingServiceTests
{
    private static Dictionary<string, Dictionary<string,int>> BuildInvertedIndex(IEnumerable<Document> docs)
    {
        var idx = new Dictionary<string, Dictionary<string,int>>(StringComparer.OrdinalIgnoreCase);
        foreach (var d in docs)
        {
            foreach (var kv in d.TermFrequency)
            {
                if (!idx.TryGetValue(kv.Key, out var posting)) idx[kv.Key] = posting = new Dictionary<string,int>();
                posting[d.Id] = kv.Value;
            }
        }
        return idx;
    }

    [Fact]
    public void SingleTerm_HigherTf_RanksHigher()
    {
        // docA has higher 'coding' TF than docB
        var docA = new Document("A", "a.txt", "") { TermFrequency = new Dictionary<string,int>{{"coding", 5}} };
        var docB = new Document("B", "b.txt", "") { TermFrequency = new Dictionary<string,int>{{"coding", 1}} };

        var docs = new List<Document>{ docA, docB };
        var inverted = BuildInvertedIndex(docs);
        var ranking = new RankingService();

        var results = ranking.Rank(new List<string>{"coding"}, docs, inverted);

        Assert.Equal("A", results.First().DocumentId);
    }

    [Fact]
    public void MultiTerm_DocWithBothTerms_RanksAboveSingleTermDoc()
    {
        // doc1 contains both 'coding' and 'program' once each
        var doc1 = new Document("1", "doc1", "") { TermFrequency = new Dictionary<string,int>{{"coding",1},{"program",1}} };
        // doc2 contains 'coding' twice only
        var doc2 = new Document("2", "doc2", "") { TermFrequency = new Dictionary<string,int>{{"coding",2}} };

        var docs = new List<Document>{ doc1, doc2 };
        var inverted = BuildInvertedIndex(docs);
        var ranking = new RankingService();

        var results = ranking.Rank(new List<string>{"coding","program"}, docs, inverted);

        // We expect doc1 (which matches both query terms) to score higher than doc2
        Assert.True(results.First().DocumentId == "1" || results.First().DocumentId == "2");
        // assert doc1 score strictly greater than doc2
        var first = results.First(r => r.DocumentId == "1").Score;
        var second = results.First(r => r.DocumentId == "2").Score;
        Assert.True(first > second, $"Expected doc1 score > doc2 score but got {first} <= {second}");
    }
}
