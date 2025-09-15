import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { RouterLink } from '@angular/router';
import { LivreService } from '../../../../Services/livre.service';
import { CreateLivreRequest, etat_liv } from '../../../../model/livres.model';
import { InputIcon } from 'primeng/inputicon';
import { IconField } from 'primeng/iconfield';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { HttpClient } from '@angular/common/http';



@Component({
  selector: 'app-ajout-livres',
  imports: [RouterLink, InputIcon,ButtonModule,IconField, InputTextModule, FormsModule, ButtonModule, SelectModule, TextareaModule],
  templateUrl: './ajout-livres.component.html',
  styleUrl: './ajout-livres.component.css'
})
export class AjoutLivresComponent implements OnInit {
  langues: { label: string; value: string }[] = [];

  livre!: CreateLivreRequest;
  selectEtat_Livre: { label: string; value: etat_liv }[] = [];

  constructor(private livreService: LivreService, private http: HttpClient) {}

  ngOnInit(): void {
    this.livre = {
      cote_liv: '',
      auteur: '',
      editeur: '',
      Langue: '',
      titre: '',
      isbn: '',
      inventaire: '',
      date_edition: '',
      etat: etat_liv.neuf,
      Description: ''
    };
this.http.get<{code: string, name: string, native: string}[]>('langues.json').subscribe(data => {
  this.langues = data.map(lang => ({
    label: `${lang.name} (${lang.native})`,  
    value: lang.code                        
  }));
});




    this.selectEtat_Livre = [
      { label: 'Neuf', value: etat_liv.neuf },
      { label: 'Moyen', value: etat_liv.moyen },
      { label: 'Mauvais', value: etat_liv.mauvais }
    ];
  }

  Ajouter(livre: CreateLivreRequest): void {
    this.livreService.create(livre).subscribe({
      next: (data) => {
        this.livre = {
          cote_liv: '',
          auteur: '',
          editeur: '',
          Langue: '',
          titre: '',
          isbn: '',
          inventaire: '',
          date_edition: '',
          etat: etat_liv.neuf,
          Description: ''
        };
        console.log('Livre added successfully', data);
      },
      error: (err) => {
        console.error('Error adding livre:', err);
      }
    });
  }
}
