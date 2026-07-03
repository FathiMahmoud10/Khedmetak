using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Repo
{
    // Interface
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment?> GetByMerchantRefNumAsync(string merchantRefNum);
        Task UpdateAsync(Payment payment);
    }

    // Implementation
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _db;
        public PaymentRepository(AppDbContext db) => _db = db;

        public async Task<Payment> CreateAsync(Payment payment)
        {
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetByMerchantRefNumAsync(string merchantRefNum)
            => await _db.Payments.FirstOrDefaultAsync(p => p.MerchantRefNum == merchantRefNum);

        public async Task UpdateAsync(Payment payment)
        {
            _db.Payments.Update(payment);
            await _db.SaveChangesAsync();
        }
    }
}
