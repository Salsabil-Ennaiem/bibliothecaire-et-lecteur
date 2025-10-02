import { Component, OnInit } from '@angular/core';
import { TypeMemb, UpdateMembreDto } from '../../../../model/membre.model';
import { MembreService } from '../../../../Services/membre.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MessageService } from 'primeng/api';
import { FormsModule, NgForm } from '@angular/forms';
import { SelectModule } from 'primeng/select';
import { InputText } from "primeng/inputtext";
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-modifier-membre',
  imports: [SelectModule, FormsModule, RouterLink, CommonModule, InputText, ButtonModule],
  templateUrl: './modifier-membre.component.html',
  styleUrl: './modifier-membre.component.css'
})
export class ModifierMembreComponent implements OnInit {
  selecttypeMemb: { label: string; value: TypeMemb }[] = [];
  membreId!: string;
  membre: UpdateMembreDto = {
    typeMembre: null!,
    nom: null,
    prenom: null,
    email: '',
    telephone: null
  };

  constructor(private membServ: MembreService, private router: Router, private route: ActivatedRoute, private messegeservice: MessageService) { }
  ngOnInit(): void {
    this.membreId = this.route.snapshot.paramMap.get('id') ?? '';

    this.selecttypeMemb = [
      { label: 'Etudiant', value: TypeMemb.Etudiant },
      { label: 'Enseignant', value: TypeMemb.Enseignant },
      { label: 'Autre', value: TypeMemb.Autre }
    ];

    this.membServ.getById(this.membreId).subscribe({
      next: (data) => {
        this.membre = data;
      },
      error: (err) => console.error('Failed to load membre', err),
    });
  }

  onSubmit(form: NgForm): void {
    if (form.valid) {
      this.membServ.update(this.membreId, this.membre).subscribe({
        next: (updatedMembre) => {
          console.log('Updated successfully:', updatedMembre);
          this.messegeservice.add({ severity: 'success', summary: 'Succès', detail: 'Membre Modifier' });
          this.router.navigate(['/bibliothecaire/membres']);

        },
        error: (err) => {
          console.error('Update failed', err);
          this.messegeservice.add({ severity: 'error', summary: 'Error', detail: 'Error Membre Modifier' });
        },
      });
    } else {
      this.messegeservice.add({ severity: 'warn', summary: 'Formulaire invalide', detail: 'Veuillez remplir tous les champs ' });
    

    }
  }
}
