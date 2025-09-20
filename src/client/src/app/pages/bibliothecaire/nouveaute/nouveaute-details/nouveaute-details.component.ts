import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { NouveauteService } from '../../../../Services/nouveaute.service';
import { NouveauteDTO } from '../../../../model/nouveaute.model';
import { ActivatedRoute } from '@angular/router';
import { FichierDto } from '../../../../model/fichier.model';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-nouveaute-details',
  imports: [CommonModule, ButtonModule],
  templateUrl: './nouveaute-details.component.html',
  styleUrl: './nouveaute-details.component.css'
})
export class NouveauteDetailsComponent implements OnInit {
    constructor(private nouvSer: NouveauteService, private route :ActivatedRoute,private msgserv:MessageService) { }

id: string | null = null;
  ngOnInit(): void {
      this.id = this.route.snapshot.paramMap.get('id');
if (this.id) {
    this.getId(this.id);
  } else {
    console.error('ID non trouvé dans la route');
  }
  }
  nouv: NouveauteDTO = {
    id_nouv: '',
    titre: '',
    fichier: '',
    description: '',
    date_publication: new Date,
 couverture: null,
  CouvertureFile: null,
  Fichiers: []
  };
toImageSrc(file: FichierDto | null | undefined): string | null {
  if (!file || !file.contenuFichier) {
    console.warn('No content for file', file);
    return null;
  }
  const base64String = btoa(String.fromCharCode(...new Uint8Array(file.contenuFichier)));
  return `data:${file.typeFichier ?? 'image/png'};base64,${base64String}`;
}


  getId(id: string) {
    this.nouvSer.getById(id).subscribe({
  next: (data) => {
    console.log('Nouveaute loaded:', data);
    this.nouv = data;
    console.log('CouvertureFile:', this.nouv.CouvertureFile);
  },
  error: (err) => {console.error('Erreur chargement nouveautés', err);
               this.msgserv.add({ severity: 'error', summary: 'Error', detail: 'Error detail Nouv:' });

  }
});

  }

  
  collapsed = true;
  

  toggleFiles() {
    this.collapsed = !this.collapsed;
  }
}
