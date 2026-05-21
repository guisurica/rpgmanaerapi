using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpgmanagerapi.Data;
using rpgmanagerapi.Data.DTOs;
using rpgmanagerapi.Models;
using rpgmanagerapi.Validations;

namespace rpgmanagerapi.Routes;

public static class ReportRoutes
{
    public static void MapReportRoutes(this WebApplication app)
    {
        var reportGroup = app.MapGroup("/reports")
            .RequireAuthorization();

        reportGroup.MapPost("/", CreateReportAsync);
        reportGroup.MapGet("/{campaignId}", GetCampaignReportsAsync);
    }

    private static async Task<IResult> CreateReportAsync(
        [FromBody] CreateReportDTO input,
        IHttpContextAccessor _httpContext,
        ApplicationDbContext _context)
    {
        var createReportValidator = new CreateReportValidation();
        var inputValidate = createReportValidator.Validate(input);

        if (!inputValidate.IsValid)
        {
            return Results.BadRequest(inputValidate.Errors.FirstOrDefault().ErrorMessage);
        }

        var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userId.Value == null)
        {
            return Results.Unauthorized();
        }

        var userFound = await _context.Set<User>().FindAsync(new Guid(userId.Value));
        if (userFound == null)
        {
            return Results.NotFound("Usuário não encontrado");
        }

        var campaignFound = await _context.Set<Campaign>().FindAsync(new Guid(input.campaignId));
        if (campaignFound == null)
        {
            return Results.NotFound("Campanha não encontrada");
        }

        var pcFound = await _context.Set<PlayerCampaign>()
            .FirstOrDefaultAsync(pc => pc.UserId == userFound.Id &&
                                    pc.CampaignId == campaignFound.Id);

        if (pcFound == null)
        {
            return Results.BadRequest("Você não pertence a essa campanha");
        }

        if (pcFound.PlayerStatus != PlayerStatus.ADMIN)
        {
            return Results.BadRequest("Apenas o mestre da campanha pode criar um evento");
        }

        var createNewReport = Report.Create(input.name, input.text, input.campaignId, campaignFound);
        if (!createNewReport.IsSuccessResult)
        {
            return Results.BadRequest(createNewReport.Message);
        }

        createNewReport.Data.MarkAsRead(pcFound.Id);

        await _context.Set<Report>().AddAsync(createNewReport.Data);

        await _context.SaveChangesAsync();

        return Results.Ok(createNewReport.Message);
    }

    private static async Task<IResult> GetCampaignReportsAsync(
        string campaignId,
        ApplicationDbContext _context)
    {
        var reports = await _context.Set<Report>()
            .Where(r => r.CampaignId == new Guid(campaignId))
            .ToListAsync();

        return Results.Ok(reports);
    }
}