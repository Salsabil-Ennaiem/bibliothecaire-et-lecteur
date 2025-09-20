import { Component, HostListener, OnInit } from '@angular/core';
import { MembreService } from '../../../../Services/membre.service';
import { CommonModule } from '@angular/common';
import { MembreDto, StatutMemb } from '../../../../model/membre.model';
import { ButtonModule } from 'primeng/button';
import { Router } from '@angular/router';
import { SpeedDialModule } from 'primeng/speeddial';
import { MenuItem, MessageService } from 'primeng/api';
import { InputTextModule } from 'primeng/inputtext';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-list-membre',
  imports: [CommonModule, FormsModule, ButtonModule, SpeedDialModule, InputIconModule, InputIconModule, InputTextModule, IconFieldModule],
  templateUrl: './list-membre.component.html',
  styleUrl: './list-membre.component.css'
})
export class ListMembreComponent implements OnInit {

  constructor(private MemServ: MembreService, private router: Router, private messgserv: MessageService) { }
  users: MembreDto[] = []
  ngOnInit(): void {
    this.loadMembres();
  }

  loadMembres(): void {
    this.MemServ.getAll().subscribe({
      next: (data) => this.users = data,
      error: (err) => console.error('Erreur chargement Emp', err)
    });
  }
  public Getvalue(value: number) {
    return StatutMemb[value];
  }
  public IconTypeMem(value: number) {
    if (value == 0)
      return "👨‍🏫";
    else if (value == 1)
      return "🎓";
    else
      return "📚";
  }
  showSpeedDial: boolean = false;
  toggleSpeedDial(event: Event, id: string) {
    event.stopPropagation();
    const emprunt = this.users.find(e => e.id_membre === id);
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
      }
    ];
  }
  modifier(id: string) {
    console.log(`Navigating to edit Membre ID: ${id}`);
    this.router.navigate([`/bibliothecaire/membres/modifier/${id}`]);
  }

  supprimer(id: string): void {
    if (confirm('Voulez-vous vraiment supprimer cette nouveauté ?')) {
      console.log(`Delete Membre ID: ${id}`);
      this.loadMembres();
      this.MemServ.delete(id).subscribe(
        {
          next: () => {
            console.log('Membre deleted successfully');
            this.loadMembres();
            this.messgserv.add({ severity: 'success', summary: 'Succès', detail: 'Membre Supprimer' });

          },
          error: (error) => console.error('Error deleting Membre:', error)
        }
      );
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
        if(value!="")
    {this.MemServ.search(this.searchQuery).subscribe({
      next: (data) => { this.users = data },
      error: (err) => { console.error('Error searching livres:', err) }
    });}
    else{this.loadMembres();}
  }
  isClickInside(event: MouseEvent): boolean {
    const searchContainer = document.getElementById('search-container');
    return searchContainer ? searchContainer.contains(event.target as Node) : false;
  }
}
