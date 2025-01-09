/*******************
 * Web Host Builder
 *******************/

using System.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MSD.Crux.API.Helpers;
using MSD.Crux.API.Repositories;
using MSD.Crux.API.Repositories.InMemory;
using MSD.Crux.API.Repositories.Psql;
using MSD.Crux.API.Repositories.PsqlDb;
using MSD.Crux.API.Services;
using MSD.Crux.Core.Repositories;
using MSD.Crux.Core.Services;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

/* DI Container   ******************/
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IDbConnection>(sp =>
                                             {
                                                 //appsettings json 에 작성된 커넥션 스트링으로 NpgsqlConnection 인스턴스 생성해서 IDbConnection 객체를 만든다.
                                                 string? connectionString = builder.Configuration.GetConnectionString("Postgres");
                                                 return new NpgsqlConnection(connectionString);
                                             });
builder.Services.AddTransient<IEmployeeRepo, EmployeeRepoPsqlDb>();
builder.Services.AddTransient<IUserRepo, UserRepoPsqlDb>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IUserLoginService, UserLoginService>();
// JWT 인증 설정
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                                                                                        {
                                                                                            options.TokenValidationParameters = new TokenValidationParameters
                                                                                            {
                                                                                                ValidateIssuer = true,
                                                                                                ValidateAudience = true,
                                                                                                ValidateLifetime = true,
                                                                                                ValidateIssuerSigningKey = true,
                                                                                                ValidIssuer =
                                                                                                                                        builder.Configuration["Jwt:Issuer"],
                                                                                                ValidAudience =
                                                                                                                                        builder.Configuration["Jwt:Audience"],
                                                                                                IssuerSigningKey =
                                                                                                                                        JwtHelper
                                                                                                                                            .GetPublicKey(builder.Configuration)
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

app.UseAuthorization();

app.MapControllers();

/***************
 * Run the host
 **************/
app.Run();
