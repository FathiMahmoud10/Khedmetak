using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Khedmetak.AI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Abstraction
{
    public interface IEmbeddingService
    {
       

        public  Task<float[]> GenerateEmbeddingAsync(string text);


    }
}
