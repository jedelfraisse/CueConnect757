using CueConnect757.DataSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CueConnect757.DataSQL;

public class CueConnectDbContext : DbContext
{
    public CueConnectDbContext(DbContextOptions<CueConnectDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Division> Divisions => Set<Division>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<PlayerSessionStats> PlayerSessionStats => Set<PlayerSessionStats>();
    public DbSet<MembershipHistory> MembershipHistories => Set<MembershipHistory>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CueConnectDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
