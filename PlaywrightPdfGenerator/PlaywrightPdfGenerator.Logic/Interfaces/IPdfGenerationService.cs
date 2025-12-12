using PlaywrightPdfGenerator.Logic.Models;

namespace PlaywrightPdfGenerator.Logic.Interfaces;
public interface IPdfGenerationService
{
    Task<PdfGenerationResult> GenerateAsync<T>(T data, Templates template, string shortLanguage = "D") where T : class;
}
