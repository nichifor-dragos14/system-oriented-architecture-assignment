import { Component } from '@angular/core';
import { AuthService } from '../core/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule
  ],
  template: `
    <h2>Professor Login</h2>

    <form (ngSubmit)="submit()">
      <input [(ngModel)]="email" name="email" placeholder="Email" required>
      <input [(ngModel)]="password" name="password" type="password" placeholder="Password" required>
      <button> Login </button>
      <p *ngIf="error"> {{error}} </p>
    </form>`
})
export class LoginComponent {
  email=''; 
  password=''; 
  error='';

  constructor(private auth: AuthService, private router: Router) {}

  submit() {
    this.auth.login(this.email, this.password).subscribe({
      next: response => { this.auth.setToken(response.access_token);
        if (this.auth.role() !== 'Professor') { 
          this.error='Not a Professor account.'; this.auth.logout(); 
        }

        else {
          this.router.navigateByUrl('/dashboard');
        }

      },
      error: () => this.error = 'Invalid credentials'
    });
  }
}
