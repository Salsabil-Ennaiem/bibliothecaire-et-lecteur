
export enum TypeMemb {
    Etudiant = "Etudiant",
    Enseignant = "Enseignant",
    Autre = "Autre"
}
export enum StatutMemb {
    actif = "actif",
    sanctionne = "sanctionne",
    block = 'block'
}
export enum Statut_emp {
    en_cours = "en_cours",
    retourne = "retourne",
    perdu = "perdu"
}
export interface EmppruntDTO {
    id_emp: string;
    date_emp: Date;
    id_inv: string;
    date_edition: string;
    titre: string;
    editeur: string;
    cote_liv: string;
    date_retour_prevu: Date;
    date_effectif: Date | null;
    Statut_emp: Statut_emp;
    note: string | null;
    TypeMembre: TypeMemb;
    nom: string | null;
    prenom: string | null;
    email: string | null;
    telephone: string | null;
    cin_ou_passeport: string;
    Statut: StatutMemb;
}

export interface UpdateEmppruntDTO {
    Statut_emp: Statut_emp | null;
    note: string | null;
}

export interface CreateEmpRequest {
    note: string | null;
    cote_liv: string;
    TypeMembre: TypeMemb;
    nom: string | null;
    prenom: string | null;
    email: string | null;
    telephone: string | null;
    cin_ou_passeport: string | null;
}