using domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class NouveauteConfiguration : IEntityTypeConfiguration<Nouveaute>
{
    public void Configure(EntityTypeBuilder<Nouveaute> entity)
    {
        entity.ToTable("Nouveaute");
        entity.HasKey(e => e.id_nouv);

        entity.Property(e => e.id_nouv)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.titre)
            .IsRequired();

                entity.Property(e => e.description)
                    .HasColumnType("text");
                    
        entity.Property(e => e.date_publication)
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");


        entity.HasMany(e => e.Fichiers)
            .WithOne(l => l.ficherNouv)
            .HasForeignKey(e => e.NouveauteId);

        entity.HasOne(f => f.Couvertures)
        .WithOne(m => m.couvertureNouv)
        .HasForeignKey<Nouveaute>(k=>k.couverture);
            
    }
}
