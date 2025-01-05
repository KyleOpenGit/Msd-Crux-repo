/*******************
 * Web Host Builder
 *******************/

using MSD.Crux.API.Helpers;

var builder = WebApplication.CreateBuilder(args);

/* DI Container   ******************/
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

/*************************************************
 * HTTP request 파이프라인에 미들웨어 추가 및 설정
 *************************************************/
if (app.Environment.IsDevelopment() || app.Environment.IsLocal())
{
    Console.WriteLine($"실행중인 Environment: {app.Environment.EnvironmentName}");
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
