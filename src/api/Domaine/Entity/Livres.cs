﻿using Domaine.Entity;

namespace domain.Entity;

public class Livres
{
    public string? id_livre { get; set; }
    public required string date_edition { get; set; }
    public required string titre { get; set; }
    public string? auteur { get; set; }
    public string? isbn { get; set; }
    public required string editeur { get; set; }
    public string? Description { get; set; }
    public string? Langue { get; set; }
    public string? couverture { get; set; }
    public virtual ICollection<Inventaire>? Inventaires { get; set; }
    public virtual Fichier? Fichiers { get; set; }

    }








