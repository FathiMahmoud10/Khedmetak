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
        // Seed Roles
        var roleId = 1;
        modelBuilder.Entity<IdentityRole<int>>().HasData(
            new IdentityRole<int> { Id = roleId, Name = "User", NormalizedName = "USER" }
        );

        // Seed Users
        var hasher = new PasswordHasher<User>();

        var fathi = new User
        {
            Id = 1,
            UserName = "Fathi",
            NormalizedUserName = "FATHI",
            Email = "fathi@khedmetak.com",
            NormalizedEmail = "FATHI@KHEDMETAK.COM",
            EmailConfirmed = true,
            Name = "Fathi",
            Role = "User",
            Password = "12345678",
            SecurityStamp = "f4fb76b8-2ea9-42b7-876a-39fbcf9e6cf4",
            ConcurrencyStamp = "e7492cfa-e160-49b8-a6d1-817abcf992bf"
        };
        fathi.PasswordHash = hasher.HashPassword(fathi, "12345678");

        var aya = new User
        {
            Id = 2,
            UserName = "Aya",
            NormalizedUserName = "AYA",
            Email = "aya@khedmetak.com",
            NormalizedEmail = "AYA@KHEDMETAK.COM",
            EmailConfirmed = true,
            Name = "Aya",
            Role = "User",
            Password = "12345678",
            SecurityStamp = "bc521d96-c167-4277-a859-00ef1295beea",
            ConcurrencyStamp = "df768913-9118-4a9f-a496-e26bbbc23eef"
        };
        aya.PasswordHash = hasher.HashPassword(aya, "12345678");

        var naglaa = new User
        {
            Id = 3,
            UserName = "Naglaa",
            NormalizedUserName = "NAGLAA",
            Email = "naglaa@khedmetak.com",
            NormalizedEmail = "NAGLAA@KHEDMETAK.COM",
            EmailConfirmed = true,
            Name = "Naglaa",
            Role = "User",
            Password = "12345678",
            SecurityStamp = "cbe62da6-dbdb-4fbc-bdf8-18e388ffc811",
            ConcurrencyStamp = "b1f5fe6b-67a4-44b7-bdc6-2c93d9fb34d0"
        };
        naglaa.PasswordHash = hasher.HashPassword(naglaa, "12345678");

        var rahma = new User
        {
            Id = 4,
            UserName = "Rahma",
            NormalizedUserName = "RAHMA",
            Email = "rahma@khedmetak.com",
            NormalizedEmail = "RAHMA@KHEDMETAK.COM",
            EmailConfirmed = true,
            Name = "Rahma",
            Role = "User",
            Password = "12345678",
            SecurityStamp = "d7d91e6b-e53b-4861-a53d-82c5f1fa6d03",
            ConcurrencyStamp = "5c5fbef1-cb69-42b7-99e2-348f6cfef7e9"
        };
        rahma.PasswordHash = hasher.HashPassword(rahma, "12345678");

        modelBuilder.Entity<User>().HasData(fathi, aya, naglaa, rahma);

        // Seed User Roles
        modelBuilder.Entity<IdentityUserRole<int>>().HasData(
            new IdentityUserRole<int> { UserId = 1, RoleId = roleId },
            new IdentityUserRole<int> { UserId = 2, RoleId = roleId },
            new IdentityUserRole<int> { UserId = 3, RoleId = roleId },
            new IdentityUserRole<int> { UserId = 4, RoleId = roleId }
        );

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

            e.HasOne(cs => cs.GovService)
                .WithMany()
                .HasForeignKey(cs => cs.GovServiceId)
                .OnDelete(DeleteBehavior.SetNull);

            e.Property(cs => cs.Status)
                .HasConversion<int>()
                .HasDefaultValue(Khedmetak.DAL.Enums.RequestStatus.Pending);
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