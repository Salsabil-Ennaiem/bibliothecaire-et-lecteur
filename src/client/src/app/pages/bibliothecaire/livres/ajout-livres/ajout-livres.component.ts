import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { RouterLink } from '@angular/router';
import { LivreService } from '../../../../Services/livre.service';
import { CreateLivreRequest, EtatLiv } from '../../../../model/livres.model';
import { InputIcon } from 'primeng/inputicon';
import { IconField } from 'primeng/iconfield';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';



@Component({
  selector: 'app-ajout-livres',
  imports: [ RouterLink,InputIcon, IconField, InputTextModule, FormsModule, ButtonModule, SelectModule , TextareaModule  ],
  templateUrl: './ajout-livres.component.html',
  styleUrl: './ajout-livres.component.css'
})
export class AjoutLivresComponent implements OnInit {
     livre! : CreateLivreRequest ;// Le ! indique à TypeScript qu'on garantit l'initialisation
  //selectEtat_Livre: EtatLiv[] = [];
    selectEtat_Livre: { label: string, value: EtatLiv }[] = [];

  
  constructor(private livreService: LivreService) { }

ngOnInit(): void {
  this.livre= {
      cote_liv: '',
      auteur: '',
      editeur: '',
      Langue: '',
      titre: '',
      isbn: '',
      inventaire: '',
      date_edition: '',
      etat: EtatLiv.Neuf, 
      Description: '',
      couverture: ''
    };
      // Initialize the dropdown options
    this.selectEtat_Livre = [
      { label: 'Neuf', value: EtatLiv.Neuf },
      { label: 'Moyen', value: EtatLiv.Moyen },
      { label: 'Mauvais', value: EtatLiv.Mauvais }
    ];
  }
  Ajouter( livre: CreateLivreRequest):void {
      this.livreService.create(livre).subscribe({
        next :(data) => {
           this.livre = {
          cote_liv: '',
          auteur: '',
          editeur: '',
          Langue: '',
          titre: '',
          isbn: '',
          inventaire: '',
          date_edition: '',
          etat: EtatLiv.Neuf,
          Description: '',
          couverture: ''
        };
      
          console.log('Livre added successfully',data);},
        error:(err) => {console.error('Error added livre:', err)}
  });
    }
  /*
 Ajout - Edit :Formulaire
  @Component({
  selector: 'app-livre-form',
  template: `
    <form [formGroup]="livreForm" (ngSubmit)="onSubmit()">
      <input formControlName="titre" placeholder="Titre" required />
      <input formControlName="auteur" placeholder="Auteur" required />
      <button type="submit">{{ isEdit ? 'Modifier' : 'Ajouter' }}</button>
    </form>
  `
})
export class LivreFormComponent implements OnInit {
  @Input() livreInitial: Livre | null = null; // null pour ajout, objet pour modification
  livreForm: FormGroup;
  isEdit = false;

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.isEdit = !!this.livreInitial;
    this.livreForm = this.fb.group({
      titre: [this.livreInitial?.titre || '', Validators.required],
      auteur: [this.livreInitial?.auteur || '', Validators.required]
    });
  }

  onSubmit() {
    if (this.livreForm.valid) {
      if (this.isEdit) {
        // appeler API modifier
      } else {
        // appeler API ajouter
      }
    }
  }
}
*/
}
