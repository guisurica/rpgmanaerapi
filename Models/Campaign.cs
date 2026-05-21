using rpgmanagerapi.Common;

namespace rpgmanagerapi.Models;

public sealed class Campaign : BaseModel
{
    public string Description { get; private set; }
    public string GameSystem { get; private set; }
    public Guid CampaignMaster { get; private set; }
    public List<Report> Reports { get; private set; }

    private Campaign() : base(string.Empty){ }

    private Campaign(string name, string description, string system, Guid cm) : base(name)
    {
        Name = name;
        Description = description;
        GameSystem = system;
        CampaignMaster = cm;
        Reports = new List<Report>();
    }


    public static Result<Campaign> CreateCampaign(string name, string description, string system, Guid cm)
    {
        var newCampaign = new Campaign(name, description, system, cm);

        return Result<Campaign>.Success("Campaign created successfully", 201, newCampaign);
    }

    public void UpdateCampaign(string name, string description)
    {
        this.Name = name;
        this.Description = description;
    }
}