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
        Assert.Equal(2, doc1.TermFrequency["coding"]);
        Assert.Equal(1, doc1.TermFrequency["program"]);
    }

    [Fact]
    public void Indexer_Should_Index_Multiple_Documents()
    {
        // Arrange
        Document doc1 = new Document("1", "doc1", "coding program coding");
        Document doc2 = new Document("2", "doc2", "program testing coding");
        IIndexerService indexer = new IndexerService();

        // Act
        indexer.IndexDocuments(new List<Document> { doc1, doc2 });

        // Assert
        var index = indexer.GetIndex();
        Assert.Equal(2, index["coding"]["1"]);  // doc1 coding count
        Assert.Equal(1, index["coding"]["2"]);  // doc2 coding count
        Assert.Equal(1, index["program"]["1"]);
        Assert.Equal(1, index["program"]["2"]);
        Assert.Equal(1, index["test"]["2"]); // "testing" stemmed to "test"
    }

    [Fact]
    public void Indexer_Should_Ignore_StopWords()
    {
        // Arrange
        Document doc = new Document("1", "doc", "the and of in coding");
        IIndexerService indexer = new IndexerService();

        // Act
        indexer.IndexDocument(doc);

        // Assert
        var index = indexer.GetIndex();
        Assert.False(index.ContainsKey("the"));
        Assert.False(index.ContainsKey("and"));
        Assert.False(index.ContainsKey("of"));
        Assert.False(index.ContainsKey("in"));
        Assert.True(index.ContainsKey("cod")); // "coding" stemmed to "cod"
    }

    [Fact]
    public void Indexer_Should_Clear_Index()
    {
        // Arrange
        Document doc = new Document("1", "doc", "coding program");
        IIndexerService indexer = new IndexerService();
        indexer.IndexDocument(doc);

        // Act
        indexer.ClearIndex();

        // Assert
        Assert.Empty(indexer.GetIndex());
    }

    [Fact]
    public void Indexer_Should_Update_TermFrequency_Per_Document()
    {
        // Arrange
        Document doc = new Document("1", "doc", "coding coding coding program");
        IIndexerService indexer = new IndexerService();

        // Act
        indexer.IndexDocument(doc);

        // Assert
        Assert.Equal(3, doc.TermFrequency["cod"]);  // "coding" stemmed form
        Assert.Equal(1, doc.TermFrequency["program"]);
    }
}