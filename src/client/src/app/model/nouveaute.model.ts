import { FichierDto } from "./fichier.model";

export interface NouveauteDTO {
    id_nouv?: string | null;
    titre?: string | null;
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
    titre?: string | null;
    CouvertureFile?: FichierDto | null

}

export interface CreateNouveauteRequestWithFiles {
    titre?: string | null;
    description?: string | null;
    Couv?: FichierDto | null;
    File?: FichierDto[] | null;
}