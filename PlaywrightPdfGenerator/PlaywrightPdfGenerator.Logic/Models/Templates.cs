namespace PlaywrightPdfGenerator.Logic.Models;

public enum Templates
{
    Invoice
}

public static class TemplatesExtensions
{
    public static string ToTemplateString(this Templates template) => template switch
    {
        Templates.Invoice => "Invoice",
        _ => throw new ArgumentOutOfRangeException(nameof(template))
    };
}
 