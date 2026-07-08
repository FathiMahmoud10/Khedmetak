using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.DTOS.StandardDocument
{
    // Khedmetak.DAL/DTOs/StandardDocument/CreateStandardDocumentDto.cs
    namespace Khedmetak.DAL.DTOs.StandardDocument
    {
        public class CreateStandardDocumentDto
        {
            public string DocumentName { get; set; } = string.Empty;
            public IFormFile? StandardDocumentFile { get; set; }
            public string? GeneralRule { get; set; }
        }
    }
}
