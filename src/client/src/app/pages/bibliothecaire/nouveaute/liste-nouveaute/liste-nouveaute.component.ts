import { Component, Input, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { CarouselModule } from 'primeng/carousel';
import { TagModule } from 'primeng/tag';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NouveauteService } from '../../../../Services/nouveaute.service';
import { NouveauteGetALL } from '../../../../model/nouveaute.model';
//import { FichierDto } from '../../../../model/fichier.model';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { MessageService } from 'primeng/api';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-liste-nouveaute',
  imports: [ButtonModule, CarouselModule, TagModule, CommonModule, FormsModule ],
  templateUrl: './liste-nouveaute.component.html',
  styleUrl: './liste-nouveaute.component.css',
  providers: []
})
export class ListeNouveauteComponent implements OnInit {
  nouveautes: NouveauteGetALL[] = [];

  today: number = Date.now(); 
  isNewPublication(datePublication: string | null): boolean {
    if (!datePublication) return false;
    const pubDate = new Date(datePublication).getTime();
    const yearAgo = this.today - (365 * 24 * 60 * 60 * 1000);
    return pubDate > yearAgo;
  }
  @Input() isHosted: boolean = false;

  ngOnInit(): void {
    this.loadNouveautes();
  }
  constructor(private router: Router, private nouveauteService: NouveauteService, private messageService: MessageService, private sanitizer: DomSanitizer) { }
  loadNouveautes(): void {
    this.nouveauteService.getAllNouv().subscribe({
      next: (data) => this.nouveautes = data,
      error: (err) => {
        console.error('Erreur chargement nouveautés', err);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error get Nouveautes' });

      }
    });
  }

  showDetails(id: string | null): void {
    if (!id) return;
    this.router.navigate(['/bibliothecaire/nouveaute/detail', id]);
  }

  onAjouter(): void {
    this.router.navigate(['/bibliothecaire/nouveaute/ajouter']);
  }

  onSupprimer(id: string): void {
    if (confirm('Voulez-vous vraiment supprimer cette nouveauté ?')) {
      this.nouveauteService.delete(id).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Succès', detail: 'Nouvete Supprimer' });
          this.loadNouveautes();
        },
        error: (err) => {
          console.error('Erreur suppression nouveauté', err);
          alert('Erreur lors de la suppression.');
        }
      });
    }
  }
  /*
  toImageSrc(file: FichierDto | null | undefined): SafeUrl | null {
    if (!file?.contenuFichier) return null;

    const bytes = file.contenuFichier;
    let binary = '';
    for (let i = 0; i < bytes.length; i++) {
      binary += String.fromCharCode(bytes[i]);
    }
    const base64String = window.btoa(binary);
    const mimeType = file.typeFichier ?? 'image/png';

    const objectURL = `data:${mimeType};base64,${base64String}`;
    return this.sanitizer.bypassSecurityTrustUrl(objectURL);
  }

  getSafeImage(file?: FichierDto | null): SafeUrl | null {
    if (!file?.contenuFichier) return null;

    // Convert binary content to base64
    const bytes = new Uint8Array(file.contenuFichier);
    let binary = '';
    bytes.forEach(b => binary += String.fromCharCode(b));
    const base64String = window.btoa(binary);

    // Compose full data URL
    const objectURL = `data:${file.typeFichier ?? 'image/png'};base64,${base64String}`;

    // Bypass security and return safe URL
    return this.sanitizer.bypassSecurityTrustUrl(objectURL);
  }
*/

  responsiveOptions = [
    {
      breakpoint: '1024px',
      numVisible: 3,
      numScroll: 1
    },
    {
      breakpoint: '768px',
      numVisible: 2,
      numScroll: 1
    },
    {
      breakpoint: '560px',
      numVisible: 1,
      numScroll: 1
    }
  ];

}
