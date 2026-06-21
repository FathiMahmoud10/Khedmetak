using Khedmetak.AI.Configuration;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.AI.Services.Implementation;
using Khedmetak.API.Middlewares;
using Khedmetak.BLL.MappingProfile;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repo.Implementation;
using Khedmetak.DAL.Repo.Implementation.UnitOfWork;
using Khedmetak.DAL.Repo.shared;
using Khedmetak.DAL.Repositories;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenAI;
using System.ClientModel;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Khedmetak API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "ادخل التوكن هنا مباشرة"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#region DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")));
#endregion

#region Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
#endregion

#region JWT
var jwt = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["SecretKey"]!))
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = Khedmetak.BLL.ApiResponse.ApiResponse<string>.Fail("غير مصرح لك، يرجى تسجيل الدخول أولاً");
            var json = System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });
            await context.Response.WriteAsync(json);
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = Khedmetak.BLL.ApiResponse.ApiResponse<string>.Fail("غير مسموح لك بالوصول إلى هذا المصدر");
            var json = System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });
            await context.Response.WriteAsync(json);
        }
    };
});
#endregion

#region AutoMapper
builder.Services.AddAutoMapper(typeof(KhedmetakProfile));
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
#endregion

#region AIConfiguration

builder.Services.Configure<AISettings>(
    builder.Configuration.GetSection("AI"));

var openAIClient = new OpenAIClient(
        new ApiKeyCredential(builder.Configuration.GetSection("AI")["ApiKey"]),
          new OpenAIClientOptions
          {
              Endpoint = new Uri("https://models.github.ai/inference"),
          });

builder.Services.AddSingleton(openAIClient);

#endregion

#region Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IGovServiceRepository, GovServiceRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IServiceStepRepository, ServiceStepRepository>();
builder.Services.AddScoped<IRequiredDocumentRepository, RequiredDocumentRepository>();
#endregion

#region Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IGovServiceService, GovServiceService>();
builder.Services.AddScoped<IGovServiceAdminService, GovServiceAdminService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<JwtService>();

//-------- AI Services -----------------------------------
builder.Services.AddScoped<IChatSessionService, ChatSessionService>();
builder.Services.AddScoped<IChatMessageService, ChatMessageService>();
builder.Services.AddScoped<IAIChatService, AIChatService>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
builder.Services.AddScoped<IChunkService, ChunkService>();

builder.Services.AddScoped<IQdrantService, QdrantService>();
builder.Services.AddScoped<IVectorIndexingService, VectorIndexingService>();
#endregion

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll"); // ← لازم يكون قبل Authentication و Authorization

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();