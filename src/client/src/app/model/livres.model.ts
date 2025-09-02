export enum Statut_liv {
  disponible = 'Disponible',
  emprunté = 'Emprunté',
  perdu = 'Perdu'
}

export enum etat_liv {
  neuf = 'Neuf',
  moyen = 'Moyen',
  mauvais = 'Mauvais'
}

export interface LivreDTO {
  id_inv: string;
  date_edition: string;
  titre: string;
  auteur?: string;

  isbn?: string;
  editeur: string;
  Description?: string;
  Langue?: string;

  couverture?: string;
  cote_liv: string;
  etat?: etat_liv;
  statut: Statut_liv;

  inventaire?: string;
}
export interface UpdateLivreDTO {
  date_edition: string;
  titre: string;
  auteur?: string;
  isbn?: string;

  editeur: string;
  Description?: string;
  Langue?: string;
  couverture?: string;

  cote_liv: string;
  etat?: etat_liv;
  statut?: Statut_liv;
  inventaire?: string;
}


export interface CreateLivreRequest {
  cote_liv: string;
  auteur: string;
  editeur: string;
  Langue: string;

  titre: string;
  isbn: string;
  inventaire: string;
  date_edition: string;


  etat: etat_liv;
  Description: string;
  couverture: string
}


