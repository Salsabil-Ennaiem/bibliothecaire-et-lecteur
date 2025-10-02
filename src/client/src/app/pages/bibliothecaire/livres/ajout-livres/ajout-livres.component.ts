import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { Router, RouterLink } from '@angular/router';
import { LivreService } from '../../../../Services/livre.service';
import { CreateLivreRequest, etat_liv } from '../../../../model/livres.model';
import { InputIcon } from 'primeng/inputicon';
import { IconField } from 'primeng/iconfield';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { MessageService } from 'primeng/api';
import { FileUploadModule } from 'primeng/fileupload';



@Component({
  selector: 'app-ajout-livres',
  imports: [RouterLink, CommonModule, InputIcon, ButtonModule, IconField, InputTextModule,
    FormsModule, ButtonModule, SelectModule, TextareaModule, FileUploadModule],
  templateUrl: './ajout-livres.component.html',
  styleUrl: './ajout-livres.component.css'
})
export class AjoutLivresComponent implements OnInit {
  langues: { label: string; value: string }[] = [];

  livre: CreateLivreRequest = {
    cote_liv: '',
    auteur: '',
    editeur: '',
    Langue: '',
    titre: '',
    isbn: '',
    inventaire: '',
    date_edition: '',
    etat: null,
    Description: ''
  };
  selectEtat_Livre: { label: string; value: etat_liv }[] = [];

  constructor(private livreService: LivreService, private http: HttpClient, private routr: Router, private messagesev: MessageService) { }

  ngOnInit(): void {
    this.http.get<{ name: string }[]>('langues.json').subscribe(data => {
      this.langues = data.map(lang => ({
        label: ` ${lang.name} `,
        value: lang.name
      }));
    });

    this.selectEtat_Livre = [
      { label: 'Neuf', value: etat_liv.neuf },
      { label: 'Moyen', value: etat_liv.moyen },
      { label: 'Mauvais', value: etat_liv.mauvais }
    ];
  }

  Ajouter(form: NgForm): void {
    if (form.valid) {
      this.livreService.create(this.livre).subscribe({
        next: () => {
          alert('Emprunt ajouté avec succès');
          this.messagesev.add({ severity: 'success', summary: 'Succès', detail: 'Livre ajouté' });
          this.routr.navigate(['/bibliothecaire/livres']);
        },
        error: (err) => {
          console.error('Error adding livre:', err);
          this.messagesev.add({ severity: 'error', summary: 'Error', detail: 'Error Ajout Livre:' });

        }
      });
    } else {
      this.messagesev.add({ severity: 'warn', summary: 'Formulaire invalide', detail: 'Veuillez remplir tous les champs requis' });
    }
  }
  coverFile: File | null = null;
  coverPreview: string | null = null;

  onSelectCover(event: any) {
    const file = event.files[0];
    if (file) {
      this.coverFile = file;
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.coverPreview = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  onClearCover() {
    this.coverFile = null;
    this.coverPreview = null;
  }


}
