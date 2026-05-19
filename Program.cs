using Microsoft.EntityFrameworkCore;
using rpgmanagerapi.Data;
using Scalar.AspNetCore;
using Microsoft.OpenApi;
using rpgmanagerapi.Routes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using rpgmanagerapi.Data.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "rpgmanager.com",
            ValidAudience = "rpgmanager.com",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });

builder.Services.AddAuthorization();


builder.Services.Configure<JWTConfig>(builder.Configuration.GetSection("JWT"));
builder.Services.AddHttpContextAccessor();

builder.Services.AddOpenApi();

builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((doc, _, _) =>
    {
        doc.Info = new OpenApiInfo
        {
            Title = "Rpg manager API",
            Version = "1.0",
            Description = "Game management api.",
        };
        return Task.CompletedTask;
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/v1.json");

    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

app.CampaignEndpoints();
app.AuthEndpoints();

app.Run();
