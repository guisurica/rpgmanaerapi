using rpgmanagerapi.Common;

namespace rpgmanagerapi.Models;

public sealed class Report : BaseModel
{
    public string Text { get; private set; }
    public List<Guid> ReadBy { get; private set; }
    public Guid CampaignId { get; private set; }
    public Campaign Campaign { get; private set; }

    private Report() : base(string.Empty) { }

    private Report(string name, string text, string campaignId, Campaign campaign) : base(name)
    {
        Text = text;
        ReadBy = new List<Guid>();
        CampaignId = new Guid(campaignId);
        Campaign = campaign;
    }

    public static Result<Report> Create(string name, string text, string campaignId, Campaign campaign)
    {
        var newReport = new Report(name, text, campaignId, campaign);

        return Result<Report>.Success("Evento criado com sucesso", 200, newReport);
    }

    public void MarkAsRead(Guid pcId)
    {
        if (!ReadBy.Contains(pcId))
        {
            ReadBy.Add(pcId);
        }
    }


}