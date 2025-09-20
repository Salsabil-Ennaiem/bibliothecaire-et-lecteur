import { Component, OnInit } from '@angular/core';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { IftaLabelModule } from 'primeng/iftalabel';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { MultiSelectModule } from 'primeng/multiselect'; import { Textarea } from 'primeng/textarea';
import { CreateSanctionRequest, Raison_sanction, SanctionDTO } from '../../../../model/sanction.model';
import { MessageService } from 'primeng/api';
import { SanctionService } from '../../../../Services/sanction.service';
import { DatePickerModule } from 'primeng/datepicker';
import { InputNumberModule } from 'primeng/inputnumber';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-ajout-sanction',
  imports: [IconFieldModule, InputIconModule, InputNumberModule, DatePickerModule, MultiSelectModule,
    InputTextModule, FormsModule, IftaLabelModule, CommonModule, ButtonModule, Textarea, RouterLink],
  templateUrl: './ajout-sanction.component.html',
  styleUrl: './ajout-sanction.component.css'
})
export class AjoutSanctionComponent implements OnInit {
  selectRaison: { label: string; value: Raison_sanction }[] = [];

  Sanction: CreateSanctionRequest = {
    id_emp: '',
    email: '',
    raison: [Raison_sanction.autre],
    date_fin_sanction: new Date,
    montant: 0,
    description: ''
  };
  id: string = '';
  minDate: Date;

  constructor(private messagesev: MessageService, private routr: Router, private route: ActivatedRoute, private sanctioserv: SanctionService) {
    const today = new Date();
    this.minDate = new Date(today.getFullYear(), today.getMonth(), today.getDate() + 1);
  }
  ngOnInit(): void {

    this.route.params.subscribe(params => {
      this.id = params['id'];
    });

    this.selectRaison = [
      { label: 'Retard', value: Raison_sanction.retard },
      { label: 'Perte', value: Raison_sanction.perte },
      { label: 'Degat', value: Raison_sanction.degat },
      { label: 'Autre', value: Raison_sanction.autre }
    ];

  }
  Annuler() {
    console.log("annuler ok");
  }
  canSubmit(): boolean {
    return !!this.Sanction.raison &&
      !!this.Sanction.date_fin_sanction;
  }
  Ajouter(): void {
    if (this.canSubmit()) {
      this.sanctioserv.create(this.Sanction, this.id).subscribe({
        next: () => {
          alert('Sanction ajouté avec succès');
          this.messagesev.add({ severity: 'success', summary: 'Succès', detail: 'Sanction ajouté' });
          this.routr.navigate(['/bibliothecaire/emprunts']);
        },
        error: err => alert('Erreur lors de l\'ajout : ' + err.message)
      });
    } else {
      alert('Veuillez remplir tous les champs obligatoires.');
    }
  }
}
