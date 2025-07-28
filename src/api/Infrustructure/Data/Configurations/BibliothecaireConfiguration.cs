using domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Data.Configurations;

public class BibliothecaireConfiguration : IEntityTypeConfiguration<Bibliothecaire>
{
    public void Configure(EntityTypeBuilder<Bibliothecaire> builder)
    {
        builder.ToTable("Bibliothecaires");

        builder.Property(e => e.nom)
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(e => e.prenom)
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(e => e.adresse)
            .HasMaxLength(100);


        builder.HasMany(e => e.Membres)
            .WithOne(m => m.Bibliothecaire)
            .HasForeignKey(m => m.id_biblio);

        builder.HasMany(e => e.Emprunts)
            .WithOne(e => e.Bibliothecaire)
            .HasForeignKey(e => e.id_biblio);

        builder.HasMany(e => e.Sanctions)
            .WithOne(s => s.Bibliothecaire)
            .HasForeignKey(s => s.id_biblio);

        builder.HasMany(e => e.Nouveautes)
            .WithOne(n => n.Bibliothecaire)
            .HasForeignKey(n => n.id_biblio);

        builder.HasMany(e => e.Parametres)
            .WithOne(p => p.Bibliothecaire)
            .HasForeignKey(p => p.IdBiblio);

        builder.HasMany(e => e.Inventaires)
            .WithOne(l => l.Bibliothecaire)
            .HasForeignKey(l => l.id_biblio)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(f => f.Fichier)
        .WithOne(b => b.Bibliothecaire)
        .HasForeignKey<Bibliothecaire>(f => f.Photo);

    }
}
