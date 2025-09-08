using Xunit;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using IndexIQ.API.Models;
using IndexIQ.API.Services;

namespace IndexIQ.Tests
{
    public class DocumentProcessingServiceTests
    {
        public async Task ProcessFileAsync_Should_Process_Text_File()
        {
            var content = "Hello world, this is a test docuemnt for txt files";
            var fileName = "test.txt";
            var file = CreateFakeFormFile(content, fileName);

            IDocumentProcessingService docService = new DocumentProcessingService();

            //Act
            Document result = await docService.ProcessSingleFileAsync(file);

            Assert.NotNull(result);
            Assert.Equal("test.txt", result.FileName);
            Assert.Equal("Hello world, this is a test docuemnt for txt files", result.ContentText);

        }

        // Helper method to create a fake file upload in memory
        private IFormFile CreateFakeFormFile(string content, string fileName)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return new FormFile(stream, 0, stream.Length, "file", fileName);
        }
    }
}
