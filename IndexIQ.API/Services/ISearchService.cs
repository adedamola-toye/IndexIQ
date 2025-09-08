using IndexIQ.API.Models;

namespace IndexIQ.API.Services
{
    public interface ISearchService
    {
        Task<List<QueryResult>> SearchAsync(SearchQuery query);
    }
}
