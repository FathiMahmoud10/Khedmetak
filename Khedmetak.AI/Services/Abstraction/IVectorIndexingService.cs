using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Abstraction
{
    public interface IVectorIndexingService
    {
        public Task IndexServiceAsync(int serviceId);

    }
}
