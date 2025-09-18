import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DrawerModule } from 'primeng/drawer';
import { SliderModule } from 'primeng/slider';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { TextareaModule } from 'primeng/textarea';
import { MessageService } from 'primeng/api';

import { ParametreService } from '../../../Services/parametre.service';
import { ParametreDTO, UpdateParametreDTO } from '../../../model/parametre.model';

@Component({
  selector: 'app-parametre',
  standalone: true,
  imports: [
    TextareaModule,
    InputTextModule,
    SliderModule,
    TagModule,
    DrawerModule,
    FormsModule,
    CommonModule,
    ButtonModule,
  ],
  templateUrl: './parametre.component.html',
  styleUrls: ['./parametre.component.css']
})
export class ParametreComponent implements OnInit {

  @Input() visible = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  isEditing = false;

  parmtre: ParametreDTO = {
    delais_Emprunt_Autre: 0,
    delais_Emprunt_Enseignant: 0,
    delais_Emprunt_Etudiant: 0,
    modele_Email_Retard: ''
  };

  uparam: UpdateParametreDTO = {
    delais_Emprunt_Autre: 0,
    delais_Emprunt_Enseignant: 0,
    delais_Emprunt_Etudiant: 0,
    modele_Email_Retard: ''
  };

  constructor(private paramServ: ParametreService, private messageService: MessageService) { }

  ngOnInit(): void {
    this.get();
  }

  get(): void {
    this.paramServ.getById().subscribe({
      next: (data) => {
        this.parmtre = data;
        this.uparam = { ...data }; // copy after fetch to avoid stale/empty values
      },
      error: (error) => console.error('Error fetching Parametre:', error)
    });

  }

  modifier(): void {
    if (this.isEditing) {
      this.paramServ.modifier(this.uparam).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Succès', detail: 'Paramètre modifié' });
          this.parmtre = { ...this.uparam };
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
    this.uparam = { ...this.parmtre }; // reset editing copy
  }

  cancelEditing(): void {
    this.isEditing = false;
        this.uparam = { ...this.parmtre }; // reset editing copy

  }

  onDrawerHide(): void {
    this.visible = false;
    this.visibleChange.emit(false);
    this.cancelEditing();
  }
}
