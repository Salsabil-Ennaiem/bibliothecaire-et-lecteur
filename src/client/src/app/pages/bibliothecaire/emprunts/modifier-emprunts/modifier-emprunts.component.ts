import { Component, OnInit } from '@angular/core';
import { SelectModule } from 'primeng/select';
import { Statut_emp, UpdateEmppruntDTO } from '../../../../model/emprunts.model';
import { EmpruntService } from '../../../../Services/emprunt.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MessageService } from 'primeng/api';
import { FormsModule } from '@angular/forms';
import { TextareaModule } from 'primeng/textarea';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-modifier-emprunts',
  imports: [SelectModule, FormsModule, TextareaModule, ButtonModule, RouterLink],
  templateUrl: './modifier-emprunts.component.html',
  styleUrl: './modifier-emprunts.component.css'
})
export class ModifierEmpruntsComponent implements OnInit {
  selectstatutEmp: { label: string; value: Statut_emp }[] = [];
  empId!: string;
  Emp: UpdateEmppruntDTO = {
    statut_emp: null!,
    note: ''
  };

  constructor(private empserv: EmpruntService, private routter: Router, private route: ActivatedRoute, private messegeservice: MessageService) { }
  ngOnInit(): void {
    this.empId = this.route.snapshot.paramMap.get('id') ?? '';

    this.selectstatutEmp = [
      { label: 'En Cours', value: Statut_emp.en_cours },
      { label: 'Perdu', value: Statut_emp.perdu },
      { label: 'Retourne', value: Statut_emp.retourne }
    ];

    this.empserv.getById(this.empId).subscribe({
      next: (data) => {
        this.Emp = data;
      },
      error: (err) => console.error('Failed to load Emprunts', err),
    });
  }

  onSubmit(form: any): void {
    if (form.valid) {
      this.empserv.update(this.empId, this.Emp).subscribe({
        next: (updatedEmp) => {
          console.log('Updated successfully:', updatedEmp);
          this.messegeservice.add({ severity: 'success', summary: 'Succès', detail: 'Emprunts Modifier' });
          this.routter.navigate(['/bibliothecaire/emprunts']);
        },
        error: (err) => {
          console.error('Update failed', err);
          this.messegeservice.add({ severity: 'error', summary: 'Error', detail: 'Error modifier Emprunts:' });

        },
      });
    } else {
      form.control.markAllAsTouched();

    }
  }
}
