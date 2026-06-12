using Khedmetak.BLL.MappingProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Khedmetak.API.Middlewares;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repo.Implementation.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

#region Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
#endregion

#region Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IGovServiceService, GovServiceService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<JwtService>(); 
#endregion

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();  
app.UseAuthorization();   
app.MapControllers();
app.Run();