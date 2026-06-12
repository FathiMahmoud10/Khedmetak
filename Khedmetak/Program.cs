using Khedmetak.AI.Configuration;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.AI.Services.Implementation;
using Khedmetak.BLL.MappingProfile;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.Core.Data;
//using Khedmetak.DAL.UnitOfWork;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repo.Implementation.UnitOfWork;
using Khedmetak.DAL.Repo.shared;
using Khedmetak.DAL.Repositories;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")));
#region AutoMapper
builder.Services.AddAutoMapper(typeof(KhedmetakProfile));
#endregion


#region AIConfiguration

builder.Services.Configure<AISettings>(
    builder.Configuration.GetSection("AI"));

builder.Services.AddHttpClient<IAIChatService, AIChatService>(client =>
{
    client.BaseAddress = new Uri("https://openrouter.ai/api/v1/responses");
});

// configure the header of request to AI Model that should contain APIKey, and URL of AI  Model Provider
//var openAiClient = new OpenAIClient(
//          new ApiKeyCredential(builder.Configuration.GetSection("AI")["ApiKey"]),
//          new OpenAIClientOptions
//          {
//              Endpoint = new Uri("https://openrouter.ai/api/v1/responses"),
//          });

//builder.Services.AddSingleton<IChatClient>(sp =>
//new ChatClientBuilder(openAiClient.GetChatClient(builder.Configuration["AI:Model"]).AsIChatClient()).UseFunctionInvocation().Build());

//builder.Services.AddSingleton(openAiClient);

#endregion

#region Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IGovServiceRepository, GovServiceRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
#endregion

#region Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IGovServiceService, GovServiceService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IChatSessionService, ChatSessionService>();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();