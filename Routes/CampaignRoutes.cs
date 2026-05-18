using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpgmanagerapi.Data;
using rpgmanagerapi.Data.DTOs;
using rpgmanagerapi.Models;
using rpgmanagerapi.Validations;
using Scalar.AspNetCore;

namespace rpgmanagerapi.Routes;

public static class CampaignRoutes
{
    public static void CampaignEndpoints(this WebApplication app)
    {
        var campaignGroup = app.MapGroup("/camapigns");

        campaignGroup.MapPost("/campaigns", async (
            [FromBody] CreateCampaignDTO input,
            ApplicationDbContext _context
        ) =>
        {
            var createCampaignValidator = new CreateCampaignValidation();

            var validateDTO = createCampaignValidator.Validate(input);

            if (!validateDTO.IsValid)
            {
                return Results.BadRequest(validateDTO.Errors.FirstOrDefault().ErrorMessage);
            }

            var newCampaign = Campaign
                .CreateCampaign(input.Name, input.Description, input.system, Guid.NewGuid());

            await _context.AddAsync(newCampaign.Data);

            await _context.SaveChangesAsync();

            return Results.Ok(newCampaign);
        })
        .WithName("create-campaigns");

        campaignGroup.MapGet("/campaigns", async (ApplicationDbContext _context) =>
        {
            return _context.Set<Campaign>().ToListAsync();
        })
        .WithName("get-campaigns");

        campaignGroup.MapPut("/campaigns", async (
            [FromBody] UpdateCampaignDTO input,
            ApplicationDbContext _context
        ) =>
        {
            var updateCampaignValidator = new UpdateCampaignValidation();

            var validateDTO = updateCampaignValidator.Validate(input);

            if (!validateDTO.IsValid) return Results.BadRequest(validateDTO.Errors.FirstOrDefault().ErrorMessage);

            var campaignFound = await _context.Set<Campaign>().FirstOrDefaultAsync(c => c.Id.ToString() == input.Id.ToUpper());
            if (campaignFound == null) return Results.NotFound();

            campaignFound.UpdateCampaign(input.Name, input.Description);

            _context.Set<Campaign>().Update(campaignFound);

            await _context.SaveChangesAsync();

            return Results.Ok(campaignFound);

        });

        campaignGroup.MapDelete("/campaigns/{id}", async (ApplicationDbContext _context, string id) =>
        {
            var campaignFound = await _context.Set<Campaign>().FirstOrDefaultAsync(c => c.Id.ToString() == id.ToUpper());
            if (campaignFound == null) return Results.NotFound();

            _context.Set<Campaign>().Remove(campaignFound);

            await _context.SaveChangesAsync();

            return Results.Ok();
        });
    }
}