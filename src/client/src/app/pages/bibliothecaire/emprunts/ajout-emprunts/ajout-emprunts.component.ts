import { Component, OnInit } from '@angular/core';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { IftaLabelModule } from 'primeng/iftalabel';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { Textarea } from 'primeng/textarea';
import { CreateEmpRequest } from '../../../../model/emprunts.model';
import { TypeMemb } from '../../../../model/membre.model';
import { MembreService } from '../../../../Services/membre.service';
import { EmpruntService } from '../../../../Services/emprunt.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-ajout-emprunts',
  standalone: true,
  imports: [IconFieldModule, RouterLink, InputIconModule, InputTextModule, FormsModule, IftaLabelModule, CommonModule, ButtonModule, SelectModule, Textarea],
  templateUrl: './ajout-emprunts.component.html',
  styleUrl: './ajout-emprunts.component.css'

})
export class AjoutEmpruntsComponent implements OnInit {
  selecttypeMemb: { label: string; value: TypeMemb }[] = [];
  emprunt: CreateEmpRequest = {
    cin_ou_passeport: '',
    typeMembre: TypeMemb.Etudiant,
    nom: '',
    prenom: '',
    email: '',
    telephone: '',
    note: ''
  };

  memberExists: boolean | null = null;
  id: string = '';

  constructor(private memService: MembreService, private messagesev: MessageService, private empService: EmpruntService, private route: ActivatedRoute, private routr: Router) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.id = params['id'];
    });
    this.selecttypeMemb = [
      { label: 'Etudiant', value: TypeMemb.Etudiant },
      { label: 'Enseignant', value: TypeMemb.Enseignant },
      { label: 'Autre', value: TypeMemb.Autre }
    ];
  }
  isCinInvalid = true;

  onCinChange(value: string) {
    this.isCinInvalid = !(value && value.length >= 6);
    if (!this.isCinInvalid) {
      this.memService.search(value).subscribe(res => {
        if (res && res.length > 0) {
          this.emprunt.nom = res[0].nom;
          this.emprunt.prenom = res[0].prenom;
          this.emprunt.email = res[0].email;
          this.emprunt.telephone = res[0].telephone;
          this.emprunt.typeMembre = this.convertTypeMembre(res[0].typeMembre);
          this.memberExists = true;
        } else {
          this.memberExists = false;
        }
      });
    }

  }
  convertTypeMembre(type: number): TypeMemb {

    if (type == 0)
      return TypeMemb.Etudiant
    else if (type == 1)
      return TypeMemb.Enseignant
    else
      return TypeMemb.Autre
  }

  canSubmit(): boolean {
    return !!this.emprunt.cin_ou_passeport &&
      !!this.emprunt.nom &&
      !!this.emprunt.prenom &&
      !!this.emprunt.email &&
      this.emprunt.typeMembre !== null;
  }

  Ajouter(): void {
    if (this.canSubmit()) {
      this.empService.create(this.id, this.emprunt).subscribe({
        next: () => {
          alert('Emprunt ajouté avec succès');
          this.messagesev.add({ severity: 'success', summary: 'Succès', detail: 'Emprunt ajouté' });
          this.routr.navigate(['/bibliothecaire/emprunts']);
        },
        error: err => alert('Erreur lors de l\'ajout : ' + err.message)
      });
    } else {
      alert('Veuillez remplir tous les champs obligatoires.');
    }
  }




}