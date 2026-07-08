using System.Collections.Generic;

namespace Khedmetak.AI.DTOs;

public class OCRResult
{
    public bool Readable { get; set; }

    public Dictionary<string, string> Fields { get; set; } = [];

    public List<string> MissingFields { get; set; } = [];

    public double Confidence { get; set; }
}
