import { Component, ViewChild } from '@angular/core';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule, NgForm } from '@angular/forms';
import { IftaLabelModule } from 'primeng/iftalabel';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../Services/auth.service';
import { LoginRequest } from '../../../model/bibliothecaire.model';
import { MessageService } from 'primeng/api';


@Component({
  selector: 'app-login',
  imports: [ButtonModule, RouterLink, FormsModule, IconFieldModule, InputIconModule, CommonModule, IftaLabelModule, InputTextModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  @ViewChild('loginForm') loginForm: NgForm | undefined;
  loginObj: LoginRequest = {
    email: '',
    password: ''
  };

  isLoading = false;

  constructor(private authService: AuthService, private router: Router, private messgeserv: MessageService) { }

  OnLogin(): void {
  if (!this.loginForm?.valid) {
      this.messgeserv.add({ severity: 'warn', summary: 'attention', detail: 'Veuillez saisir votre email et mot de passe ' });
      return;
    }
    this.isLoading = true;
    this.authService.login(this.loginObj).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigate(['bibliothecaire']);
        this.messgeserv.add({ severity: 'success', summary: 'Succès', detail: 'Bienvenu' });
      },
      error: (err) => {
        this.isLoading = false;
        this.messgeserv.add({ severity: 'error', summary: 'Error', detail: 'Erreur lors de la connexion' });
      }
    });
  }
}


