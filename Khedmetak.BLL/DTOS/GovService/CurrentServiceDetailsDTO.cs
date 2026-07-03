using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.DTOS.GovService
{
    public class CurrentServiceDetailsDTO
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = "لم تحدد بعد";
        public string? CategoryName { get; set; } = "----";
        public int? RequiredDocumentsCount { get; set; } = 0;
        public decimal? Fees { get; set; } = 0;
        public string? TakenTime { get; set; } = "0";
    }
}
