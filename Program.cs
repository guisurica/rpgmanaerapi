using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpgmanagerapi.Data;
using rpgmanagerapi.Data.DTOs;
using rpgmanagerapi.Models;
using rpgmanagerapi.Validations;
using Scalar.AspNetCore;
using Microsoft.OpenApi;
using rpgmanagerapi.Routes;

var builder = WebApplication.CreateBuilder(args);

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

app.CampaignEndpoints();

app.Run();
