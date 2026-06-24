// Khedmetak.BLL/Services/Implementation/StatisticsService.cs
using Khedmetak.BLL.DTOS.Statistics;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Microsoft.AspNetCore.Identity;

namespace Khedmetak.BLL.Services.Implementation
{
    public class StatisticsService : IStatisticsService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public StatisticsService(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<StatisticsDto> GetStatisticsAsync()
        {
            var totalUsers = _userManager.Users.Count();
            var totalServices = (await _unitOfWork.GovServices.GetAllWithCategoryAsync()).Count();
            var totalCategories = (await _unitOfWork.Categories.GetAllWithServicesCountAsync()).Count();

            return new StatisticsDto
            {
                TotalUsers = totalUsers,
                TotalServices = totalServices,
                TotalCategories = totalCategories,
            };
        }
    }
}