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
  const htmlRoot = document.documentElement;  // <html>
  const body = document.body;                 // <body>

  if (this.isDarkMode) {
    htmlRoot.classList.add('dark-theme');
    body.classList.add('dark-theme');
  } else {
    htmlRoot.classList.remove('dark-theme');
    body.classList.remove('dark-theme');
  }
}

}
