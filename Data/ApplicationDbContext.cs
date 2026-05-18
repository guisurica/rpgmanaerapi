using Microsoft.EntityFrameworkCore;
using rpgmanagerapi.Models;

namespace rpgmanagerapi.Data;

public sealed class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<PlayerCampaign> PlayerCampaigns { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campaign>()
            .ToTable("campaigns");
        modelBuilder.Entity<User>()
            .ToTable("users");
        modelBuilder.Entity<PlayerCampaign>()
            .ToTable("user_campaign");

        base.OnModelCreating(modelBuilder);
    }
} 