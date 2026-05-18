namespace rpgmanagerapi.Models;

public sealed class PlayerCampaign : BaseModel
{
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public Guid CampaignId { get; private set; }
    public Campaign Campaign  { get; private set; }
    public PlayerStatus PlayerStatus { get; private set; }

    private PlayerCampaign() : base(string.Empty) { }
}


public enum PlayerStatus
{
    ADMIN,
    COMMON
}