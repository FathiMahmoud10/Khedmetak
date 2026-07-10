// Khedmetak.DAL/Database/AppDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Khedmetak.Core.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseSqlServer(
                //"Server=db57211.public.databaseasp.net;Database=db57211;User Id=db57211;Password=dE#37-fK%N5n;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
                "Server=db59305.public.databaseasp.net; Database=db59305; User Id=db59305; Password=Z!y4=n2PoM?9; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True; "
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}