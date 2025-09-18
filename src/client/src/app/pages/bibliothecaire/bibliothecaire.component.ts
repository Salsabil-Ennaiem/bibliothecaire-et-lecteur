import { CommonModule } from '@angular/common';
import { Component, HostListener, ViewChild } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { ParametreComponent } from './parametre/parametre.component';
import { ProfilComponent } from './profil/profil.component';
import { NotificationComponent } from './notification/notification.component';
import { DrawerModule } from 'primeng/drawer';


@Component({
  selector: 'app-bibliothecaire',
  imports: [RouterOutlet,DrawerModule,ButtonModule, CommonModule,RouterLink , ParametreComponent , ProfilComponent , NotificationComponent] ,
  templateUrl: './bibliothecaire.component.html',
  styleUrl: './bibliothecaire.component.css'
})
export class BibliothecaireComponent {

   @ViewChild(NotificationComponent) notificationComponent!: NotificationComponent;

  onBellClick(event: Event) {
    this.notificationComponent.open(event);
  }
  unreadCount = 0;


  onUnreadCountChange(count: number) {
    this.unreadCount = count;
  }

    drawerVisible = false;
  sidebarVisible = false;
   @HostListener('window:scroll', ['$event'])
  onWindowScroll() {
    if (this.drawerVisible) {
      this.drawerVisible = false;
    }
  }
  showParametre = false;
  showParametres() {
    this.showParametre= true;
  }

  showProfil = false;
  showProfile() {
    this.showProfil= true;
  }

  

  isSticky: boolean = false;
  @HostListener('window:scroll', ['$event'])
  checkScroll() {
    this.isSticky = window.pageYOffset > 100;
  }
  scrollToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
