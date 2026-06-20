using Khedmetak.AI.DTOs;
using Khedmetak.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Abstraction
{
    public interface IQdrantService
    {
        public  Task UpsertServiceChunksAsync(
           GovService service,
           List<ServiceChunkDTO> chunks,
           Func<string, Task<float[]>> embedFunc);
    }
}
