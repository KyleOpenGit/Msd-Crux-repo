using System.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MSD.Crux.API.Helpers;
using MSD.Crux.Common;
using MSD.Crux.Shared;
using Npgsql;

/*******************
 * Web Host Builder
 *******************/
WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

/* DI Container **********************/
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Shared 확장 메서드로 서비스, 레포지토리, 공통 설정을 등록
builder.Services.AddCruxSharedConfiguration(builder.Configuration);
builder.Services.AddCruxServicesAll();
builder.Services.AddCruxRepositoriesAll();

WebApplication? app = builder.Build();


/*************************************************
 * HTTP request 파이프라인에 미들웨어 추가 및 설정
 *************************************************/
if (app.Environment.IsDevelopment() || app.Environment.IsLocal())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

/***************
 * Run the host
 **************/
app.Run();
