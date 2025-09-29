export interface FichierDto {
  idFichier: string;
  nomFichier?: string | null;
  cheminFichier?: string | null;
  typeFichier?: string | null;
  contenuFichier?:  Uint8Array | null;
  contentHash?: string;
  tailleFichier: number;
  dateCreation: Date;
  nouveauteId?: string | null;
}
