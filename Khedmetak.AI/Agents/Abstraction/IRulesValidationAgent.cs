using System.Collections.Generic;
using System.Threading.Tasks;
using Khedmetak.AI.DTOs;

namespace Khedmetak.AI.Agents.Abstraction;

public interface IRulesValidationAgent
{
    Task<RuleValidationResult> ValidateRulesAsync(OCRResult ocrResult, List<string> rules);
}
