using Khedmetak.AI.Agents.Abstraction;
using Khedmetak.AI.Agents.Implementaion;
using Khedmetak.AI.Agents.Implementation;
using Khedmetak.AI.Configuration;
using Khedmetak.AI.Orchestrators;
using Khedmetak.AI.RAG;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.AI.Services.Implementation;
using Khedmetak.API.Middlewares;
using Khedmetak.BLL.MappingProfile;
using Khedmetak.BLL.Services;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.BLL.Services.Abstraction.Fawry;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.BLL.Services.Interfaces;
using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Entities.FawrySettings;
using Khedmetak.DAL.Repo;
using Khedmetak.DAL.Repo.Abstraction;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repo.Implementation;
using Khedmetak.DAL.Repo.Implementation.UnitOfWork;
using Khedmetak.DAL.Repo.shared;
using Khedmetak.DAL.Repositories;
using Khedmetak.DAL.Repositories.Interfaces;
using Khedmetak.DigitalPortal.Services.Abstraction;
using Khedmetak.DigitalPortal.Services.Implementation;
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
using Shard.VectorDBInterfaces;
using System.ClientModel;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Controllers
/*
  أي endpoint في الـ API ده
 هيتمنع الوصول ليه لأي حد مش مسجّل دخول، إلا لو حطيت 
[AllowAnonymous] صريح فوق 
الـ Controller أو الـ Action عشان تستثنيه من القاعدة دي.
 */

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
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
        Description = ""
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

/*  الغرض من الجزء ده عو استخدام جداول ال  identity  الجاهزة */
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = false;   // مش لازم رقم
    options.Password.RequireLowercase = false;  // مش لازم حرف صغير
    options.Password.RequireUppercase = false; // مش لازم حرف كبير
    options.Password.RequireNonAlphanumeric = false;  // مش لازم رمز زي @ أو # أو $
    options.Password.RequiredLength = 6; // طول الباسورد لازم يكون 6 حروف على الأقل
})
    .AddEntityFrameworkStores<AppDbContext>() //  بيخزن البيانات بتاعت امستخدم باستخدام الكونكشن المووجود في 
    .AddDefaultTokenProviders();
#endregion

#region JWT
// الغرض من الجزء ده عو استخدام ال JWT  في تسجيل الدخول

var jwt = builder.Configuration.GetSection("JwtSettings"); // ده بيقرأ section اسمها "JwtSettings" من ملف appsettings

//استخدم JWT Bearer كطريقة التحقق الافتراضية" لأي request جاي، سواء عشان تتحقق من هوية المستخدم 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, //يتأكد إن الـ Token اتولّد من نفس السيرفر بتاعك 
        ValidateAudience = true, //يتأكد إن الـ Token موجه للجمهور المستهدف (Audience) بتاعك
        ValidateLifetime = true, //يتأكد إن الـ Token لسه صالح ومش منتهي
        ValidateIssuerSigningKey = true, //يتأكد إن الـ Token اتوقّع باستخدام المفتاح السري بتاعك
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["SecretKey"]!))
    };

    // ده بيخلي الـ API يرد برسالة JSON بدل ما يرد بالصفحة الافتراضية
    // للـ 401 Unauthorized أو 403 Forbidden
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
        // ده بيخلي الـ API يرد برسالة JSON بدل ما يرد بالصفحة الافتراضية

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
// الغرض من الجزء ده عو استخدام AutoMapper  في تحويل ال DTOs  لل Entities  والعكس
builder.Services.AddAutoMapper(typeof(KhedmetakProfile));
#endregion

#region CORS

//الكود ده بيظبط إعدادات CORS (Cross-Origin Resource Sharing) —
//وهي الآلية اللي بتحدد مين مسموحله يبعت Requests للـ API بتاعك من دومين مختلف
//(زي لو الـ Frontend شغال على دومين تاني غير الـ Backend).
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
builder.Services.Configure<AISettings>(builder.Configuration.GetSection("AI"));

var apiKey = builder.Configuration.GetSection("AI")["ApiKey"];

//if (!string.IsNullOrWhiteSpace(apiKey))

    var openAIClient = new OpenAIClient(
        new ApiKeyCredential(apiKey),
        new OpenAIClientOptions
        {
            Endpoint = new Uri("https://models.github.ai/inference"),
        });

    //------------ OpenAI Client For AI Chat  --------------
    builder.Services.AddKeyedSingleton<OpenAIClient>("github", openAIClient);


    // ----------- Chat Client for AI Chat ----------- 
    builder.Services.AddKeyedSingleton<IChatClient>("Chat", (sp, _) =>
    {
        var client = new OpenAIClient(
            new ApiKeyCredential(builder.Configuration["AI:ApiKey"]!),
             new OpenAIClientOptions
             {
                 Endpoint = new Uri("https://models.github.ai/inference")
             });

        return new ChatClientBuilder(
            client.GetChatClient(builder.Configuration["AI:Model"]!)
                .AsIChatClient())
            .UseFunctionInvocation()
            .Build();
    });

    //------------Chat Client For Document Validation  --------------
    builder.Services.AddKeyedSingleton<IChatClient>("DocValidation", (sp, _) =>
    {
        var client = new OpenAIClient(
            new ApiKeyCredential(builder.Configuration["AIValidation:Llama:ApiKey"]!),
            new OpenAIClientOptions
            {
                Endpoint = new Uri("https://models.github.ai/inference")
            });

        return new ChatClientBuilder(
            client.GetChatClient(builder.Configuration["AIValidation:Llama:DocumentModel"]!)
                .AsIChatClient())
            //.UseFunctionInvocation()
            .Build();
    });



    //builder.Services.AddSingleton<IChatClient>(sp =>
    //    new ChatClientBuilder(openAIClient.GetChatClient(
    //        builder.Configuration["AI:Model"]).
    //        AsIChatClient()).UseFunctionInvocation().Build());


    //----------- Embedding Client for Jina API -----------
    builder.Services.AddHttpClient("jina", client =>
    {
        client.BaseAddress = new Uri("https://api.jina.ai");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                builder.Configuration["AI:EmbeddingAPIKey"]);
    });

    // ------------- Embedding for image --------------
    //builder.Services.AddSingleton<IClipImageEmbeddingService>(sp =>
    //{
    //    var env = sp.GetRequiredService<IHostEnvironment>();

    //    var modelPath = Path.Combine(
    //        env.ContentRootPath,
    //        "AIModels",
    //        "Clip",
    //        "image_encode.onnx");

    //    return new ClipImageEmbeddingService(modelPath);
    //});

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

#region Dependency Injection

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
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IStandardDocumentRepository, StandardDocumentRepository>();
#endregion

#region Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.Configure<FawrySettings>(builder.Configuration.GetSection("Fawry"));
builder.Services.AddScoped<IFawryService, FawryMockService>(); // للتجربة دلوقتي
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IGovServiceService, GovServiceService>();
builder.Services.AddScoped<IGovServiceAdminService, GovServiceAdminService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IUserDashboardService, UserDashboardService>();
builder.Services.AddScoped<IServiceFeeTierRepository, ServiceFeeTierRepository>();
builder.Services.AddScoped<IServiceImportantNoteRepository, ServiceImportantNoteRepository>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IStandardDocumentService, StandardDocumentService>();
builder.Services.AddHttpClient<IDigitalPortalService, DigitalPortalHttpService>(client =>
{
    var baseUrl = builder.Configuration["DigitalPortalSettings:BaseUrl"] ?? "http://localhost:5200/";
    if (!baseUrl.EndsWith("/"))
    {
        baseUrl += "/";
    }
    client.BaseAddress = new Uri(baseUrl);
});

// AI Services
builder.Services.AddScoped<IChatSessionService, ChatSessionService>();
builder.Services.AddScoped<IChatMessageService, ChatMessageService>();
//builder.Services.AddScoped<IAIChatService, AIChatService>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
builder.Services.AddScoped<IChunkService, ChunkService>();
builder.Services.AddScoped<IUserDocumentService, UserDocumentService>();
builder.Services.AddScoped<ITemplatesAgent, TemplatesAgent>();
builder.Services.AddHttpClient<IOCRAgent, OCRAgent>();
builder.Services.AddScoped<IRulesValidationAgent, RulesValidationAgent>();
builder.Services.AddScoped<IDocumentValidationOrchestrator, DocumentValidationOrchestrator>();



builder.Services.AddScoped<IVectorDB, QdrantService>();
//builder.Services.AddScoped<IVectorDBOperationsService, VectorDBOperationsService>();
builder.Services.AddScoped<IVectorDBService, VectorDBService>();
//builder.Services.AddScoped<IImageVectorDbService, ImageVectorDbService>();
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

#endregion

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();