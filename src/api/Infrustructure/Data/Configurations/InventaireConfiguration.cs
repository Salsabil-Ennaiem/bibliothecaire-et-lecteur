using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using domain.Entity;
using domain.Entity.Enum;

namespace Data.Configurations;

public class InventaireConfiguration : IEntityTypeConfiguration<Inventaire>
{
    public void Configure(EntityTypeBuilder<Inventaire> entity)
    {
        entity.ToTable("Inventaire");
        entity.HasKey(e => e.id_inv);

        entity.Property(e => e.id_inv)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.id_liv)
            .IsRequired();

        entity.Property(e => e.cote_liv)
            .HasMaxLength(50);

        entity.Property(e => e.etat)
            .HasConversion<string>()
            .HasDefaultValue(etat_liv.moyen)
            .IsRequired();

        entity.Property(e => e.statut)
            .HasConversion<string>()
            .HasDefaultValue(Statut_liv.disponible)
            .IsRequired();

        entity.HasMany(e => e.Emprunts)
            .WithOne(e => e.Inventaire)
            .HasForeignKey(e => e.Id_inv);
    }
}
