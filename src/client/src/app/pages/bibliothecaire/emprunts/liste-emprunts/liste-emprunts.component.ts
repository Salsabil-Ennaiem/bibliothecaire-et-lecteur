import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { SpeedDialModule } from 'primeng/speeddial';
import { SelectModule } from 'primeng/select';
import { MenuItem } from 'primeng/api';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { EmpruntService } from '../../../../Services/emprunt.service';
import { EmppruntDTO } from '../../../../model/emprunts.model';


@Component({
  selector: 'app-liste-emprunts',
  imports: [InputIconModule, IconFieldModule, InputTextModule,
    ButtonModule, CardModule, SpeedDialModule, SelectModule, CommonModule, FormsModule, RouterLink],
  templateUrl: './liste-emprunts.component.html',
  styleUrls: ['./liste-emprunts.component.css']
})
export class ListeEmpruntsComponent implements OnInit {

    ngOnInit() {
this.loadEmprunts();
  }
  constructor(private EmpService: EmpruntService, private router: Router) { }
  emprunts: EmppruntDTO[] = [];

  loadEmprunts(): void {
    this.EmpService.getAll().subscribe({
      next: (data) => this.emprunts = data,
      error: (err) => console.error('Erreur chargement Emp', err)
    });
  }

  isInputVisible = false;
  searchQuery = '';
  toggleInput() {
    this.isInputVisible = true;
  }

  handleSearch(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.searchQuery = value;
    this.EmpService.search(this.searchQuery).subscribe({
      next:(data) => this.emprunts = data,
      error:(error) => console.error('Error searching livres:', error)});
  }

  @HostListener('document:click', ['$event'])
  @HostListener('window:scroll', [])
  @HostListener('document:keydown', ['$event'])
  handleOutsideEvents(event?: MouseEvent | KeyboardEvent) {
    if (event instanceof MouseEvent) {
      const clickedInside = this.isClickInside(event);
      if (!clickedInside) {
        this.isInputVisible = false;
      }
    } else if (event instanceof KeyboardEvent) {
      if (event.key === 'Escape') {
        this.isInputVisible = false;
      }
    } else {
      this.isInputVisible = false;
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
    isFlipped: boolean = false;
  toggleFlip(id: string) {
    this.emprunts.forEach(emprunt => {
      if (emprunt.id_emp !== id) {
        this.isFlipped = false;
      }
    });
    // Toggle the clicked card
    const emprunt = this.emprunts.find(e => e.id_emp === id);
    if (emprunt) {
      this.isFlipped = !this.isFlipped;
    }
  }
  
    modifier(id: string) {
    console.log(`Navigating to edit Emprunts ID: ${id}`);
    this.router.navigate([`/bibliothecaire/emprunts/modifier/${id}`]);
  }
      Ajouter() {
    console.log(`Navigating to ajouter Emprunts`);
    this.router.navigate([`/bibliothecaire/emprunts/ajouter`]);
  }

  supprimer(id: string):void {
    console.log(`Delete Emprunts ID: ${id}`);
    this.EmpService.delete(id).subscribe(
{    next:() => console.log('Emprunts deleted successfully'),
      error:(error) => console.error('Error deleting Emprunts:', error)}
    );
  }

  sanctionner(id: string) {
    console.log(`Navigating to edit Emprunts ID: ${id}`);
    this.router.navigate([`/bibliothecaire/sanctions/ajouter/${id}`]);
  }



  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
  importer() {
    this.fileInput.nativeElement.click();
  }

  handleFileUpload(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    if (!file.type.match(/application\/(vnd.ms-excel|vnd.openxmlformats-officedocument.spreadsheetml.sheet)/)) {
      console.log('Selected file:', file.name, file.type, file.size);
      alert('Veuillez sélectionner un fichier Excel (.xls ou .xlsx).');
      return;
    }
/* this.EmpService.import(file).subscribe(
      response => console.log('Import successful:', response),
      error => console.error('Error importing file:', error)
    );*/
  }
  /*exporter() {
    this.EmpService.export().subscribe(
      blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'ListeEmprunts.xlsx';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error => console.error('Error exporting file:', error)
    );
  }*/
}