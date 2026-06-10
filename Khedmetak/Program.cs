using Khedmetak.BLL.MappingProfile;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.Core.Data;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repo.Implementation.UnitOfWork;
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

#region Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
#endregion

#region Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IGovServiceService, GovServiceService>();
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