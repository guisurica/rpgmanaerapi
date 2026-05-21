using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpgmanagerapi.Data;
using rpgmanagerapi.Data.DTOs;
using rpgmanagerapi.Models;
using rpgmanagerapi.Validations;

namespace rpgmanagerapi.Routes;

public static class CampaignRoutes
{
    public static void MapCampaignRoutes(this WebApplication app)
    {
        var campaignGroup = app.MapGroup("/campaigns")
            .RequireAuthorization();

        campaignGroup.MapPost("/", CreateCampaignAsync).WithName("create-campaigns");
        campaignGroup.MapGet("/", GetCampaignsAsync).WithName("get-campaigns");
        campaignGroup.MapPut("/", UpdateCampaignAsync);
        campaignGroup.MapDelete("/{id}", DeleteCampaignAsync);

        campaignGroup.MapPost("/invites", CreateInviteAsync);
        campaignGroup.MapGet("/invites", GetInvitesAsync).WithName("get-invites");
        campaignGroup.MapPut("/invites/{inviteId}", AcceptInviteAsync).WithName("accept-invites");
    }

    private static async Task<IResult> CreateCampaignAsync(
        [FromBody] CreateCampaignDTO input,
        ApplicationDbContext _context,
        IHttpContextAccessor _httpContext)
    {
        var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userId?.Value == null)
            return Results.Unauthorized();

        var createCampaignValidator = new CreateCampaignValidation();
        var validateDTO = createCampaignValidator.Validate(input);

        if (!validateDTO.IsValid)
            return Results.BadRequest(validateDTO.Errors.FirstOrDefault()?.ErrorMessage);

        var userFound = await _context.Set<User>().FirstOrDefaultAsync(c => c.Id == new Guid(userId.Value));
        if (userFound == null)
            return Results.NotFound("Usuário não encontrado");

        var newCampaign = Campaign.CreateCampaign(input.Name, input.Description, input.system, new Guid(userId.Value));

        var campaign = await _context.AddAsync(newCampaign.Data);

        var newCampaignPlayer = PlayerCampaign.Create(
            userFound.Name,
            userFound.Id,
            newCampaign.Data.Id,
            userFound,
            campaign.Entity);

        newCampaignPlayer.Data.SetAdmin();

        await _context.Set<PlayerCampaign>().AddAsync(newCampaignPlayer.Data);
        await _context.SaveChangesAsync();

        return Results.Ok(newCampaign);
    }

    private static async Task<IResult> GetCampaignsAsync(
        ApplicationDbContext _context,
        IHttpContextAccessor _httpContext)
    {
        var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userId?.Value == null)
            return Results.Unauthorized();

        var userCampaigns = await _context.Set<PlayerCampaign>()
            .Include(pc => pc.Campaign)
            .Include(pc => pc.User)
            .AsNoTracking()
            .Where(p => p.UserId == new Guid(userId.Value))
            .ToListAsync();

        var userPcs = new List<PlayerCampaignDTO>();

        foreach (var userPc in userCampaigns)
        {
            var campaignMasterFound = await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Id == userPc.Campaign.CampaignMaster);

            if (campaignMasterFound == null) continue;

            userPcs.Add(new PlayerCampaignDTO(
                userPc.Campaign.Name,
                userPc.Campaign.Description,
                userPc.Campaign.GameSystem,
                campaignMasterFound.Name,
                userPc.Campaign.CreatedOnUtc,
                userPc.Campaign.Id.ToString()
            ));
        }

        return Results.Ok(userPcs);
    }

    private static async Task<IResult> UpdateCampaignAsync(
        [FromBody] UpdateCampaignDTO input,
        ApplicationDbContext _context)
    {
        var updateCampaignValidator = new UpdateCampaignValidation();
        var validateDTO = updateCampaignValidator.Validate(input);

        if (!validateDTO.IsValid)
            return Results.BadRequest(validateDTO.Errors.FirstOrDefault()?.ErrorMessage);

        var campaignFound = await _context.Set<Campaign>()
            .FirstOrDefaultAsync(c => c.Id.ToString() == input.Id.ToUpper());

        if (campaignFound == null)
            return Results.NotFound("Campanha não encontrada");

        campaignFound.UpdateCampaign(input.Name, input.Description);

        _context.Set<Campaign>().Update(campaignFound);
        await _context.SaveChangesAsync();

        return Results.Ok(campaignFound);
    }

    private static async Task<IResult> DeleteCampaignAsync(
        string id,
        ApplicationDbContext _context)
    {
        var campaignFound = await _context.Set<Campaign>()
            .FirstOrDefaultAsync(c => c.Id.ToString() == id.ToUpper());

        if (campaignFound == null)
            return Results.NotFound("Campanha não encontrada");

        _context.Set<Campaign>().Remove(campaignFound);
        await _context.SaveChangesAsync();

        return Results.Ok();
    }

    private static async Task<IResult> CreateInviteAsync(
        [FromBody] CreateInviteDTO input,
        IHttpContextAccessor _httpContext,
        ApplicationDbContext _context)
    {
        var inviteValidation = new CreateInviteValidation();
        var inputValidate = inviteValidation.Validate(input);

        if (!inputValidate.IsValid)
            return Results.BadRequest(inputValidate.Errors.FirstOrDefault()?.ErrorMessage);

        var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userId?.Value == null)
            return Results.Unauthorized();

        var userFound = await _context.Set<User>().FirstOrDefaultAsync(u => u.Id == new Guid(userId.Value));
        if (userFound == null)
            return Results.NotFound("Usuário não encontrado");

        var userMasterCampaign = await _context.Set<PlayerCampaign>()
            .FirstOrDefaultAsync(pc => pc.UserId == new Guid(userId.Value));

        if (userMasterCampaign == null)
            return Results.NotFound("Usuário não encontrado na campanha");

        if (userMasterCampaign.PlayerStatus != PlayerStatus.ADMIN)
            return Results.Forbid();

        var campaignFound = await _context.Set<Campaign>()
            .FirstOrDefaultAsync(c => c.Id == new Guid(input.campaignId));

        if (campaignFound == null)
            return Results.NotFound("Campanha não encontrada");

        var userInviteReceived = await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Name == input.userWantInviteUsername);

        if (userInviteReceived == null)
            return Results.NotFound("Usuário convidado não encontrado");

        var newInvite = Invite.Create(
            campaignFound.Name + " - Invite " + userInviteReceived.Name,
            userInviteReceived.Id,
            campaignFound.Id,
            userInviteReceived,
            campaignFound);

        await _context.Set<Invite>().AddAsync(newInvite.Data);
        await _context.SaveChangesAsync();

        return Results.Ok("Convite enviado com sucesso");
    }

    private static async Task<IResult> GetInvitesAsync(
        ApplicationDbContext _context,
        IHttpContextAccessor _httpContext)
    {
        var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userId?.Value == null)
            return Results.Unauthorized();

        var invites = await _context.Set<Invite>()
            .Where(i => i.UserId == new Guid(userId.Value) && !i.IsAccepted)
            .ToListAsync();

        return Results.Ok(invites);
    }

    private static async Task<IResult> AcceptInviteAsync(
        string inviteId,
        ApplicationDbContext _context,
        IHttpContextAccessor _httpContext)
    {
        var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userId?.Value == null)
            return Results.Unauthorized();

        if (string.IsNullOrEmpty(inviteId))
            return Results.NotFound("Convite não encontrado");

        var inviteFound = await _context.Set<Invite>()
            .Include(i => i.User)
            .Include(i => i.Campaign)
            .FirstOrDefaultAsync(i => i.Id == new Guid(inviteId));

        if (inviteFound == null)
            return Results.NotFound("Convite não encontrado");

        inviteFound.Accept();

        var newCampaignPlayer = PlayerCampaign.Create(
            inviteFound.User.Name,
            inviteFound.User.Id,
            inviteFound.Campaign.Id,
            inviteFound.User,
            inviteFound.Campaign);

        newCampaignPlayer.Data.SetCommon();

        await _context.Set<PlayerCampaign>().AddAsync(newCampaignPlayer.Data);
        await _context.SaveChangesAsync();

        return Results.Ok("Convite aceito com sucesso, a campanha aparecerá na sua lista de campanhas");
    }
}