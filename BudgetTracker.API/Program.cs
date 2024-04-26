using BudgetTracker.Domain.Data;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Model;
using BudgetTracker.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var services = builder.Services;
var configuration = builder.Configuration;

services.AddScoped<IUserService, UserService>();
services.AddScoped<IEmailService, EmailService>();
services.AddScoped<ITokenService, TokenService>();
var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
services.AddSingleton(emailConfig);

services.Configure<IdentityOptions>(
options => options.SignIn.RequireConfirmedEmail = true);

services.AddDbContext<AuthDBContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


services.AddDbContext<BudgetTrackerDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("BudgetTrackConnection")));
services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDBContext>()
    .AddDefaultTokenProviders();

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = configuration["Authentication:JwtBearer:Issuer"],
            ValidAudience = configuration["Authentication:JwtBearer:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:JwtBearer:SecretKey"]))

        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
