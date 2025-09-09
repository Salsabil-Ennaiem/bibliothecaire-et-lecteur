export interface NouveauteDTO {
    id_nouv: string | null;
    titre: string | null;
    // fichier: { [key: string]: any; } ;
    fichier: string | null;
    description: string | null;
    date_publication: string;
    couverture: string | null;
}

export interface NouveauteGetALL {
    id_nouv: string;
    date_publication: string;
    couverture: string | null;
    titre: string | null;
}


export interface CreateNouveauteRequest {
    titre: string | null;
    description: string | null;
    fichier: string | null;

    couverture: string;
}

