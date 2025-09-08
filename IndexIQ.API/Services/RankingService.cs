using System;
using System.Collections.Generic;
using System.Linq;
using IndexIQ.API.Models;
using IndexIQ.API.Services;

public class RankingService : IRankingService
{
    // Smoothing constants/decisions: we use idf = log((N + 1) / (df + 1)) + 1
    // This prevents division by zero and gives non-zero weights for rare terms.
    public List<QueryResult> Rank(
        IReadOnlyList<string> queryTokens,
        IEnumerable<Document> candidateDocuments,
        IDictionary<string, Dictionary<string,int>> invertedIndex)
    {
        if (queryTokens == null || queryTokens.Count == 0)
            return new List<QueryResult>();

        var docsList = candidateDocuments.ToList();
        int totalDocs = Math.Max(1, docsList.Count);

        // Precomputing  df and idf for the query terms 
        var idf = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        foreach (var term in queryTokens.Distinct())
        {
            if (invertedIndex.TryGetValue(term, out var posting))
                idf[term] = Math.Log((double)(totalDocs + 1) / (posting.Count + 1)) + 1.0;
            else
                idf[term] = Math.Log((double)(totalDocs + 1) / 1.0) + 1.0; // term not seen in corpus
        }

        // Build query vector (tf for query terms)
        var qtf = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        foreach (var t in queryTokens)
            qtf[t] = qtf.GetValueOrDefault(t) + 1.0;
        // convert query tf -> weight = tf * idf (we don't normalize query tf here, but we will in cosine)
        var qWeights = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in qtf)
        {
            var term = kv.Key;
            var tf = kv.Value;
            qWeights[term] = tf * idf[term];
        }
        // compute query vector norm
        double qNorm = Math.Sqrt(qWeights.Values.Sum(w => w * w));
        if (qNorm == 0) qNorm = 1e-9;

        var results = new List<QueryResult>(docsList.Count);

        foreach (var doc in docsList)
        {
            // compute dot product between query vector and doc vector for only query terms
            double dot = 0.0;
            double docSqSum = 0.0;

            // precompute doc total terms (for TF normalization)
            double docTotalTerms = doc.TermFrequency?.Values.Sum() ?? 0;
            if (docTotalTerms <= 0)
            {
                
                results.Add(new QueryResult(doc.Id, doc.FileName, 0.0));
                continue;
            }

            foreach (var term in qWeights.Keys)
            {
                // doc term frequency
                doc.TermFrequency.TryGetValue(term, out var rawTf);
                double tfDoc = rawTf / docTotalTerms; // normalized tf
                double weightDoc = tfDoc * idf[term];  // tf-idf weight for this term in doc
                double weightQuery = qWeights[term];   // query weight (tf_query * idf)

                dot += weightDoc * weightQuery;
                docSqSum += weightDoc * weightDoc;
            }

            double docNorm = Math.Sqrt(docSqSum);
            if (docNorm == 0)
            {
                results.Add(new QueryResult(doc.Id, doc.FileName,0.0));
                continue;
            }

            double cosine = dot / (docNorm * qNorm);
            results.Add(new QueryResult(doc.Id, doc.FileName,cosine));
        }

        // sort descending
        return results.OrderByDescending(r => r.Score).ToList();
    }
}
