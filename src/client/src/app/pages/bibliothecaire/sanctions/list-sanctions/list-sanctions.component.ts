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
import { MessageService } from 'primeng/api';
import { MultiSelectModule } from 'primeng/multiselect';




@Component({
  selector: 'app-list-sanctions',
  imports: [InputIconModule, IconFieldModule, InputTextModule, SpeedDialModule, RouterLink,SelectModule,
    ButtonModule , CardModule, SelectModule, CommonModule, MultiSelectModule, FormsModule],
  templateUrl: './list-sanctions.component.html',
  styleUrl: './list-sanctions.component.css'
})
export class ListSanctionsComponent implements OnInit, OnDestroy {
  intervalId?: any;
  ngOnInit() {
    this.loadSanction(); this.intervalId = setInterval(() => { }, 60000);//1mn=60000ms
  }
  ngOnDestroy() {
    if (this.intervalId) clearInterval(this.intervalId);
  }
  constructor(private SancService: SanctionService, private router: Router, private messageService: MessageService) { }
  Sanctions: SanctionDTO[] = [];
  selectedPay: boolean | null = null;
  selectpaye: { label: string; value: boolean | null }[] = [
    { label: 'Tous', value: null },
    { label: 'Payé', value: true },
    { label: 'Non Payé', value: false }];
      selectedRaison: Raison_sanction [] = [];
  selectRaison: { label: string; value: Raison_sanction | null }[] = [
    { label: 'Retard', value: Raison_sanction.retard },
    { label: 'Perte', value: Raison_sanction.perte },
    { label: 'Degat', value: Raison_sanction.degat },
    { label: 'Autre', value: Raison_sanction.autre }
  ];

  loadSanction(): void {
    this.SancService.getAll().subscribe({
      next: (data) => this.Sanctions = data,
      error: (err) => {
        console.error('Erreur chargement Emp', err);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error get Sanction:' });

      }
    });
  }
   applyFilterR(raison?: Raison_sanction[]): void {
    if (!raison || raison.length === 0) {
        this.loadSanction();
    } else {
        this.SancService.FiltrRaison(raison).subscribe({
            next: result => {
                this.Sanctions = result;
            },
            error: error => {
                console.error('Error loading filtered Sanction', error);
            }
        });
    }
}

  applyFilterP(paye?: boolean): void {
    if (paye === null) {
      this.loadSanction();
    } else {
      this.SancService.FiltrPay(paye).subscribe({
        next: result => {
          this.Sanctions = result;
        },
        error: error => {
          console.error('Error loading filtered Sanction', error);
        }
      });
    }
  }
  getTimeDisplay(Sanction?: SanctionDTO): string {
    if (!Sanction) return '';

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
    if (value != "") {
      this.SancService.search(this.searchQuery).subscribe({
        next: (data) => { this.Sanctions = data },
        error: (err) => {
          console.error('Error searching livres:', err);
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error searching Sanction:' });

        }
      });
    }
    else { this.loadSanction(); }
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
    if (confirm('Voulez-vous vraiment Marque Comme payé?')) {
      this.SancService.modifier(id).subscribe({
        next: (data: any) => {
          this.Sanctions = data;
          this.loadSanction();
          this.messageService.add({ severity: 'success', summary: 'Succès', detail: 'Sanction Payé ' });
        },
        error: (err: any) => {
          console.error('Erreur chargement Emp', err);
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error Modifier Sanction:' });

        }
      });
    }
  }






}