using Xero.Interfaces;
using Xero.Services;
using Xero.Helper;


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Xero.Core.Services.Interfaces;
using Xero.Core.Services.Main;
using Xero.Core.Services.Context;

using Xero.Core.Data.Configuration;
using Xero.NetStandard.OAuth2.Client;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();


// Add Database Connection
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:Production"]));

// Add JwtConfiguration
var JwtConfiguration = builder.Configuration.GetSection("JwtConfiguration").Get<JwtConfiguration>();

builder.Services.AddScoped<IXeroService, XeroService>();
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddTransient<IStaffService, StaffService>();

builder.Services.AddHostedService<TokenRefreshService>();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = JwtConfiguration.Issuer,
        ValidAudience = JwtConfiguration.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfiguration.Secret))
    };
}); ;

var app = builder.Build();


app.ConfigureCustomExceptionMiddleware();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
