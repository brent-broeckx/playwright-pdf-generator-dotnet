namespace PlaywrightPdfGenerator.Logic.Models;
public class PdfGenerationResult
{
    public byte[] PdfBytes { get; set; }
    public const string ContentType = "application/pdf";
}
