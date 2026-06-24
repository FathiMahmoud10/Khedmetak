using Khedmetak.BLL.DTOS.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.Services.Abstraction
{
    public interface IStatisticsService
    {
        Task<StatisticsDto> GetStatisticsAsync();

    }
}
