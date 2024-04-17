
using CarRental_BE.Interfaces;
using CarRental_BE.Models.Auth;
using CarRental_BE.Repositories.DBContext;
using CarRental_BE.Repositories.FollowVehicle;
using CarRental_BE.Repositories.PostVehicle;
using CarRental_BE.Repositories.RentVehicle;
using CarRental_BE.Repositories.ReviewVehicle;
using CarRental_BE.Repositories.User;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDistributedMemoryCache();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add Swagger/OpenAPI
builder.Services.AddSwaggerGen();

// Add NewtonsoftJson
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext")));

// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
    };
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSetting"));

// Add scoped services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IMailService, MailService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPostVehicleRepository, PostVehicleRepository>();
builder.Services.AddScoped<IRentVehicleRepository, RentVehicleRepository>();
builder.Services.AddScoped<IReviewVehicleRepository, ReviewVehicleRepository>();
builder.Services.AddScoped<IFollowVehicleRepository, FollowVehicleRepository>();


// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(x => x
     .AllowAnyMethod()
     .AllowAnyHeader()
     .AllowCredentials()
     .SetIsOriginAllowed(origin => true));

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Create user-content directory if it doesn't exist
string userContentDirectory = Path.Combine(app.Environment.ContentRootPath, "user-content");
if (!Directory.Exists(userContentDirectory))
{
    Directory.CreateDirectory(userContentDirectory);
}

app.MapControllers();

app.Run();
