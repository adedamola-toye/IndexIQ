using IndexIQ.API.Models;

namespace IndexIQ.API.Services
{
    public interface IIndexerService
    {

        //Index a single document
        void IndexDocument(Document document);

        //Index multiple documents at once
        void IndexDocuments(IEnumerable<Document> documents);

        //Retrieve the inverted index
        Dictionary<string, Dictionary<string, int>> GetIndex();

        //Clear index for testing purposes
        void ClearIndex();
    }
}