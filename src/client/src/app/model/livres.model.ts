import { FichierDto } from "./fichier.model";

export enum Statut_liv {
  disponible ,
  emprunte ,
  perdu 
}

export enum etat_liv {
  neuf ,
  moyen ,
  mauvais 
}

export interface LivreDTO {
  id_inv: string;
  date_edition: string;
  titre: string;
  auteur?: string | null;

  isbn?: string | null;
  editeur: string;
  Description?: string | null;
  Langue?: string | null;

  couverture?: string | null;
  cote_liv: string;
  etat: etat_liv ;
  statut: Statut_liv;
  inventaire?: string | null;
}

export interface UpdateLivreDTO {
  date_edition?: string | null;
  titre?: string | null;
  auteur?: string | null;
  isbn?: string | null;

  editeur?: string | null;
  Description?: string | null;
  Langue?: string | null;
  couverture?: FichierDto | null;
  cote_liv?: string | null;
  etat?: etat_liv | null;
  statut: Statut_liv;
  inventaire?: string | null;
}

export interface CreateLivreRequest {
  cote_liv?: string | null;
  auteur?: string | null;
  editeur?: string | null;
  Langue?: string | null;

  titre?: string | null;
  isbn?: string | null;
  inventaire?: string | null;
  date_edition?: string | null;

  etat?: etat_liv | null;
  Description?: string | null;
  couverture?: FichierDto | null;
}