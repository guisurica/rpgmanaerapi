namespace rpgmanagerapi.Data.DTOs;

public record PlayerCampaignDTO(
    string campaignName,
    string campaignDescription,
    string campaignSystem,
    string masterName,
    DateTime CreatedOnUtc,
    string campaignId
);