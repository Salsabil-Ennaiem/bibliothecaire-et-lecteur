import { CommonModule } from '@angular/common';
import { Component, ViewChild, ElementRef, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DrawerModule } from 'primeng/drawer';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { ProfileDTO, UpdateProfileDto } from '../../../model/bibliothecaire.model';
import { ProfileService } from '../../../Services/profile.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-profil',
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    DrawerModule,
    InputTextModule,
    TextareaModule
  ],
  templateUrl: './profil.component.html',
  styleUrls: ['./profil.component.css'],
})
export class ProfilComponent implements OnInit {

  @Input() visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  isEditing = false;

  profile: ProfileDTO = {
    id_biblio: '',
    nom: '',
    prenom: '',
    email: '',
    telephone: ''
  };

  uprofil: UpdateProfileDto = {
    nom: '',
    prenom: '',
    email: '',
    telephone: '',
    ancienMotDePasse: '',
    nouveauMotDePasse: ''
  };

  constructor(private ProfileServ: ProfileService, private messageService: MessageService) { }

  ngOnInit(): void {
    this.get();
  }

get(): void {
  this.ProfileServ.get().subscribe({
    next: (data) => {
      this.profile = data;
      this.uprofil = {
        nom: data.nom,
        prenom: data.prenom,
        email: data.email,
        telephone: data.telephone,
        ancienMotDePasse: '',
        nouveauMotDePasse: ''
      };
    },
    error: (error) => console.error('Error fetching profile:', error)
  });
}


  modifier(): void {
    if (this.isEditing) {
      this.ProfileServ.Modifier(this.uprofil).subscribe({
        next: (updatedProfile) => {
          this.messageService.add({ severity: 'success', summary: 'Succès', detail: 'Profil modifié' });
          this.profile = { ...updatedProfile };
          this.isEditing = false;
        },

        error: err => this.messageService.add({ severity: 'error', summary: 'Erreur', detail: 'Erreur lors de la modification : ' + err.message })
      });
    } else {
      this.messageService.add({ severity: 'warn', summary: 'Attention', detail: 'Veuillez passer en mode édition pour modifier.' });
    }
  }

  startEditing(): void {
    this.isEditing = true;
    this.uprofil = {
      nom: this.profile.nom,
      prenom: this.profile.prenom,
      email: this.profile.email,
      telephone: this.profile.telephone,
      ancienMotDePasse: '',
      nouveauMotDePasse: ''
    };
  }

cancelEditing(): void {
  this.isEditing = false;
  this.uprofil = {
    nom: this.profile.nom,
    prenom: this.profile.prenom,
    email: this.profile.email,
    telephone: this.profile.telephone,
    ancienMotDePasse: '',
    nouveauMotDePasse: ''
  };
}


  onDrawerHide(): void {
    this.visible = false;
    this.visibleChange.emit(false);
    this.cancelEditing();
  }
}
