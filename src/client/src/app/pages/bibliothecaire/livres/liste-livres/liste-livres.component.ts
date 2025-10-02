import { Component, ElementRef, HostListener, Input, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { SpeedDialModule } from 'primeng/speeddial';
import { MenuItem, MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import { LivreService } from '../../../../Services/livre.service';
import { etat_liv, LivreDTO, Statut_liv } from '../../../../model/livres.model';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';

@Component({
  selector: 'app-liste-livres',
  standalone: true,
  imports: [ CommonModule, FormsModule, ButtonModule, InputTextModule,
    IconFieldModule, InputIconModule, SpeedDialModule, SelectModule],
  templateUrl: './liste-livres.component.html',
  styleUrls: ['./liste-livres.component.css']
})
export class ListeLivresComponent {
  public Getstautvalue(value: number) {
    return Statut_liv[value];
  }
  public Getetavalue(value: number) {
    return etat_liv[value];
  }
  selectedStatutLiv: Statut_liv | null = null;
  selectstatutLiv: { label: string; value: Statut_liv | null }[] = [
    { label: 'Tous', value: null },
    { label: 'Disponible', value: Statut_liv.disponible },
    { label: 'Perdu', value: Statut_liv.perdu },
    { label: 'Emprunte', value: Statut_liv.emprunte }
  ];
  @Input() isHosted: boolean = false;
  livres: LivreDTO[] = [];

  constructor(private livreService: LivreService, private route: Router, private messageserv: MessageService) { };
  ngOnInit() {
    this.getBooks();
  }

  getBooks(): void {
    this.livreService.getAllLiv().subscribe({
      next: (data) => this.livres = data,
      error: (error) => {
        console.error('Error fetching livres:', error);
        this.messageserv.add({ severity: 'error', summary: 'Error', detail: 'Error get books:' });

      }
    });
  }
  applyFilter(statut_liv?: Statut_liv): void {
    if (statut_liv === null) {
      this.getBooks();
    } else {
      this.livreService.filtre(statut_liv).subscribe({
        next: result => {
          this.livres = result;
        },
        error: error => {
          console.error('Error loading filtered Livres', error);
        }
      });
    }
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
      this.livreService.search(this.searchQuery).subscribe({
        next: (data) => { this.livres = data },
        error: (err) => {
          console.error('Error searching livres:', err);
          this.messageserv.add({ severity: 'error', summary: 'Error', detail: 'Error searching livres:' });

        }
      });
    }
    else { this.getBooks(); }
  }
  isClickInside(event: MouseEvent): boolean {
    const searchContainer = document.getElementById('search-container');
    return searchContainer ? searchContainer.contains(event.target as Node) : false;
  }
  isOpen: Set<string> = new Set(); // Track open book IDs

  toggleBook(livre: LivreDTO, event: Event) {
    event.stopPropagation();
    const livreId = livre.id_inv;
    if (this.isOpen.has(livreId)) {
      this.isOpen.delete(livreId);
    } else {
      this.isOpen.clear();
      this.isOpen.add(livreId);
    }
  }
  getSpeedDialItems(livreId: string, statut: Statut_liv): MenuItem[] {
    const items: MenuItem[] = [ {
        label: 'Modifier',
        icon: 'pi pi-pencil',
        command: () => this.editLivre(livreId)
      },{
        label: 'Supprimer',
        icon: 'pi pi-trash',
        command: () => this.deleteLivre(livreId)
      }];


    if (statut === Statut_liv.disponible) {
      items.push({
        label: 'Emprunte',
        icon: 'pi pi-id-card',
        command: () => this.Emprunter(livreId)
      });
    }

    return items;
  }
  Ajouter() {
    console.log(`Navigating to ajouter Livre`);
    this.route.navigate([`/bibliothecaire/livres/ajouter`]);
  }
  editLivre(livreId: string) {
    console.log(`Navigating to edit livre ID: ${livreId}`);
    this.route.navigate([`/bibliothecaire/livres/modifier/${livreId}`]);
  }
  Emprunter(livreId: string) {
    console.log(`Navigating to emprunte livre ID: ${livreId}`);
    this.route.navigate([`/bibliothecaire/emprunts/ajouter/${livreId}`]);
  }
  deleteLivre(livreId: string) {
    if (confirm('Voulez-vous vraiment supprimer ce livre ?')) {
      this.livreService.delete(livreId).subscribe({
        next: () => {
          this.getBooks();
          this.messageserv.add({ severity: 'success', summary: 'Succès', detail: 'Livres Supprimer' });

        },
        error: (err) => {
          console.error(err);
          this.messageserv.add({ severity: 'error', summary: 'Error', detail: 'Error supprimer Livres:' });

        }

      });
    }
  }
}