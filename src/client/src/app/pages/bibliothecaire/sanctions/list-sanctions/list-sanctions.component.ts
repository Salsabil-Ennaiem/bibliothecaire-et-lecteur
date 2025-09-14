import { Component, HostListener, OnInit } from '@angular/core';
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
  imports: [ InputIconModule , IconFieldModule, InputTextModule,SpeedDialModule,RouterLink,
    ButtonModule, CardModule, SelectModule, CommonModule, FormsModule],
      templateUrl: './list-sanctions.component.html',
  styleUrl: './list-sanctions.component.css'
})
export class ListSanctionsComponent implements OnInit {

    ngOnInit() {
this.loadEmprunts();
  }
  constructor(private EmpService: SanctionService, private router: Router) { }
  emprunts: SanctionDTO[] = [];

  loadEmprunts(): void {
    this.EmpService.getAll().subscribe({
      next: (data) => this.emprunts = data,
      error: (err) => console.error('Erreur chargement Emp', err)
    });
  }
  targetDate!: Date;
  days = 0;
  hours = 0;

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
    }  else {
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
      next: (data) => { this.emprunts = data },
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

    modifier(id: string) {
    console.log(`Navigating to edit Sanctions ID: ${id}`);
    this.router.navigate([`/bibliothecaire/sanctions/modifier/${id}`]);
  }
      Ajouter() {
    console.log(`Navigating to ajouter Emprunts`);
    this.router.navigate([`/bibliothecaire/sanctions/ajouter`]);
  }

 



}