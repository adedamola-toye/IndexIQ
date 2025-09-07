using IndexIQ.API.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

public interface IDocumentProcessingService
{
    Task<Document> ProcessSingleFileAsync(IFormFile file);
    Task<List<Document>> ProcesssBatchFilesAsync(List<IFormFile> files);
}