using rpgmanagerapi.Common;

namespace rpgmanagerapi.Models;

public sealed class Invite : BaseModel
{
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public Guid CampaignId { get; private set; }
    public Campaign Campaign { get; private set; }

    public bool IsAccepted { get; private set; } = false;

    private Invite() : base(string.Empty) { }

    private Invite(string name, Guid userId, Guid campaignId, User user, Campaign campaign) : base(name)
    {
        UserId = userId;
        CampaignId = campaignId;
        User = user;
        Campaign = campaign;
    }

    public static Result<Invite> Create(string name, Guid userId, Guid campaignId, User user, Campaign campaign)
    {
        var newInvite = new Invite(name, userId, campaignId, user, campaign);

        return Result<Invite>.Success("Invite created successfully", 200, newInvite);
    }

    public void Accept()
    {
        this.IsAccepted = true;
    }

}