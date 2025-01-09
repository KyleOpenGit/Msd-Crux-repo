using System.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MSD.Crux.API.Helpers;
using MSD.Crux.Core.Repositories;
using MSD.Crux.Core.Services;
using MSD.Crux.Infra.Repositories;
using MSD.Crux.Infra.Services;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

/* DI Container **********************/
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IDbConnection>(sp =>
                                             {
                                                 string? connectionString = builder.Configuration.GetConnectionString("Postgres");
                                                 return new NpgsqlConnection(connectionString);
                                             });

// DI: Core 인터페이스 ↔ Infra 구현체 등록
builder.Services.AddScoped<IEmployeeRepo, EmployeeRepoPsqlDb>();
builder.Services.AddScoped<IUserRepo, UserRepoPsqlDb>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IUserService, UserService>();
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

/* HTTP request pipeline **********************/
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
