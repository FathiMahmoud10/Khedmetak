namespace Khedmetak.AI.DTOs;

public class TemplateComparisonResult
{
    public string DetectedDocumentType { get; set; } = "";

    public bool MatchesExpectedType { get; set; }

    public bool MatchesTemplate { get; set; }

    public double Confidence { get; set; }

    public string Summary { get; set; } = "";

    public bool ComparisonPerformed { get; set; }

    public bool? ComparisonMatch { get; set; }

    public double ComparisonConfidence { get; set; }

    public bool IsImageValid { get; set; } = true;

    public List<string> ImageProblems { get; set; } = [];

    public string? InvalidReason { get; set; }
}
