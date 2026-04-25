using CueConnect757.DataSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CueConnect757.DataSQL.EntityTypeConfigurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.ToTable("Matches");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MatchIdFromAPA).IsRequired();
        builder.Property(x => x.MatchDate).IsRequired();

        builder.HasOne(x => x.Division)
            .WithMany(x => x.Matches)
            .HasForeignKey(x => x.DivisionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.HomeTeam)
            .WithMany(x => x.HomeMatches)
            .HasForeignKey(x => x.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AwayTeam)
            .WithMany(x => x.AwayMatches)
            .HasForeignKey(x => x.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}