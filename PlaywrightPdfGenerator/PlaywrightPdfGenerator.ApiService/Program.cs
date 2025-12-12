using PlaywrightPdfGenerator.Logic.Extensions;
using PlaywrightPdfGenerator.Logic.Interfaces;
using PlaywrightPdfGenerator.Logic.Models;
using PlaywrightPdfGenerator.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddPdfGenerationServices();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("templates/invoice", async (IPdfGenerationService pdfGenerationService, string language = "nl") =>
{
    var invoiceData = new
    {
        InvoiceNumber = "INV-2025-001234",
        InvoiceDate = "December 12, 2025",
        DueDate = "January 11, 2026",
        PolicyNumber = "POL-789456123",
        CustomerName = "Customer Lorem",
        CustomerAddress = "LoremStreet 123",
        CustomerCity = "Brussels",
        CustomerZip = "1000",
        CustomerCountry = "Belgium",
        CustomerEmail = "customer.one@example.com",
        Items = new[]
        {
            new { Description = "Home Insurance Premium - Q1 2025", Quantity = 1, UnitPrice = "285.00", Amount = "285.00" },
            new { Description = "Auto Insurance Premium - Q1 2025", Quantity = 1, UnitPrice = "425.50", Amount = "425.50" },
            new { Description = "Life Insurance Premium - Q1 2025", Quantity = 1, UnitPrice = "150.00", Amount = "150.00" }
        },
        Subtotal = "860.50",
        TaxRate = "21",
        TaxAmount = "180.71",
        Total = "1,041.21",
        Notes = "Payment due within 30 days. Please reference invoice number when making payment."
    };

    var pdfGenerationResult = await pdfGenerationService.GenerateAsync(invoiceData, Templates.Invoice, language);

    return Results.File(pdfGenerationResult.PdfBytes, "application/pdf", "invoice.pdf");
})
.WithName("GetInvoiceTemplate")
.WithSummary("Generate invoice PDF template")
.WithDescription("Generates a test invoice PDF with sample data. The language can be: nl, fr, en");

app.MapDefaultEndpoints();

app.Run();
