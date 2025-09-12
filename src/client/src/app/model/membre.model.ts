export enum TypeMemb {
    Etudiant ,
    Enseignant,
    Autre
}
export enum StatutMemb {
    actif,
    sanctionne,
    block 
}
export interface MembreDto {
  id_membre: string;
  typeMembre: TypeMemb;
  nom?: string | null;
  prenom?: string | null;
  email: string;
  telephone?: string | null;
  cin_ou_passeport: string;
  date_inscription: Date;
  statut: StatutMemb;
}

export interface UpdateMembreDto {
  typeMembre: TypeMemb;
  nom?: string | null;
  prenom?: string | null;
  email: string;
  telephone?: string | null;
  cin_ou_passeport: string;
}
