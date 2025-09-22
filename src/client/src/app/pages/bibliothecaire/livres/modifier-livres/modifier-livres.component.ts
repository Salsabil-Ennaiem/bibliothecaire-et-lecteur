import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { etat_liv, Statut_liv, UpdateLivreDTO } from '../../../../model/livres.model';
import { LivreService } from '../../../../Services/livre.service';
import { MessageService } from 'primeng/api';
import { InputTextModule } from 'primeng/inputtext';
import { InputIconModule } from 'primeng/inputicon';
import { InputGroupModule } from 'primeng/inputgroup';

import { IconFieldModule } from 'primeng/iconfield';

@Component({
  selector: 'app-modifier-livres',
  imports: [SelectModule, FormsModule, ButtonModule, RouterLink , InputGroupModule,InputTextModule ,InputIconModule,IconFieldModule],
  templateUrl: './modifier-livres.component.html',
  styleUrl: './modifier-livres.component.css'
})
export class ModifierLivresComponent implements OnInit {
  selectStatutLiv: { label: string; value: Statut_liv }[] = [];
    selectetatLiv: { label: string; value: etat_liv }[] = [];

  livId!: string;
  Liv: UpdateLivreDTO = {
    statut: null!,
    etat: null ,
    cote_liv:''
  };

  constructor(private empserv: LivreService, private routter: Router, private route: ActivatedRoute, private messegeservice: MessageService) { }
  ngOnInit(): void {
    this.livId = this.route.snapshot.paramMap.get('id') ?? '';

    this.selectStatutLiv = [
      { label: 'Dosponible', value: Statut_liv.disponible },
      { label: 'Perdu', value: Statut_liv.perdu }
    ];
    
    this.selectetatLiv = [
      { label: 'Mauvais', value: etat_liv.mauvais },
      { label: 'Moyen', value: etat_liv.moyen }
    ];

    this.empserv.getById(this.livId).subscribe({
      next: (data) => {
        this.Liv = data;
      },
      error: (err) => console.error('Failed to load Livre', err),
    });
  }

  onSubmit(form: any): void {
    if (form.valid) {
      this.empserv.update(this.livId, this.Liv).subscribe({
        next: (updatedLivre) => {
          console.log('Updated successfully:', updatedLivre);
          this.messegeservice.add({ severity: 'success', summary: 'Succès', detail: 'Livre Modifier' });
          this.routter.navigate(['/bibliothecaire/livres']);

        },
        error: (err) => {
          console.error('Update failed', err);
          this.messegeservice.add({ severity: 'error', summary: 'Error', detail: 'Error modifier Livre:' });

        },
      });
    } else {
      form.control.markAllAsTouched();

    }
  }
}
