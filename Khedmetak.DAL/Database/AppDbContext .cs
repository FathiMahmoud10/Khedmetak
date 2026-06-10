using Khedmetak.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Khedmetak.Core.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    #region DbSets
    public DbSet<ChatSession> ChatSessions { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<GovService> GovServices { get; set; }
    public DbSet<ServiceSteps> ServiceSteps { get; set; }
    public DbSet<ServiceGeneralDocs> ServiceGeneralDocs { get; set; }
    public DbSet<RequiredDocument> RequiredDocuments { get; set; }
    public DbSet<UserDocument> UserDocuments { get; set; }
    public DbSet<ServiceOption> ServiceOptions { get; set; }
    public DbSet<ServiceOptionChoices> ServiceOptionChoices { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.Property(u => u.Name).IsRequired().HasMaxLength(100);
            e.Property(u => u.Role).IsRequired().HasMaxLength(50);
        });

        #region SeedData
 

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "الأحوال المدنية" },
            new Category { Id = 2, Name = "المرور" },
            new Category { Id = 3, Name = "التعليم" },
            new Category { Id = 4, Name = "الصحة" },
            new Category { Id = 5, Name = "التموين" }
        );


        modelBuilder.Entity<GovService>().HasData(
            new GovService
            {
                Id = 1,
                SrvName = "استخراج بطاقة رقم قومي",
                SrvDesc = "إصدار بطاقة رقم قومي لأول مرة",
                SrvFees = 50,
                EstimatedFees = 50,
                SrvTime = "7 أيام",
                CategoryId = 1
            },
            new GovService
            {
                Id = 2,
                SrvName = "تجديد بطاقة رقم قومي",
                SrvDesc = "تجديد بطاقة الرقم القومي المنتهية",
                SrvFees = 50,
                EstimatedFees = 50,
                SrvTime = "3 أيام",
                CategoryId = 1
            },
            new GovService
            {
                Id = 3,
                SrvName = "تجديد رخصة سيارة",
                SrvDesc = "تجديد رخصة المركبة",
                SrvFees = 500,
                EstimatedFees = 500,
                SrvTime = "يوم واحد",
                CategoryId = 2
            },
            new GovService
            {
                Id = 4,
                SrvName = "استخراج بدل فاقد شهادة ميلاد",
                SrvDesc = "إصدار شهادة ميلاد بدل فاقد",
                SrvFees = 30,
                EstimatedFees = 30,
                SrvTime = "فوري",
                CategoryId = 1
            }
        );
        #endregion


        // ChatSession
        modelBuilder.Entity<ChatSession>(e =>
        {
            e.HasOne(cs => cs.User)
                .WithMany(u => u.ChatSessions)
                .HasForeignKey(cs => cs.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(cs => cs.Category)
                .WithMany(c => c.ChatSessions)
                .HasForeignKey(cs => cs.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ChatMessage
        modelBuilder.Entity<ChatMessage>(e =>
        {
            e.Property(m => m.Content).IsRequired();
            e.Property(m => m.Role).IsRequired().HasMaxLength(20);

            e.HasOne(m => m.ChatSession)
                .WithMany(cs => cs.ChatMessages)
                .HasForeignKey(m => m.ChatSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Feedback — علاقة 1:1 مع ChatSession
        modelBuilder.Entity<Feedback>(e =>
        {
            e.HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(f => f.ChatSession)
                .WithOne(cs => cs.Feedback)
                .HasForeignKey<Feedback>(f => f.ChatSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Category
        modelBuilder.Entity<Category>(e =>
        {
            e.Property(c => c.Name).IsRequired().HasMaxLength(100);
        });

        // GovService
        modelBuilder.Entity<GovService>(e =>
        {
            e.Property(g => g.SrvName).IsRequired().HasMaxLength(200);
            e.Property(g => g.SrvFees).HasColumnType("decimal(18,2)");
            e.Property(g => g.EstimatedFees).HasColumnType("decimal(18,2)");

            e.HasOne(g => g.Category)
                .WithMany(c => c.GovServices)
                .HasForeignKey(g => g.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ServiceSteps
        modelBuilder.Entity<ServiceSteps>(e =>
        {
            e.HasOne(s => s.GovService)
                .WithMany(g => g.ServiceSteps)
                .HasForeignKey(s => s.GovServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ServiceGeneralDocs
        modelBuilder.Entity<ServiceGeneralDocs>(e =>
        {
            e.HasOne(d => d.GovService)
                .WithMany(g => g.ServiceGeneralDocs)
                .HasForeignKey(d => d.GovServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RequiredDocument
        modelBuilder.Entity<RequiredDocument>(e =>
        {
            e.HasOne(r => r.GovService)
                .WithMany(g => g.RequiredDocuments)
                .HasForeignKey(r => r.GovServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserDocument
        modelBuilder.Entity<UserDocument>(e =>
        {
            e.Property(d => d.ValidationStatus).HasMaxLength(50);

            e.HasOne(d => d.User)
                .WithMany(u => u.UserDocuments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(d => d.ChatSession)
                .WithMany(cs => cs.UserDocuments)
                .HasForeignKey(d => d.ChatSessionId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(d => d.RequiredDocument)
                .WithMany(r => r.UserDocuments)
                .HasForeignKey(d => d.RequiredDocumentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ServiceOption
        modelBuilder.Entity<ServiceOption>(e =>
        {
            e.HasOne(o => o.GovService)
                .WithMany(g => g.ServiceOptions)
                .HasForeignKey(o => o.GovServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ServiceOptionChoices
        modelBuilder.Entity<ServiceOptionChoices>(e =>
        {
            e.HasOne(c => c.ServiceOption)
                .WithMany(o => o.ServiceOptionChoices)
                .HasForeignKey(c => c.ServiceOptionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}