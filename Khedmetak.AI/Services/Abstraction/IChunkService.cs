using Khedmetak.AI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Abstraction
{
    public interface IChunkService
    {

        public Task<List<ServiceChunkDTO>> GenerateChunksAsync(int serviceId);
        public  Task<ServiceChunkDTO> GenerateServiceChunkAsync(int serviceId);




    }
}
