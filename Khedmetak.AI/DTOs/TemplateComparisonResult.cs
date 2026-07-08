namespace Khedmetak.AI.DTOs;

public class TemplateComparisonResult
{
    public string DetectedDocumentType { get; set; } = "";

    public bool MatchesExpectedType { get; set; }

    public bool MatchesTemplate { get; set; }

    public double Confidence { get; set; }

    public string Summary { get; set; } = "";
}
