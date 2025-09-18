import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { SelectModule } from 'primeng/select';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { SanctionService } from '../../../../Services/sanction.service';
import { Raison_sanction, SanctionDTO } from '../../../../model/sanction.model';
import { SpeedDialModule } from 'primeng/speeddial';




@Component({
  selector: 'app-list-sanctions',
  imports: [InputIconModule, IconFieldModule, InputTextModule, SpeedDialModule, RouterLink,
    ButtonModule, CardModule, SelectModule, CommonModule, FormsModule],
  templateUrl: './list-sanctions.component.html',
  styleUrl: './list-sanctions.component.css'
})
export class ListSanctionsComponent implements OnInit, OnDestroy {
  intervalId?: any;
  ngOnInit() {
    this.loadSanction(); this.intervalId = setInterval(() => {
      // Forcer Angular à rafraîchir en mettant à jour un compteur dummy ou en changeant état
    }, 60000);//1mn=60000ms
  }
  ngOnDestroy() {
    if (this.intervalId) clearInterval(this.intervalId);
  }
  constructor(private EmpService: SanctionService, private router: Router) { }
  Sanctions: SanctionDTO[] = [];

  loadSanction(): void {
    this.EmpService.getAll().subscribe({
      next: (data) => this.Sanctions = data,
      error: (err) => console.error('Erreur chargement Emp', err)
    });
  }
  getTimeDisplay(Sanction?: SanctionDTO): string {
    if (!Sanction) return 'desoli';

    const now = new Date();

    if (Sanction.active === true) {
      if (!Sanction.date_fin_sanction) return '';
      const retour = new Date(Sanction.date_fin_sanction);
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
    else if (Sanction.active === false) { // Cas "retourné"
      if (!Sanction.date_sanction || !Sanction.date_fin_sanction) return '';
      const dateSanc = new Date(Sanction.date_sanction);
      const datefin = new Date(Sanction.date_fin_sanction);
      const diffDays = Math.round((datefin.getTime() - dateSanc.getTime()) / (1000 * 60 * 60 * 24));
      return `Duree de Sanction ${diffDays} j`;
    }

    return '';
  }
  //Recherche 
  searchQuery = '';
  isInputVisible = false;
  @HostListener('document:click', ['$event'])
  @HostListener('window:scroll', [])
  handleOutsideEvents(event?: MouseEvent | KeyboardEvent) {
    if (event instanceof MouseEvent) {
      const clickedInside = this.isClickInside(event);
      if (!clickedInside) {
        this.isInputVisible = false;
        this.searchQuery = '';
      }
    } else {
      this.isInputVisible = false;
      this.searchQuery = '';

    }
  }
  toggleInput() {
    this.isInputVisible = true;
  }
  handleSearch(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.searchQuery = value;
    this.EmpService.search(this.searchQuery).subscribe({
      next: (data) => { this.Sanctions = data },
      error: (err) => { console.error('Error searching livres:', err) }
    }
    );
  }
  isClickInside(event: MouseEvent): boolean {
    const searchContainer = document.getElementById('search-container');
    return searchContainer ? searchContainer.contains(event.target as Node) : false;
  }


  public Getvalue(value: number[]): string[] {
    return value.map(element => Raison_sanction[element]);
  }

  flippedIndex: number | null = null;

  toggleFlipoo(index: number) {
    if (this.flippedIndex === index) {
      this.flippedIndex = null; // unflip if clicking already flipped card
    } else {
      this.flippedIndex = index; // flip new card, reset others
    }
  }

  modifier(id: string): void {
    if (confirm('Voulez-vous vraiment supprimer cette nouveauté ?')) {
    this.EmpService.modifier(id).subscribe({
      next: (data: any) => {
        this.Sanctions = data.p;
        this.loadSanction();
      },
      error: (err: any) => console.error('Erreur chargement Emp', err)
    });
  }}

  Ajouter() { 
    console.log(`Navigating to ajouter Emprunts`);
    this.router.navigate([`/bibliothecaire/sanctions/ajouter`]);
  }





}