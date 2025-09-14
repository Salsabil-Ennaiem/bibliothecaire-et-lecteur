
export enum Raison_sanction
    {
       retard,
       perte,
       degat , 
       autre
    }


export interface SanctionDTO {
  id_sanc: string;
  id_membre: string;
  id_emp?: string | null;
  email?: string | null;
  date_emp: Date;
  raison: Raison_sanction[];
  date_sanction: Date;
  date_fin_sanction?: Date | null;
  montant?: number | null;
  payement?: boolean | null;
  active: boolean;
  description?: string | null;
}

export interface CreateSanctionRequest {
  email?: string | null;
  id_emp: string;
  raison: Raison_sanction[];
  date_fin_sanction?: Date | null;
  montant?: number | null;
  description?: string | null;
}