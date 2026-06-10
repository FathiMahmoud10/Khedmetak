using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.Core.Data;
using Khedmetak.DAL.Repositories;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")));

var app = builder.Build();

#region Repositories
#region Fathi
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IGovServiceRepository, GovServiceRepository>();
#endregion


#endregion
#region Services
#region Fathi
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IGovServiceService, GovServiceService>();
#endregion


#endregion



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();