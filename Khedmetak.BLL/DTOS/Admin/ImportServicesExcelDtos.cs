using System.Collections.Generic;

namespace Khedmetak.BLL.DTOS.Admin
{

    public class ImportServicesResultDto
    {
        public int TotalRowsRead { get; set; }
        public int RowsProcessed { get; set; }
        public int ServicesCreated { get; set; }
        public int ServicesUpdated { get; set; }
        public int StepsCreated { get; set; }
        public int DocumentsCreated { get; set; }
        public int CategoriesCreated { get; set; }

        public List<ImportRowErrorDto> Errors { get; set; } = new();
    }

    public class ImportRowErrorDto
    {
        public int RowNumber { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}