using Microsoft.Extensions.DependencyInjection;
using PlaywrightPdfGenerator.Logic.Interfaces;
using PlaywrightPdfGenerator.Logic.Services;

namespace PlaywrightPdfGenerator.Logic.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPdfGenerationServices(this IServiceCollection services)
    {
        services.AddSingleton<IPdfGenerationService, PdfGenerationService>();

        return services;
    }
}
