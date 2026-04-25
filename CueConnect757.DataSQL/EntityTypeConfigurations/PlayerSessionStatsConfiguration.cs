using CueConnect757.DataSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CueConnect757.DataSQL.EntityTypeConfigurations;

public class PlayerSessionStatsConfiguration : IEntityTypeConfiguration<PlayerSessionStats>
{
    public void Configure(EntityTypeBuilder<PlayerSessionStats> builder)
    {
        builder.ToTable("PlayerSessionStats");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MatchesPlayed).IsRequired();
        builder.Property(x => x.MatchesWon).IsRequired();
        builder.Property(x => x.BreakAndRuns).IsRequired();
        builder.Property(x => x.Rackless).IsRequired();

        builder.HasOne(x => x.Player)
            .WithMany(x => x.SessionStats)
            .HasForeignKey(x => x.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Session)
            .WithMany(x => x.PlayerSessionStats)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}