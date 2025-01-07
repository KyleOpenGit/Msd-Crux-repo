/*******************
 * Web Host Builder
 *******************/

using MSD.Crux.API.Helpers;
using MSD.Crux.API.Repositories;
using MSD.Crux.API.Repositories.InMemory;
using MSD.Crux.API.Services;

var builder = WebApplication.CreateBuilder(args);

/* DI Container   ******************/
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IEmployeeRepo, EmployeeRepoInMemory>();
builder.Services.AddSingleton<IUserRepo, UserRepoInMemory>();
builder.Services.AddScoped<EmployeeService>();

var app = builder.Build();

/*************************************************
 * HTTP request 파이프라인에 미들웨어 추가 및 설정
 *************************************************/
if (app.Environment.IsDevelopment() || app.Environment.IsLocal())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

/***************
 * Run the host
 **************/
app.Run();
