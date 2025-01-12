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
var builder = WebApplication.CreateBuilder(args);

/* DI Container **********************/
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IDbConnection>(sp =>
                                             {
                                                 //appsettings json 에 작성된 커넥션 스트링으로 NpgsqlConnection 인스턴스 생성해서 IDbConnection 객체를 만든다.
                                                 string? connectionString = builder.Configuration.GetConnectionString("Postgres");
                                                 return new NpgsqlConnection(connectionString);
                                             });
// Shared 확장 메서드로 서비스와 레포지토리를 등록
builder.Services.AddCruxServicesAll();
builder.Services.AddCruxRepositoriesAll();

// JWT 인증 설정
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                                                                                        {
                                                                                            options.TokenValidationParameters = new TokenValidationParameters
                                                                                            {
                                                                                                ValidateIssuer = true,
                                                                                                ValidateAudience = true,
                                                                                                ValidateLifetime = true,
                                                                                                ValidateIssuerSigningKey = true,
                                                                                                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                                                                                                ValidAudience = builder.Configuration["Jwt:Audience"],
                                                                                                IssuerSigningKey = JwtHelper.GetPublicKey(builder.Configuration)
                                                                                            };
                                                                                        });

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

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
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

/***************
 * Run the host
 **************/
app.Run();
