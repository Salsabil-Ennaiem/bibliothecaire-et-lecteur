using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using domain.Entity;


namespace Data.Configurations;
public class LivreConfiguration : IEntityTypeConfiguration<Livres>
{
    public void Configure(EntityTypeBuilder<Livres> entity)
    {
        entity.ToTable("Livres");
        entity.HasKey(e => e.id_livre);

        entity.Property(e => e.id_livre)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.editeur)
            .HasMaxLength(100);

        entity.Property(e => e.date_edition)
            .HasMaxLength(10);

        entity.Property(e => e.isbn)
            .HasMaxLength(18);

        entity.Property(e => e.auteur)
            .HasMaxLength(100);

        entity.Property(e => e.Description)
     .HasColumnType("text");


        entity.HasOne(e => e.Fichiers)
            .WithOne(b => b.Livre)
            .HasForeignKey<Livres>(e => e.couverture);

        entity.HasMany(e => e.Inventaires)
            .WithOne(i => i.Livre)
            .HasForeignKey(i => i.id_liv);
    }
}
