import { Component, HostListener } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { RouterLink } from '@angular/router';
import { ListeLivresComponent } from '../bibliothecaire/livres/liste-livres/liste-livres.component';
import { ListeNouveauteComponent } from '../bibliothecaire/nouveaute/liste-nouveaute/liste-nouveaute.component';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
//import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-accueil',
  imports: [ToggleSwitchModule , CommonModule, FormsModule, ButtonModule, RouterLink, ListeLivresComponent, ListeNouveauteComponent],
  templateUrl: './accueil.component.html',
  styleUrls: ['./accueil.component.css']
})
export class AccueilComponent {

  isSticky: boolean = false;
  isDarkMode: boolean = false;

/*  constructor(private nn: Title ,private location: Location) {
    // Set initial page title
    this.nn.setTitle('Library Home');
  }
*/
  @HostListener('window:scroll', ['$event'])
  checkScroll() {
    this.isSticky = window.pageYOffset > 100;
  }

  scrollToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
  
  toggleTheme(): void {
    this.isDarkMode = !this.isDarkMode;
    if (this.isDarkMode) {
      document.body.classList.add('dark-theme');
     // this.nn.setTitle(this.isDarkMode ? 'Library Home - Dark Mode' : 'Library Home - Light Mode');
    } else {
      document.body.classList.remove('dark-theme');
    }
  }
  /*
  goBack(): void {
    this.location.back(); // Navigate to the previous page in browser history
  }*/
}
