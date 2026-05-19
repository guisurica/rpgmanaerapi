using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpgmanagerapi.Common;
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
        var campaignGroup = app.MapGroup("/campaigns");

        campaignGroup.RequireAuthorization();

        campaignGroup.MapPost("/", async (
            [FromBody] CreateCampaignDTO input,
            ApplicationDbContext _context,
            IHttpContextAccessor _httpContext
        ) =>
        {
            var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userId.Value == null)
                return Results.Unauthorized();

            var createCampaignValidator = new CreateCampaignValidation();

            var validateDTO = createCampaignValidator.Validate(input);

            if (!validateDTO.IsValid)
            {
                return Results.BadRequest(validateDTO.Errors.FirstOrDefault().ErrorMessage);
            }

            var userFound = await _context.Set<User>().FirstOrDefaultAsync(c => c.Id == new Guid(userId.Value));
            if (userFound == null)
                return Results.NotFound();

            var newCampaign = Campaign
                .CreateCampaign(input.Name, input.Description, input.system, new Guid(userId.Value));

            var campaign = await _context.AddAsync(newCampaign.Data);

            var newCampaignPlayer = PlayerCampaign.Create(userFound.Name,
                userFound.Id,
                newCampaign.Data.Id,
                userFound,
                campaign.Entity
            );

            await _context.Set<PlayerCampaign>().AddAsync(newCampaignPlayer.Data);

            await _context.SaveChangesAsync();

            return Results.Ok(newCampaign);
        })
        .WithName("create-campaigns");

        campaignGroup.MapGet("/{userId}", async (ApplicationDbContext _context, string userId) =>
        {
            return _context.Set<PlayerCampaign>().Where(p => p.UserId == new Guid(userId)).ToListAsync();
        })
        .WithName("get-campaigns");

        campaignGroup.MapPut("/", async (
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

        campaignGroup.MapDelete("/{id}", async (ApplicationDbContext _context, string id) =>
        {
            var campaignFound = await _context.Set<Campaign>().FirstOrDefaultAsync(c => c.Id.ToString() == id.ToUpper());
            if (campaignFound == null) return Results.NotFound();

            _context.Set<Campaign>().Remove(campaignFound);

            await _context.SaveChangesAsync();

            return Results.Ok();
        });
    }
}