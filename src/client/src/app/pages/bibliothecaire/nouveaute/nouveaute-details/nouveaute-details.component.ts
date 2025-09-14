import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { NouveauteService } from '../../../../Services/nouveaute.service';
import { NouveauteDTO } from '../../../../model/nouveaute.model';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-nouveaute-details',
  imports: [CommonModule, ButtonModule],
  templateUrl: './nouveaute-details.component.html',
  styleUrl: './nouveaute-details.component.css'
})
export class NouveauteDetailsComponent implements OnInit {
    constructor(private nouvSer: NouveauteService, private route :ActivatedRoute) { }

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
    couverture: ''
  }

  getId(id: string) {
    this.nouvSer.getById(id).subscribe(
      {
        next: (data) => this.nouv = data,
        error: (err) => console.error('Erreur chargement nouveautés', err)
      }); 
  }
  collapsed = true;
  files = [
    { name: 'file1.pdf', url: 'file1.pdf' },
    { name: 'file2.pdf', url: 'file2.pdf' },
    { name: 'file3.pdf', url: 'file3.pdf' }
  ];

  toggleFiles() {
    this.collapsed = !this.collapsed;
  }
}
