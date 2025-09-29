import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { FileUploadModule } from 'primeng/fileupload';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { CreateNouveauteRequestWithFiles } from '../../../../model/nouveaute.model';
import { NouveauteService } from '../../../../Services/nouveaute.service';
import { MessageService } from 'primeng/api';
import { Router, RouterLink } from '@angular/router';
@Component({
  selector: 'app-ajouter-nouveaute',
  imports: [CommonModule, FormsModule,
    ButtonModule, FileUploadModule,
    InputTextModule, TextareaModule, RouterLink],
  templateUrl: './ajouter-nouveaute.component.html',
  styleUrl: './ajouter-nouveaute.component.css'
})
export class AjouterNouveauteComponent {
  formData: CreateNouveauteRequestWithFiles = {
    titre: '',
    description: null,
    Couv: null,
    File: [],
  };
  constructor(private novServ: NouveauteService, private messagesev: MessageService, private routr: Router) { }
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
  this.formData.Couv = null;
}

  uploadedFiles: File[] = [];

  onSelectFiles(event: any) {
    this.uploadedFiles = this.uploadedFiles.concat(event.files);
  }

  onClearFiles() {
    this.uploadedFiles = [];
  }

  onCancel() {
    this.formData = { titre: '', description: null, Couv: null, File: [] };
    this.coverFile = null;
    this.coverPreview = null;
    this.uploadedFiles = [];
  }

  base64ToByteArray(base64: string): Uint8Array {
    const binaryString = window.atob(base64);
    const len = binaryString.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
      bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes;
  }


  
  // Similarly for multiple files
  convertFilesToDto() {
    this.formData.File = [];
    const convertPromises = this.uploadedFiles.map(file => new Promise<void>((resolve) => {
      const reader = new FileReader();
      reader.onload = e => {
        const content = (e.target as FileReader).result as string;
        this.formData.File!.push({
          idFichier: '',
          nomFichier: file.name,
          cheminFichier: null,
          typeFichier: file.type,
          contenuFichier: this.base64ToByteArray(content.split(',')[1]),
          contentHash: '',
          tailleFichier: file.size,
          dateCreation: new Date(),
          nouveauteId: null
        });
        resolve();
      };
      reader.readAsDataURL(file);
    }));
    return Promise.all(convertPromises);
  }


// Convert File to Base64 string for DTO
convertCoverToDto(): Promise<void> {
  return new Promise((resolve, reject) => {
    if (!this.coverFile) {
      this.formData.Couv = null;
      resolve();
      return;
    }
    const reader = new FileReader();
    reader.onload = (e) => {
      const content = (e.target as FileReader).result as string; // "data:<type>;base64,..."
      this.formData.Couv = {
        idFichier: '',
        nomFichier: this.coverFile!.name,
        cheminFichier: null,
        typeFichier: this.coverFile!.type,
          contenuFichier: this.base64ToByteArray(content.split(',')[1]),
        contentHash: '',
        tailleFichier: this.coverFile!.size,
        dateCreation: new Date(),
        nouveauteId: null,
      };
      resolve();
    };
    reader.onerror = (err) => reject(err);
    reader.readAsDataURL(this.coverFile);
  });
}

async Ajouter(): Promise<void> {
  if (!!this.formData.titre) {
    try {
      await this.convertCoverToDto();
      this.novServ.create(this.formData).subscribe({
        next: () => {
          this.messagesev.add({ severity: 'success', summary: 'Succès', detail: 'Emprunt ajouté' });
          this.routr.navigate(['/bibliothecaire/nouveaute']);
        },
        error: (err) => {
          const errorMessage = err.error?.message || err.message || 'Erreur inconnue';
          this.messagesev.add({ severity: 'error', summary: 'Erreur', detail: errorMessage });
        }
      });
    } catch (err) {
      this.messagesev.add({ severity: 'error', summary: 'Erreur', detail: 'Erreur lors de la préparation du fichier couverture' });
    }
  } else {
    this.messagesev.add({ severity: 'warn', summary: 'Attention', detail: 'Veuillez remplir tous les champs obligatoires.' });
  }
}
}

