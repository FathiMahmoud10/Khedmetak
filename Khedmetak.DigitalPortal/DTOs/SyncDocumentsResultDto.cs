using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DigitalPortal.DTOs
{
    public class SyncDocumentsResultDto
    {
        public bool Success { get; set; }
        public int SyncedCount { get; set; }
        public string Message { get; set; } = string.Empty;
    }

}
