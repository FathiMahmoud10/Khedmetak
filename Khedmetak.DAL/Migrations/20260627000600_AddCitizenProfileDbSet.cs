using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCitizenProfileDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetRoles')
                BEGIN
                    CREATE TABLE [AspNetRoles] (
                        [Id] int NOT NULL IDENTITY,
                        [Name] nvarchar(256) NULL,
                        [NormalizedName] nvarchar(256) NULL,
                        [ConcurrencyStamp] nvarchar(max) NULL,
                        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUsers')
                BEGIN
                    CREATE TABLE [AspNetUsers] (
                        [Id] int NOT NULL IDENTITY,
                        [Name] nvarchar(100) NOT NULL,
                        [Email] nvarchar(256) NOT NULL,
                        [Password] nvarchar(max) NOT NULL,
                        [Role] nvarchar(50) NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UserName] nvarchar(256) NULL,
                        [NormalizedUserName] nvarchar(256) NULL,
                        [NormalizedEmail] nvarchar(256) NULL,
                        [EmailConfirmed] bit NOT NULL,
                        [PasswordHash] nvarchar(max) NULL,
                        [SecurityStamp] nvarchar(max) NULL,
                        [ConcurrencyStamp] nvarchar(max) NULL,
                        [PhoneNumber] nvarchar(max) NULL,
                        [PhoneNumberConfirmed] bit NOT NULL,
                        [TwoFactorEnabled] bit NOT NULL,
                        [LockoutEnd] datetimeoffset NULL,
                        [LockoutEnabled] bit NOT NULL,
                        [AccessFailedCount] int NOT NULL,
                        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categories')
                BEGIN
                    CREATE TABLE [Categories] (
                        [Id] int NOT NULL IDENTITY,
                        [Name] nvarchar(100) NOT NULL,
                        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetRoleClaims')
                BEGIN
                    CREATE TABLE [AspNetRoleClaims] (
                        [Id] int NOT NULL IDENTITY,
                        [RoleId] int NOT NULL,
                        [ClaimType] nvarchar(max) NULL,
                        [ClaimValue] nvarchar(max) NULL,
                        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserClaims')
                BEGIN
                    CREATE TABLE [AspNetUserClaims] (
                        [Id] int NOT NULL IDENTITY,
                        [UserId] int NOT NULL,
                        [ClaimType] nvarchar(max) NULL,
                        [ClaimValue] nvarchar(max) NULL,
                        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserLogins')
                BEGIN
                    CREATE TABLE [AspNetUserLogins] (
                        [LoginProvider] nvarchar(450) NOT NULL,
                        [ProviderKey] nvarchar(450) NOT NULL,
                        [ProviderDisplayName] nvarchar(max) NULL,
                        [UserId] int NOT NULL,
                        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
                        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserRoles')
                BEGIN
                    CREATE TABLE [AspNetUserRoles] (
                        [UserId] int NOT NULL,
                        [RoleId] int NOT NULL,
                        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
                        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserTokens')
                BEGIN
                    CREATE TABLE [AspNetUserTokens] (
                        [UserId] int NOT NULL,
                        [LoginProvider] nvarchar(450) NOT NULL,
                        [Name] nvarchar(450) NOT NULL,
                        [Value] nvarchar(max) NULL,
                        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
                        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CitizenProfiles')
                BEGIN
                    CREATE TABLE [CitizenProfiles] (
                        [Id] int NOT NULL IDENTITY,
                        [FullName] nvarchar(200) NOT NULL,
                        [DateOfBirth] datetime2 NOT NULL,
                        [City] nvarchar(100) NOT NULL,
                        [District] nvarchar(100) NOT NULL,
                        [Street] nvarchar(200) NOT NULL,
                        [BuildingNumber] nvarchar(50) NOT NULL,
                        [FloorNumber] nvarchar(50) NOT NULL,
                        [ApartmentNumber] nvarchar(50) NOT NULL,
                        [PostalCode] nvarchar(20) NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UserId] int NOT NULL,
                        CONSTRAINT [PK_CitizenProfiles] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_CitizenProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
                    );
                    CREATE UNIQUE INDEX [IX_CitizenProfiles_UserId] ON [CitizenProfiles] ([UserId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'GovServices')
                BEGIN
                    CREATE TABLE [GovServices] (
                        [Id] int NOT NULL IDENTITY,
                        [SrvName] nvarchar(200) NOT NULL,
                        [SrvDesc] nvarchar(max) NOT NULL,
                        [SrvFees] decimal(18,2) NOT NULL,
                        [SrvTime] nvarchar(max) NOT NULL,
                        [EstimatedFees] decimal(18,2) NOT NULL,
                        [CategoryId] int NOT NULL,
                        CONSTRAINT [PK_GovServices] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_GovServices_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ChatSessions')
                BEGIN
                    CREATE TABLE [ChatSessions] (
                        [Id] int NOT NULL IDENTITY,
                        [StartedAt] datetime2 NOT NULL,
                        [EndedAt] datetime2 NULL,
                        [SessionGuid] uniqueidentifier NOT NULL,
                        [Status] int NOT NULL DEFAULT 0,
                        [UserId] int NULL,
                        [CategoryId] int NULL,
                        [GovServiceId] int NULL,
                        CONSTRAINT [PK_ChatSessions] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_ChatSessions_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_ChatSessions_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE SET NULL,
                        CONSTRAINT [FK_ChatSessions_GovServices_GovServiceId] FOREIGN KEY ([GovServiceId]) REFERENCES [GovServices] ([Id]) ON DELETE SET NULL
                    );
                END
            ");
            // لو ChatSessions كان موجود بالفعل من الـ Init من غير الأعمدة الجديدة دي، نضيفهم يدوي
            migrationBuilder.Sql(@"
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CategoryId' AND object_id = OBJECT_ID(N'ChatSessions'))
        ALTER TABLE [ChatSessions] ADD [CategoryId] int NULL;
");

            migrationBuilder.Sql(@"
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'GovServiceId' AND object_id = OBJECT_ID(N'ChatSessions'))
        ALTER TABLE [ChatSessions] ADD [GovServiceId] int NULL;
");

            migrationBuilder.Sql(@"
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChatSessions_Categories_CategoryId')
        ALTER TABLE [ChatSessions] ADD CONSTRAINT [FK_ChatSessions_Categories_CategoryId]
            FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE SET NULL;
");

            migrationBuilder.Sql(@"
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChatSessions_GovServices_GovServiceId')
        ALTER TABLE [ChatSessions] ADD CONSTRAINT [FK_ChatSessions_GovServices_GovServiceId]
            FOREIGN KEY ([GovServiceId]) REFERENCES [GovServices] ([Id]) ON DELETE SET NULL;
");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RequiredDocuments')
                BEGIN
                    CREATE TABLE [RequiredDocuments] (
                        [Id] int NOT NULL IDENTITY,
                        [DocumentName] nvarchar(max) NOT NULL,
                        [IsMandatory] bit NOT NULL,
                        [DocumentType] int NOT NULL,
                        [GovServiceId] int NOT NULL,
                        CONSTRAINT [PK_RequiredDocuments] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_RequiredDocuments_GovServices_GovServiceId] FOREIGN KEY ([GovServiceId]) REFERENCES [GovServices] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ServiceGeneralDocs')
                BEGIN
                    CREATE TABLE [ServiceGeneralDocs] (
                        [Id] int NOT NULL IDENTITY,
                        [Title] nvarchar(max) NOT NULL,
                        [FilePath] nvarchar(max) NOT NULL,
                        [UploadedAt] datetime2 NOT NULL,
                        [LastUpdated] datetime2 NOT NULL,
                        [GovServiceId] int NOT NULL,
                        CONSTRAINT [PK_ServiceGeneralDocs] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_ServiceGeneralDocs_GovServices_GovServiceId] FOREIGN KEY ([GovServiceId]) REFERENCES [GovServices] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ServiceOptions')
                BEGIN
                    CREATE TABLE [ServiceOptions] (
                        [Id] int NOT NULL IDENTITY,
                        [Question] nvarchar(max) NOT NULL,
                        [GovServiceId] int NOT NULL,
                        CONSTRAINT [PK_ServiceOptions] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_ServiceOptions_GovServices_GovServiceId] FOREIGN KEY ([GovServiceId]) REFERENCES [GovServices] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ServiceSteps')
                BEGIN
                    CREATE TABLE [ServiceSteps] (
                        [Id] int NOT NULL IDENTITY,
                        [Title] nvarchar(max) NOT NULL,
                        [StepOrder] int NOT NULL,
                        [GovServiceId] int NOT NULL,
                        CONSTRAINT [PK_ServiceSteps] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_ServiceSteps_GovServices_GovServiceId] FOREIGN KEY ([GovServiceId]) REFERENCES [GovServices] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ChatMessages')
                BEGIN
                    CREATE TABLE [ChatMessages] (
                        [Id] int NOT NULL IDENTITY,
                        [Content] nvarchar(max) NOT NULL,
                        [Role] nvarchar(20) NOT NULL,
                        [SentAt] datetime2 NOT NULL,
                        [StartedAt] datetime2 NOT NULL,
                        [EndedAt] datetime2 NULL,
                        [ChatSessionId] int NOT NULL,
                        CONSTRAINT [PK_ChatMessages] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_ChatMessages_ChatSessions_ChatSessionId] FOREIGN KEY ([ChatSessionId]) REFERENCES [ChatSessions] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Feedbacks')
                BEGIN
                    CREATE TABLE [Feedbacks] (
                        [Id] int NOT NULL IDENTITY,
                        [Rating] int NOT NULL,
                        [Comments] nvarchar(max) NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UserId] int NOT NULL,
                        [ChatSessionId] int NOT NULL,
                        CONSTRAINT [PK_Feedbacks] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Feedbacks_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]),
                        CONSTRAINT [FK_Feedbacks_ChatSessions_ChatSessionId] FOREIGN KEY ([ChatSessionId]) REFERENCES [ChatSessions] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserDocuments')
                BEGIN
                    CREATE TABLE [UserDocuments] (
                        [Id] int NOT NULL IDENTITY,
                        [FileName] nvarchar(max) NOT NULL,
                        [FilePath] nvarchar(max) NOT NULL,
                        [FileType] nvarchar(max) NOT NULL,
                        [UploadedAt] datetime2 NOT NULL,
                        [ValidationStatus] nvarchar(50) NOT NULL,
                        [UserId] int NOT NULL,
                        [ChatSessionId] int NULL,
                        [RequiredDocumentId] int NULL,
                        CONSTRAINT [PK_UserDocuments] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_UserDocuments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]),
                        CONSTRAINT [FK_UserDocuments_ChatSessions_ChatSessionId] FOREIGN KEY ([ChatSessionId]) REFERENCES [ChatSessions] ([Id]),
                        CONSTRAINT [FK_UserDocuments_RequiredDocuments_RequiredDocumentId] FOREIGN KEY ([RequiredDocumentId]) REFERENCES [RequiredDocuments] ([Id]) ON DELETE SET NULL
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ServiceOptionChoices')
                BEGIN
                    CREATE TABLE [ServiceOptionChoices] (
                        [Id] int NOT NULL IDENTITY,
                        [Choice] nvarchar(max) NOT NULL,
                        [IsRequired] bit NOT NULL,
                        [ServiceOptionId] int NOT NULL,
                        CONSTRAINT [PK_ServiceOptionChoices] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_ServiceOptionChoices_ServiceOptions_ServiceOptionId] FOREIGN KEY ([ServiceOptionId]) REFERENCES [ServiceOptions] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            // Seed data - بس لو الجداول كانت فاضية (يعني الميجريشن دي أول مرة تتطبق فعليًا)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM [AspNetRoles] WHERE [Id] = 1)
                BEGIN
                    INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
                    VALUES (1, NULL, N'User', N'USER');
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM [AspNetUsers] WHERE [Email] = 'fathi@khedmetak.com')
                BEGIN
                    INSERT INTO [AspNetUsers] ([Id], [AccessFailedCount], [ConcurrencyStamp], [CreatedAt], [Email], [EmailConfirmed], [LockoutEnabled], [LockoutEnd], [Name], [NormalizedEmail], [NormalizedUserName], [Password], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [Role], [SecurityStamp], [TwoFactorEnabled], [UserName])
                    VALUES
                    (1, 0, N'e7492cfa-e160-49b8-a6d1-817abcf992bf', '2026-06-27T00:05:58.292', N'fathi@khedmetak.com', 1, 0, NULL, N'Fathi', N'FATHI@KHEDMETAK.COM', N'FATHI', N'12345678', N'AQAAAAIAAYagAAAAEMunpPU4BX4nS+HPNrqrv1F/Ft3H2Mvl1MYnLjqYhYU/2uBWifqTo9TyGGi+f7sfyg==', NULL, 0, N'User', N'f4fb76b8-2ea9-42b7-876a-39fbcf9e6cf4', 0, N'Fathi'),
                    (2, 0, N'df768913-9118-4a9f-a496-e26bbbc23eef', '2026-06-27T00:05:58.489', N'aya@khedmetak.com', 1, 0, NULL, N'Aya', N'AYA@KHEDMETAK.COM', N'AYA', N'12345678', N'AQAAAAIAAYagAAAAED9MIhqwCMwRSTwgeDM+U6tWDeLLNUHiNamsoS8NRU6tinbnAcFeomKKWp760uUh0A==', NULL, 0, N'User', N'bc521d96-c167-4277-a859-00ef1295beea', 0, N'Aya'),
                    (3, 0, N'b1f5fe6b-67a4-44b7-bdc6-2c93d9fb34d0', '2026-06-27T00:05:58.658', N'naglaa@khedmetak.com', 1, 0, NULL, N'Naglaa', N'NAGLAA@KHEDMETAK.COM', N'NAGLAA', N'12345678', N'AQAAAAIAAYagAAAAEGPLnyUCW1/6733YLQ2Wk5p79nG/6/L0L1NCztEC4nAF2dz3z+7Q0Yw9NUYmVO6FoQ==', NULL, 0, N'User', N'cbe62da6-dbdb-4fbc-bdf8-18e388ffc811', 0, N'Naglaa'),
                    (4, 0, N'5c5fbef1-cb69-42b7-99e2-348f6cfef7e9', '2026-06-27T00:05:58.824', N'rahma@khedmetak.com', 1, 0, NULL, N'Rahma', N'RAHMA@KHEDMETAK.COM', N'RAHMA', N'12345678', N'AQAAAAIAAYagAAAAENPShhh3ZTiHvUGboaQg2y4RyHtkidjhFOpBofyvUGe4NeTYwn3iMMXxxHCVd7H8bw==', NULL, 0, N'User', N'd7d91e6b-e53b-4861-a53d-82c5f1fa6d03', 0, N'Rahma');
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM [Categories] WHERE [Id] = 1)
                BEGIN
                    INSERT INTO [Categories] ([Id], [Name])
                    VALUES
                    (1, N'الأحوال المدنية'),
                    (2, N'المرور'),
                    (3, N'التعليم'),
                    (4, N'الصحة'),
                    (5, N'التموين');
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM [AspNetUserRoles] WHERE [UserId] = 1 AND [RoleId] = 1)
                BEGIN
                    INSERT INTO [AspNetUserRoles] ([RoleId], [UserId])
                    VALUES
                    (1, 1),
                    (1, 2),
                    (1, 3),
                    (1, 4);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM [GovServices] WHERE [Id] = 1)
                BEGIN
                    INSERT INTO [GovServices] ([Id], [CategoryId], [EstimatedFees], [SrvDesc], [SrvFees], [SrvName], [SrvTime])
                    VALUES
                    (1, 1, 50, N'إصدار بطاقة رقم قومي لأول مرة', 50, N'استخراج بطاقة رقم قومي', N'7 أيام'),
                    (2, 1, 50, N'تجديد بطاقة الرقم القومي المنتهية', 50, N'تجديد بطاقة رقم قومي', N'3 أيام'),
                    (3, 2, 500, N'تجديد رخصة المركبة', 500, N'تجديد رخصة سيارة', N'يوم واحد'),
                    (4, 1, 30, N'إصدار شهادة ميلاد بدل فاقد', 30, N'استخراج بدل فاقد شهادة ميلاد', N'فوري');
                END
            ");

            // Indexes - كل واحد بشرط IF NOT EXISTS
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetRoleClaims_RoleId' AND object_id = OBJECT_ID('AspNetRoleClaims'))
                    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'RoleNameIndex' AND object_id = OBJECT_ID('AspNetRoles'))
                    CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUserClaims_UserId' AND object_id = OBJECT_ID('AspNetUserClaims'))
                    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUserLogins_UserId' AND object_id = OBJECT_ID('AspNetUserLogins'))
                    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUserRoles_RoleId' AND object_id = OBJECT_ID('AspNetUserRoles'))
                    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'EmailIndex' AND object_id = OBJECT_ID('AspNetUsers'))
                    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UserNameIndex' AND object_id = OBJECT_ID('AspNetUsers'))
                    CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatMessages_ChatSessionId' AND object_id = OBJECT_ID('ChatMessages'))
                    CREATE INDEX [IX_ChatMessages_ChatSessionId] ON [ChatMessages] ([ChatSessionId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatSessions_CategoryId' AND object_id = OBJECT_ID('ChatSessions'))
                    CREATE INDEX [IX_ChatSessions_CategoryId] ON [ChatSessions] ([CategoryId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatSessions_GovServiceId' AND object_id = OBJECT_ID('ChatSessions'))
                    CREATE INDEX [IX_ChatSessions_GovServiceId] ON [ChatSessions] ([GovServiceId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatSessions_UserId' AND object_id = OBJECT_ID('ChatSessions'))
                    CREATE INDEX [IX_ChatSessions_UserId] ON [ChatSessions] ([UserId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CitizenProfiles_UserId' AND object_id = OBJECT_ID('CitizenProfiles'))
                    CREATE UNIQUE INDEX [IX_CitizenProfiles_UserId] ON [CitizenProfiles] ([UserId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Feedbacks_ChatSessionId' AND object_id = OBJECT_ID('Feedbacks'))
                    CREATE UNIQUE INDEX [IX_Feedbacks_ChatSessionId] ON [Feedbacks] ([ChatSessionId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Feedbacks_UserId' AND object_id = OBJECT_ID('Feedbacks'))
                    CREATE INDEX [IX_Feedbacks_UserId] ON [Feedbacks] ([UserId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GovServices_CategoryId' AND object_id = OBJECT_ID('GovServices'))
                    CREATE INDEX [IX_GovServices_CategoryId] ON [GovServices] ([CategoryId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RequiredDocuments_GovServiceId' AND object_id = OBJECT_ID('RequiredDocuments'))
                    CREATE INDEX [IX_RequiredDocuments_GovServiceId] ON [RequiredDocuments] ([GovServiceId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ServiceGeneralDocs_GovServiceId' AND object_id = OBJECT_ID('ServiceGeneralDocs'))
                    CREATE INDEX [IX_ServiceGeneralDocs_GovServiceId] ON [ServiceGeneralDocs] ([GovServiceId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ServiceOptionChoices_ServiceOptionId' AND object_id = OBJECT_ID('ServiceOptionChoices'))
                    CREATE INDEX [IX_ServiceOptionChoices_ServiceOptionId] ON [ServiceOptionChoices] ([ServiceOptionId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ServiceOptions_GovServiceId' AND object_id = OBJECT_ID('ServiceOptions'))
                    CREATE INDEX [IX_ServiceOptions_GovServiceId] ON [ServiceOptions] ([GovServiceId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ServiceSteps_GovServiceId' AND object_id = OBJECT_ID('ServiceSteps'))
                    CREATE INDEX [IX_ServiceSteps_GovServiceId] ON [ServiceSteps] ([GovServiceId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserDocuments_ChatSessionId' AND object_id = OBJECT_ID('UserDocuments'))
                    CREATE INDEX [IX_UserDocuments_ChatSessionId] ON [UserDocuments] ([ChatSessionId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserDocuments_RequiredDocumentId' AND object_id = OBJECT_ID('UserDocuments'))
                    CREATE INDEX [IX_UserDocuments_RequiredDocumentId] ON [UserDocuments] ([RequiredDocumentId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserDocuments_UserId' AND object_id = OBJECT_ID('UserDocuments'))
                    CREATE INDEX [IX_UserDocuments_UserId] ON [UserDocuments] ([UserId]);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "CitizenProfiles");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "ServiceGeneralDocs");

            migrationBuilder.DropTable(
                name: "ServiceOptionChoices");

            migrationBuilder.DropTable(
                name: "ServiceSteps");

            migrationBuilder.DropTable(
                name: "UserDocuments");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ServiceOptions");

            migrationBuilder.DropTable(
                name: "ChatSessions");

            migrationBuilder.DropTable(
                name: "RequiredDocuments");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "GovServices");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}