import { FichierDto } from "./fichier.model";

export interface NouveauteDTO {
    id_nouv?: string | null;
    titre: string ;
    fichier?: string | null;
    description?: string | null;
    date_publication: Date;
    couverture?: string | null;
    CouvertureFile?: FichierDto | null
    Fichiers: FichierDto[] | null

}

export interface NouveauteGetALL {
    id_nouv?: string | null;
    date_publication: Date;
    couverture?: string | null;
    titre: string ;
    CouvertureFile?: FichierDto | null

}

export interface CreateNouveauteRequestWithFiles {
    titre: string;
    description?: string | null;
    Couv?: FichierDto | null;
    File?: FichierDto[] | null;
}