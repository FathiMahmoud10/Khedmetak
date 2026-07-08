using System.Collections.Generic;

namespace Khedmetak.AI.DTOs;

public class RuleValidationResult
{
    public List<RuleResult> Results { get; set; } = [];
}

public class RuleResult
{
    public string Rule { get; set; } = "";

    public bool Passed { get; set; }

    public string Note { get; set; } = "";
}
