import { Component } from '@angular/core';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
//import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
//import { IftaLabelModule } from 'primeng/iftalabel';
//import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
//import { Textarea } from 'primeng/textarea';

import { RouterLink } from '@angular/router';
import { LivreService } from '../../../../Services/livre.service';
import { CreateLivreRequest, EtatLiv } from '../../../../model/Livres.model';


@Component({
  selector: 'app-ajout-livres',
  imports: [ RouterLink,IconFieldModule, InputIconModule, FormsModule, ButtonModule, SelectModule  ],
  templateUrl: './ajout-livres.component.html',
  styleUrl: './ajout-livres.component.css'
})
export class AjoutLivresComponent {
    livre: CreateLivreRequest |any;
  selectEtat_Livre: EtatLiv[] = [];
  
  constructor(private livreService: LivreService) { }

  Ajouter( livre: CreateLivreRequest):void {
      this.livreService.create( livre).subscribe({
        next :(data) => {
          this.livre=data;
          console.log('Livre added successfully',data);},
        error:(err) => {console.error('Error added livre:', err)}
  });
    }
  
}
