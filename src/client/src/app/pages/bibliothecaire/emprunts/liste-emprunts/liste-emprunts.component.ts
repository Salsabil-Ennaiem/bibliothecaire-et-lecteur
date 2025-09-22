import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { SpeedDialModule } from 'primeng/speeddial';
import { MenuItem, MessageService } from 'primeng/api';
import { CommonModule, formatDate } from '@angular/common';
import { Router } from '@angular/router';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { EmpruntService } from '../../../../Services/emprunt.service';
import { EmppruntDTO, Statut_emp } from '../../../../model/emprunts.model';
import { BadgeModule } from 'primeng/badge';
import { DatePickerModule } from 'primeng/datepicker';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';

@Component({
  selector: 'app-liste-emprunts',
  imports: [InputIconModule, DatePickerModule, IconFieldModule, BadgeModule, SelectModule,
    ButtonModule, SpeedDialModule, CommonModule, InputTextModule, FormsModule],
  templateUrl: './liste-emprunts.component.html',
  styleUrls: ['./liste-emprunts.component.css']
})
export class ListeEmpruntsComponent implements OnInit, OnDestroy {
  intervalId?: any;
  selectedStatutEmp: Statut_emp | null = null;
  selectstatutEmp: { label: string; value: Statut_emp | null }[] = [
    { label: 'Tous', value: null },
    { label: 'En Cours', value: Statut_emp.en_cours },
    { label: 'Perdu', value: Statut_emp.perdu },
    { label: 'Retourne', value: Statut_emp.retourne }
  ];

  ngOnInit() {
    this.loadEmprunts();
    this.intervalId = setInterval(() => { }, 60000);//1mn=60000ms

  }
  ngOnDestroy() {
    if (this.intervalId) clearInterval(this.intervalId);
  }
  constructor(private EmpService: EmpruntService, private router: Router, private messageService: MessageService) { }
  emprunts: EmppruntDTO[] = [];
  applyFilter(statut_emp?: Statut_emp): void {

    if (statut_emp === null) {
      this.loadEmprunts();
    }
    else {
      this.EmpService.filtre(statut_emp).subscribe({
        next: result => {
          this.emprunts = result;
        },
        error: error => {
          console.error('Error loading filtered emprunts', error);
        }
      });
    }
  }

  loadEmprunts(): void {
    this.EmpService.getAll().subscribe({
      next: (data) => this.emprunts = data,
      error: (err) => {
        console.error('Erreur chargement Emp', err);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error get Emprunts:' });

      }
    });
  }
  getTimeDisplay(emprunt?: EmppruntDTO): string {
    if (!emprunt) return 'desoli';

    const now = new Date();

    if (emprunt.statut_emp === 0) {
      if (!emprunt.date_retour_prevu) return '';
      const retour = new Date(emprunt.date_retour_prevu);
      const diffMs = retour.getTime() - now.getTime();
      const diffMinutes = Math.floor((diffMs % (1000 * 60 * 60)) / (1000 * 60));
      const diffHours = Math.floor((diffMs % (1000 * 60)) / (1000 * 60 * 60));
      const diffDays = Math.round(diffMs / (1000 * 60 * 60 * 24));

      const signe = diffMs < 0 ? '-' : '';
      const absHours = Math.abs(diffHours);
      const absMinutes = Math.abs(diffMinutes);
      const absJours = Math.abs(diffDays);

      return `Temps restant : ${signe}${absJours}j ${absHours}h ${absMinutes}m`;
    }
    else if (emprunt.statut_emp === 1) { // Cas "retourné"
      if (!emprunt.date_retour_prevu || !emprunt.date_effectif) return '';
      const datePrev = new Date(emprunt.date_retour_prevu);
      const dateEff = new Date(emprunt.date_effectif);
      const diffDays = Math.round((dateEff.getTime() - datePrev.getTime()) / (1000 * 60 * 60 * 24));
      if (diffDays > 0) { return `Retourne avant délais ${diffDays} j`; }
      else { return `Retard aprés le délais ${diffDays} j` }
    }
    else if (emprunt.statut_emp === 2) { // Cas "perdu"
      if (!emprunt.date_emp) return '';
      const dateEmp = new Date(emprunt.date_emp);
      return `Date emprunt : ${dateEmp.toLocaleDateString()}`;
    }
    return '';
  }

  //Recherche 
  searchQuery = "";
  isInputVisible = false;
  @HostListener('document:click', ['$event'])
  @HostListener('window:scroll', [])
  handleOutsideEvents(event?: MouseEvent | KeyboardEvent) {
    if (event instanceof MouseEvent) {
      const clickedInside = this.isClickInside(event);
      if (!clickedInside) {
        this.isInputVisible = false;
        this.searchQuery = null!;
      }
    } else {
      this.isInputVisible = false;
      this.searchQuery = null!;

    }
  }
  toggleInput() {
    this.isInputVisible = true;
  }
  handleSearch(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.searchQuery = value;
    if (value != "") {
      this.EmpService.search(this.searchQuery).subscribe({
        next: (data) => { this.emprunts = data },
        error: (err) => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error searching Emprunts:' });

          console.error('Error searching Emprunts:', err)
        }
      });
    }
    else {
      this.loadEmprunts();
    }
  }
  isClickInside(event: MouseEvent): boolean {
    const searchContainer = document.getElementById('search-container');
    return searchContainer ? searchContainer.contains(event.target as Node) : false;
  }

  showSpeedDial: boolean = false;
  toggleSpeedDial(event: Event, id: string) {
    event.stopPropagation();
    const emprunt = this.emprunts.find(e => e.id_emp === id);
    if (emprunt) {
      this.showSpeedDial = !this.showSpeedDial;
    }
  }
  createSpeedDialItems(empruntId: string): MenuItem[] {
    return [
      {
        label: 'Modifier',
        icon: 'pi pi-pencil',
        command: () => this.modifier(empruntId)
      },
      {
        label: 'Supprimer',
        icon: 'pi pi-trash',
        command: () => this.supprimer(empruntId)
      },
      {
        label: 'Sanction',
        icon: 'pi pi-ban',
        command: () => this.sanctionner(empruntId)
      }
    ];
  }

  getSeverity(statut: number): 'info' | 'success' | 'warn' | 'danger' | 'secondary' | 'contrast' {
    switch (statut) {
      case 0:
        return 'success';
      case 1:
        return 'warn';
      case 2:
        return 'danger';
      default:
        return 'info';
    }
  }

  flippedIndex: number | null = null;

  toggleFlipoo(index: number) {
    if (this.flippedIndex === index) {
      this.flippedIndex = null;
    } else {
      this.flippedIndex = index;
    }
  }

  public Getvalue(value: number) {
    return Statut_emp[value];
  }
  public IconTypeMem(value: number) {
    if (value == 0)
      return "👨‍🏫";
    else if (value == 1)
      return "🎓";
    else
      return "📚";
  }

  modifier(id: string) {
    console.log(`Navigating to edit Emprunts ID: ${id}`);
    this.router.navigate([`/bibliothecaire/emprunts/modifier/${id}`]);
  }
  supprimer(id: string): void {
    if (confirm('Voulez-vous vraiment supprimer cette nouveauté ?')) {
      console.log(`Delete Emprunts ID: ${id}`);
      this.EmpService.delete(id).subscribe(
        {
          next: () => {
            console.log('Emprunts deleted successfully');
            this.loadEmprunts();
            this.messageService.add({ severity: 'success', summary: 'Succès', detail: 'Emprunt Supprimer' });

          },
          error: (error) => {
            console.error('Error deleting Emprunts:', error);
            this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error deliting Emprunts:' });

          }

        }
      );
    }
  }

  sanctionner(id: string): void {
    console.log(`Navigating to edit Emprunts ID: ${id}`);
    this.router.navigate(['/bibliothecaire/sanctions/ajouter', id]);
  }
}