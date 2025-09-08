using IndexIQ.API.Models;
public interface IRankingService
{

    List<QueryResult> Rank(
        IReadOnlyList<string> queryTokens,
        IEnumerable<Document> candidateDocuments,
        IDictionary<string, Dictionary<string, int>> invertedIndex);
}
