import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../Services/auth.service';
import { FormsModule, NgForm } from '@angular/forms';
import { ResetPasswordRequest } from '../../../model/bibliothecaire.model';
import { ButtonModule } from 'primeng/button';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { CommonModule } from '@angular/common';
import { IftaLabelModule } from 'primeng/iftalabel';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-rest-mdw',
  imports: [ButtonModule, RouterLink, FormsModule, IconFieldModule, InputIconModule, CommonModule, IftaLabelModule, InputTextModule],
  templateUrl: './rest-mdw.component.html',
  styleUrl: './rest-mdw.component.css'
})
export class RestMdwComponent implements OnInit {
  model: ResetPasswordRequest = {
    email: '',
    token: '',
    newPassword: ''
  };
  confirmPassword: string = '';
  submitted: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      if (params['token'] && params['email']) {
        this.model.token = params['token'];
        this.model.email = params['email'];
      } else {
        this.errorMessage = 'Lien de réinitialisation invalide.';
      }
    });
  }

  onSubmit(form: NgForm) {
    this.submitted = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (!form.valid) {
      this.errorMessage = 'Veuillez remplir tous les champs obligatoires.';
      return;
    }

    if (this.model.newPassword !== this.confirmPassword) {
      this.errorMessage = 'Les mots de passe ne correspondent pas.';
      return;
    }

    this.authService.resetPassword(this.model).subscribe({
      next: () => {
        this.successMessage = 'Mot de passe réinitialisé avec succès. Vous pouvez maintenant vous connecter.';
        setTimeout(() => this.router.navigate(['/compte/login']), 3000);
      },
      error: (error) => {
        this.errorMessage = error?.error?.message || 'Erreur lors de la réinitialisation du mot de passe.';
      }
    });
  }
}