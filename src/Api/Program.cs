using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using TMS.Addresses.Application;
using TMS.Addresses.Infrastructure;
using TMS.Addresses.Infrastructure.Persistence;
using TMS.Api;
using TMS.Api.Filters;
using TMS.Identity.Application;
using TMS.Identity.Infrastructure;
using TMS.Identity.Infrastructure.Authentication;
using TMS.Identity.Infrastructure.Persistence;
using TMS.Api.OpenApi;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddControllers(options =>
    options.Filters.Add<FluentValidationFilter>());

builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
    options.AddOperationTransformer<OperationIdFilter>();
});

builder.Services.AddAddressesApplication();
builder.Services.AddAddressesInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);

var jwt = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException("Missing Jwt configuration.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt.SigningKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy
            .WithOrigins("http://localhost:4200", "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

using (var scope = app.Services.CreateScope())
{
    var addressesDb = scope.ServiceProvider
        .GetRequiredService<AddressesDbContext>();
    await addressesDb.Database.MigrateAsync();

    var authDb = scope.ServiceProvider
        .GetRequiredService<AuthDbContext>();
    await authDb.Database.MigrateAsync();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<ApplicationUser>>();
    await SeedData.SeedAsync(addressesDb, userManager);
}

app.Run();