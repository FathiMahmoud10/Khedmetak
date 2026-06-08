
using Khedmetak.Core.Entities;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Entities.Base;
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
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Category> Categories { get; set; }

    // الخدمات الحكومية
    public DbSet<GovService> GovServices { get; set; }
    public DbSet<Servicestep> ServiceSteps { get; set; }
    public DbSet<RequiredDocument> RequiredDocuments { get; set; }
    public DbSet<KnowledgeBase> KnowledgeBases { get; set; }

    // المستندات
    public DbSet<UserDocument> UserDocuments { get; set; }
    #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GovService>()
            .HasKey(g => g.SrvId);

        modelBuilder.Entity<RequiredDocument>()
            .HasKey(d => d.DocumentId);

        modelBuilder.Entity<Feedback>()
            .HasKey(f => f.FeedBackId);

        modelBuilder.Entity<ChatSession>()
            .HasMany(s => s.Categories)
            .WithMany(c => c.ChatSessions)
            .UsingEntity("ChatSessionCategory");

        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.ChatSession)
            .WithOne(s => s.Feedback)
            .HasForeignKey<Feedback>(f => f.ChatSessionId);
    }
}
