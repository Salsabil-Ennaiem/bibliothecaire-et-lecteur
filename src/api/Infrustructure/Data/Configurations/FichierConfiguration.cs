using domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class FichierConfiguration : IEntityTypeConfiguration<Fichier>
{
    public void Configure(EntityTypeBuilder<Fichier> entity)
    {
        entity.ToTable("Fichier");
        entity.HasKey(e => e.IdFichier);

        entity.Property("ContentHash").IsRequired();
        
        entity.HasIndex(f => f.ContentHash)
            .IsUnique();

        entity.Property(e => e.IdFichier)
            .ValueGeneratedOnAdd();

    }
}
