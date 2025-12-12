using HandlebarsDotNet;
using Microsoft.Playwright;
using PlaywrightPdfGenerator.Logic.Interfaces;
using PlaywrightPdfGenerator.Logic.Models;
using System.Reflection;

namespace PlaywrightPdfGenerator.Logic.Services;
public class PdfGenerationService : IPdfGenerationService
{
    public async Task<PdfGenerationResult> GenerateAsync<T>(T data, Templates template, string language = "nl") where T : class
    {
        var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var templatePath = Path.Combine(assemblyLocation, "Templates", $"{language}", $"{template.ToTemplateString()}.hbs");
        var templateContent = await File.ReadAllTextAsync(templatePath);

        var hbsTemplate = Handlebars.Compile(templateContent);

        var html = hbsTemplate(data);

        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        var page = await browser.NewPageAsync();

        await page.SetContentAsync(html);

        var pdfData = await page.PdfAsync(new PagePdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
        });

        await browser.CloseAsync();

        return new() { PdfBytes = pdfData };
    }
}
