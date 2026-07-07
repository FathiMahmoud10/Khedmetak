namespace Khedmetak.AI.DTOs.ImageRag
{
    public class SearchImageResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? FileName { get; set; }
        public double? SimilarityScore { get; set; }
    }
}
