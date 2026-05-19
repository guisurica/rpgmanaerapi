using rpgmanagerapi.Common;

namespace rpgmanagerapi.Models;

public sealed class PlayerCampaign : BaseModel
{
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public Guid CampaignId { get; private set; }
    public Campaign Campaign  { get; private set; }
    public PlayerStatus PlayerStatus { get; private set; }

    private PlayerCampaign() : base(string.Empty) { }

    private PlayerCampaign(string name, Guid userId, Guid campaignId, User user, Campaign campaign) : base(name)
    {
        UserId = userId;
        CampaignId = campaignId;
        User = user;
        Campaign = campaign;
    }

    public static Result<PlayerCampaign> Create(string name, Guid userId, Guid campaignId, User user, Campaign campaign)
    {
        var playerCampaign = new PlayerCampaign(name, userId, campaignId, user, campaign);

        return Result<PlayerCampaign>.Success("Player creatd successfully", 200, playerCampaign);
    }
}


public enum PlayerStatus
{
    ADMIN,
    COMMON
}