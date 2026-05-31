
using Khedmetak.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Khedmetak.Core.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
 
    #region Dbset

    public DbSet<GovernmentService> GovernmentServices { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<ChatSession> ChatSessions { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}