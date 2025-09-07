using System.Text.RegularExpressions;
using IndexIQ.API.Models;
using IndexIQ.API.Utilities;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Globalization;
using NPOI.HWPF;        // for .doc
using NPOI.HWPF.Extractor;
using NPOI.XWPF.UserModel;
using System.Text;  // for .docx

public class DocumentProcessingService : IDocumentProcessingService
{

    public async Task<Document> ProcessSingleFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty");
        }
        string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        string fileContent = fileExtension switch
        {
            ".txt" or ".csv" => await ExtractTextAsync(file),
            ".html" => await ExtractHtmlAsync(file),
            ".doc" or ".docx" => await ExtractWordAsync(file),
            ".xlsx" or "xls" => await ExtractExcelAsync(file),
            ".pptx" or ".ppt" => await ExtractPowerpointAsync(file),
            _ => throw new NotSupportedException($"File type {fileExtension} is not supported.")
        };
        string autoID = Guid.NewGuid().ToString();
        return new IndexIQ.API.Models.Document(autoID, file.FileName, fileContent);
    }

    public async Task<List<Document>> ProcesssBatchFilesAsync(List<IFormFile> files)
    {
        var documents = new List<Document>();
        foreach (var file in files)
        {
            var doc = await ProcessSingleFileAsync(file);
            documents.Add(doc);
        }
        return documents;
    }

    public async Task<string> ExtractTextAsync(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        string content = await reader.ReadToEndAsync();
        return content;
    }



    public async Task<string> ExtractHtmlAsync(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        string html = await reader.ReadToEndAsync();

        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        //Only <body> text for now
        var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
        string bodyText = bodyNode?.InnerText ?? string.Empty;

        return bodyText.Trim();
    }

    public async Task<string> ExtractWordAsync(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        using var doc = new XWPFDocument(memoryStream);

        var sb = new StringBuilder();
        foreach (var p in doc.Paragraphs)
        {
            sb.AppendLine(p.ParagraphText);
        }

        foreach (var table in doc.Tables)
        {
            foreach (var row in table.Rows)
            {
                foreach (var cell in row.GetTableCells())
                {
                    sb.Append(cell.GetText());
                    sb.Append("\t");
                }
                sb.AppendLine();
            }
        }
        return sb.ToString().Trim();
    }
    public async Task<string> ExtractExcelAsync(IFormFile file)
    {

    }
    public async Task<string> ExtractPowerpointAsync(IFormFile file)
    {

    }
}