using CueConnect757.DataSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CueConnect757.DataSQL.EntityTypeConfigurations;

public class MembershipHistoryConfiguration : IEntityTypeConfiguration<MembershipHistory>
{
    public void Configure(EntityTypeBuilder<MembershipHistory> builder)
    {
        builder.ToTable("MembershipHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Year).IsRequired();
        builder.Property(x => x.LeagueName).HasMaxLength(150).IsRequired();

        builder.HasOne(x => x.Player)
            .WithMany(x => x.MembershipHistory)
            .HasForeignKey(x => x.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}