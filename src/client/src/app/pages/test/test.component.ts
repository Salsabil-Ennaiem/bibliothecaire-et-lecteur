import {  Component, OnDestroy, OnInit} from '@angular/core';
import { EmppruntDTO, Statut_emp } from '../../model/emprunts.model';
import { CommonModule } from '@angular/common';
import { EmpruntService } from '../../Services/emprunt.service';
import { StatutMemb, TypeMemb } from '../../model/membre.model';
const statutLabels = {
  0: 'en_cours',
  1: 'return',
  2: 'perdu',
};
@Component({
  selector: 'app-test',
  imports: [CommonModule],
  templateUrl: './test.component.html',
  styleUrl: './test.component.css'
})

export class TestComponent implements OnInit ,OnDestroy{
  emprunts: EmppruntDTO[] = [];
  flipped: boolean[] = [];
  emprunt: EmppruntDTO ={
    id_emp: '',
    editeur: '',
    cote_liv: '',
    id_inv: '',
    date_edition:'',
    titre: '',
    date_emp: new Date(),
    date_retour_prevu: new Date(),
    date_effectif: new Date(),
    statut_emp: Statut_emp.en_cours,
    note: '',
    typeMembre: TypeMemb.Autre,
    nom: '',
    prenom: '',
    email: '',
    telephone: '',
    cin_ou_passeport: '',
    statut:StatutMemb.actif
  };

  maxHeight: number = 300; // Default card height

  targetDate!: Date;
  days = 0;
  hours = 0;
  minutes = 0;
  seconds = 0;

  private intervalId: any;

  ngOnDestroy() {
    clearInterval(this.intervalId);
  }

  private updateCountdown() {
    const now = new Date().getTime();
    const target = this.targetDate.getTime();
    const diff = target - now;

    if (diff < 0) {
      this.days = this.hours = this.minutes = this.seconds = 0;
      clearInterval(this.intervalId);
      return;
    }

    this.days = Math.floor(diff / (1000 * 60 * 60 * 24));
    this.hours = Math.floor((diff / (1000 * 60 * 60)) % 24);
    this.minutes = Math.floor((diff / (1000 * 60)) % 60);
    this.seconds = Math.floor((diff / 1000) % 60);
  }
  ngOnInit(): void {
    this.loadEmprunts();
        this.flipped = this.emprunts.map(() => false);
     this.updateCountdown();
    this.intervalId = setInterval(() => this.updateCountdown(), 1000);

  }
  constructor(private EmpService: EmpruntService) { }
  loadEmprunts(): void {
    this.EmpService.getAll().subscribe({
      
      next: (data) => {this.emprunts = data;
        console.log(this.emprunt.statut_emp, typeof this.emprunt.statut_emp);
      },
      error: (err) => console.error('Erreur chargement Emp', err)
    });
  }

  ngAfterViewInit(): void {
    // Can do height sync here if cards have template refs (or use ChangeDetectorRef)
    setTimeout(() => this.adjustCardHeights(), 0);
  }

  toggleFlip(index: number) {
    this.flipped[index] = !this.flipped[index];
  }

  getStatusClass(statut: Statut_emp): string {
    switch (statut) {
      case Statut_emp.en_cours:
        return 'orange';
      case Statut_emp.retourne:
        return 'green';
      case Statut_emp.perdu:
        return 'red';
      default:
        return '';
    }
  }
  
getStatutLabel(statut: Statut_emp): string {
  switch (statut) {
    case Statut_emp.en_cours: return "en_cours";
    case Statut_emp.retourne: return "retourne";
    case Statut_emp.perdu: return "perdu";
    default:  return 'lala';
  }}
  
  
  getMemberIcon(type: TypeMemb): string {
    switch (type) {
      case TypeMemb.Enseignant:
        return '👨‍🏫';
      case TypeMemb.Etudiant:
        return '🎓';
      default:
        return '📚';
    }
  }

  getTimerDisplay(e: EmppruntDTO): string {
    const now = new Date();
    if (e.statut_emp === Statut_emp.en_cours) {
      // Descending time left = difference between date_retour_prevu and now
      const diff = e.date_retour_prevu.getTime() - now.getTime();
      if (diff <= 0) return '0d 0h 0m';
      const days = Math.floor(diff / (1000 * 60 * 60 * 24));
      const hrs = Math.floor((diff / (1000 * 60 * 60)) % 24);
      const mins = Math.floor((diff / (1000 * 60)) % 60);
      return `${days}d ${hrs}h ${mins}m`;
    } else if (e.statut_emp === Statut_emp.retourne) {
      // Number of days between date_emp and date_retour_prevu
      const diff = e.date_retour_prevu.getTime() - e.date_emp.getTime();
      const days = Math.floor(diff / (1000 * 60 * 60 * 24));
      return `${days} jours`;
    } else if (e.statut_emp === Statut_emp.perdu) {
      // Display date_emp only in string format
      return e.date_emp.toLocaleDateString();
    }
    return '';
  }

  adjustCardHeights() {
    // Sync all cards heights to the max height found
    const heights = Array.from(document.querySelectorAll('.card')).map((el) => el.clientHeight);
    this.maxHeight = Math.max(...heights, this.maxHeight);
  }

}
