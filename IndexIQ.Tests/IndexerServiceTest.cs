using Xunit;
using IndexIQ.API.Models;
using IndexIQ.API.Services;
public class IndexerTests
{
    [Fact]
    public static void Indexer_Should_Index_Single_Document()
    {
        //Arrange
        Document doc1 = new Document("1", "myDoc", "coding program coding");
        IIndexerService indexer = new IndexerService();

        //Act
        indexer.IndexDocument(doc1);

        //Assert
        Assert.Equal(2, indexer.GetIndex()["coding"]["1"]);
        Assert.Equal(1, indexer.GetIndex()["program"]["1"]);
    }
}