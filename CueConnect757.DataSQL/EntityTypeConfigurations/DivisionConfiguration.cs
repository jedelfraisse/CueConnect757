using CueConnect757.DataSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CueConnect757.DataSQL.EntityTypeConfigurations;

public class DivisionConfiguration : IEntityTypeConfiguration<Division>
{
    public void Configure(EntityTypeBuilder<Division> builder)
    {
        builder.ToTable("Divisions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DivisionIdFromAPA).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.NightOfPlay).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Format).HasMaxLength(50).IsRequired();

        builder.HasOne(x => x.Session)
            .WithMany(x => x.Divisions)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Teams)
            .WithOne(x => x.Division)
            .HasForeignKey(x => x.DivisionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Matches)
            .WithOne(x => x.Division)
            .HasForeignKey(x => x.DivisionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}