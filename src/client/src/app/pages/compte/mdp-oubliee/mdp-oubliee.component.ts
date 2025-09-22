import { Component , ViewChild} from '@angular/core';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule, NgForm } from '@angular/forms';
import { IftaLabelModule } from 'primeng/iftalabel';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { Router, RouterLink } from '@angular/router';
import { ForgotPasswordRequest } from '../../../model/bibliothecaire.model';
import { AuthService } from '../../../Services/auth.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-mdp-oubliee',
  imports: [RouterLink ,ButtonModule  , FormsModule,IconFieldModule , InputIconModule ,CommonModule,IftaLabelModule,InputTextModule],
  templateUrl: './mdp-oubliee.component.html',
  styleUrl: './mdp-oubliee.component.css'
})
export class MdpOublieeComponent {
  @ViewChild('motDePasseOublieeForm') motDePasseOublieeForm: NgForm | undefined;
  mdpreq: ForgotPasswordRequest = {
    email: ''
  };

  isLoading = false;

  constructor(private authService: AuthService, private router: Router, private messgeserv: MessageService) { }

  mdp(): void {
  if (!this.motDePasseOublieeForm?.valid) {
      this.messgeserv.add({ severity: 'warn', summary: 'attention', detail: 'Veuillez saisir votre email ' });
      return;
    }
    this.isLoading = true;
    this.authService.forgotPassword(this.mdpreq).subscribe({
      next: () => {
        this.isLoading = false;
        this.messgeserv.add({ severity: 'success', summary: 'Succès', detail: 'Ouvrier Votre Email' });
      },
      error: (err) => {
        this.isLoading = false;
        this.messgeserv.add({ severity: 'error', summary: 'Error', detail: 'Erreur lors de la connexion' });
      }
    });
  }
}