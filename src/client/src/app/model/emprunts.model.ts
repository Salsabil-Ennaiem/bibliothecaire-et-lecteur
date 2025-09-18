import { StatutMemb, TypeMemb } from "./membre.model";

export enum Statut_emp {
    en_cours ,
    retourne ,
    perdu 
}
export interface EmppruntDTO {
  id_emp: string;
  editeur: string;
  cote_liv: string;
  id_inv: string;
  date_edition: string;
  titre: string;
  date_emp: Date;
  date_retour_prevu: Date;
  date_effectif?: Date | null;
  statut_emp: Statut_emp;
  note?: string | null;
  typeMembre: TypeMemb;
  nom?: string | null;
  prenom?: string | null;
  email: string;
  telephone?: string | null;
  cin_ou_passeport: string;
  statut: StatutMemb;
}

export interface UpdateEmppruntDTO {
  statut_emp: Statut_emp;
  note?: string | null;
}
 
export interface CreateEmpRequest {
  note?: string | null;
  typeMembre: TypeMemb;
  nom?: string | null;
  prenom?: string | null;
  email?: string | null;
  telephone?: string | null;
  cin_ou_passeport: string ;
}
