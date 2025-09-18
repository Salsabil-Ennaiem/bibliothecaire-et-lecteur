import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterOutlet } from '@angular/router';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet ,ToastModule,FormsModule,CommonModule ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
 isDarkMode = false;

  toggleTheme() {
    this.isDarkMode = !this.isDarkMode;
    const root = document.documentElement; // cible <html>

    if(this.isDarkMode) {
      root.classList.add('dark-theme');
    } else {
      root.classList.remove('dark-theme');
    }
  }
}
