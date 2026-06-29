using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.Agents.Implementaion;
using Khedmetak.AI.Agents.Implementation;
using Khedmetak.AI.Configuration;
using Khedmetak.AI.RAG;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.AI.Services.Implementation;
using Khedmetak.API.Middlewares;
using Khedmetak.BLL.MappingProfile;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction;
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
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenAI;
using OpenAI.Chat;
using Qdrant.Client;
using System.ClientModel;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Controllers
builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddEndpointsApiExplorer();
#endregion

#region Swagger
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
#endregion

#region DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
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
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
#endregion

#region AI Configuration
builder.Services.Configure<AISettings>(
    builder.Configuration.GetSection("AI"));

var apiKey = builder.Configuration.GetSection("AI")["ApiKey"];

if (!string.IsNullOrWhiteSpace(apiKey))
{
    var openAIClient = new OpenAIClient(
        new ApiKeyCredential(apiKey),
        new OpenAIClientOptions
        {
            Endpoint = new Uri("https://models.inference.ai.azure.com"),
        });

    builder.Services.AddKeyedSingleton<OpenAIClient>("github", openAIClient);
    builder.Services.AddSingleton<IChatClient>(sp =>
        new ChatClientBuilder(openAIClient.GetChatClient(
            builder.Configuration["AI:Model"]).
            AsIChatClient()).UseFunctionInvocation().Build());

    builder.Services.AddHttpClient("jina", client =>
    {
        client.BaseAddress = new Uri("https://api.jina.ai");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                builder.Configuration["AI:EmbeddingAPIKey"]);
    });
}
#endregion

#region Qdrant VectorDatabase
builder.Services.Configure<QdrantDBSettings>(
    builder.Configuration.GetSection("QdrantVectorDB"));

builder.Services.AddSingleton<QdrantClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<QdrantDBSettings>>().Value;

    return new QdrantClient(
        host: settings.QdrantEndpoint,
        port: 6334,
        https: true,
        apiKey: string.IsNullOrWhiteSpace(settings.QdrantApiKey) ? "placeholder" : settings.QdrantApiKey
    );
});
#endregion

#region Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IGovServiceRepository, GovServiceRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IServiceStepRepository, ServiceStepRepository>();
builder.Services.AddScoped<IRequiredDocumentRepository, RequiredDocumentRepository>();
builder.Services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
builder.Services.AddScoped<IUserDocumentRepository, UserDocumentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChatSessionRepository, ChatSessionRepository>();

#endregion

#region Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IGovServiceService, GovServiceService>();
builder.Services.AddScoped<IGovServiceAdminService, GovServiceAdminService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IUserDashboardService, UserDashboardService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

// AI Services
builder.Services.AddScoped<IChatSessionService, ChatSessionService>();
builder.Services.AddScoped<IChatMessageService, ChatMessageService>();
//builder.Services.AddScoped<IAIChatService, AIChatService>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
builder.Services.AddScoped<IChunkService, ChunkService>();
builder.Services.AddScoped<IUserDocumentService, UserDocumentService>();

builder.Services.AddScoped<IVectorDB, QdrantService>();
builder.Services.AddScoped<IVectorDBOperationsService, VectorDBOperationsService>();
builder.Services.AddScoped<IVectorDBService, VectorDBService>();
builder.Services.AddScoped<IRelevanceValidatorAgent, RelevanceValidatorAgent>();
builder.Services.AddScoped<IRagService, RagService>();
builder.Services.AddScoped<IGovServiceTools, GovServiceTools>();

// Agents
builder.Services.AddScoped<IServiceIntentAgent, ServiceIntentAgent>();
builder.Services.AddScoped<IRewriteQuestionAgent, RewriteQuestionAgent>();
builder.Services.AddScoped<IAIServiceResponseAgent, AIServiceResponseAgent>();
builder.Services.AddScoped<IGeneralChatAgent, GeneralChatAgent>();

builder.Services.AddScoped<IChatOrchestrator, ChatOrchestrator>();
#endregion

// ✅ Build بره الـ if
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// ✅ Static files عشان الملفات المرفوعة في wwwroot/uploads تكون accessible
app.UseStaticFiles();

// ✅ CORS لازم يكون قبل Authentication
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();