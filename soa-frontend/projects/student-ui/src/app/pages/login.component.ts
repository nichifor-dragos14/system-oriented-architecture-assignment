import { Component } from '@angular/core';
import { AuthService } from '../core/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  template: `
    <main class="page">
      <section class="card">
        <h2 class="title">Student Login</h2>

        <form (ngSubmit)="submit()" #f="ngForm" class="form" novalidate>
          <div class="field">
            <label for="email">Email</label>
            <input
              id="email"
              [(ngModel)]="email"
              name="email"
              type="email"
              placeholder="you@example.com"
              autocomplete="username"
              required
              [class.invalid]="f.submitted && !email"
            />
            <small *ngIf="f.submitted && !email" class="hint">Email is required.</small>
          </div>

          <div class="field">
            <label for="password">Password</label>
            <div class="password">
              <input
                id="password"
                [(ngModel)]="password"
                name="password"
                [type]="showPassword ? 'text' : 'password'"
                placeholder="••••••••"
                autocomplete="current-password"
                required
                [class.invalid]="f.submitted && !password"
              />
              <button type="button" class="toggle" (click)="showPassword = !showPassword">
                {{ showPassword ? 'Hide' : 'Show' }}
              </button>
            </div>
            <small *ngIf="f.submitted && !password" class="hint">Password is required.</small>
          </div>

          <button class="submit" [disabled]="submitting || !email || !password">
            <span *ngIf="!submitting">Login</span>
            <span *ngIf="submitting" class="spinner" aria-hidden="true"></span>
          </button>

          <p *ngIf="error" class="error">{{ error }}</p>
        </form>
      </section>
    </main>
  `,
  styles: [`
  :host { display:block; }
  .page { min-height: 100dvh; display:grid; place-items:center; padding: 24px; background: #f5f7fb; }
  .card { width: 100%; max-width: 380px; background: #fff; border: 1px solid #e5e7eb; border-radius: 16px; padding: 24px 22px; box-shadow: 0 6px 24px rgba(0,0,0,.06); }
  .title { margin: 0 0 16px; font-size: 22px; font-weight: 700; text-align:center; color:#0f172a; }
  .form { display: grid; gap: 14px; }
  .field { display: grid; gap: 6px; }
  label { font-size: 13px; color:#374151; }

  input {
    width: 100%;
    height: 44px;           
    box-sizing: border-box;       
    border: 1px solid #d1d5db;
    border-radius: 10px;
    padding: 10px 12px;
    font-size: 14px;
    outline: none;
    transition: border-color .15s, box-shadow .15s;
  }
  input:focus { border-color: #2563eb; box-shadow: 0 0 0 3px rgba(37,99,235,.15); }
  .invalid { border-color: #ef4444 !important; }
  .hint { color:#6b7280; font-size: 12px; }

  .password { position: relative; display:flex; align-items:center; }
  .password input {
    flex: 1;
    height: 44px;            
    padding-right: 84px;         
  }
  .toggle {
    position: absolute;
    right: 6px;
    top: 50%;                     
    transform: translateY(-50%);    
    height: 32px;                   
    padding: 0 10px;
    border: 1px solid #e5e7eb;
    background:#f9fafb;
    border-radius: 8px;
    font-size:12px;
    cursor:pointer;
    line-height: 32px;            
  }
  .toggle:hover { background:#f3f4f6; }

  .submit { margin-top: 4px; display: inline-flex; justify-content:center; align-items:center; width: 100%; height: 40px; border: none; border-radius: 10px; background: #2563eb; color: #fff; font-weight: 600; cursor: pointer; transition: transform .05s ease; }
  .submit[disabled] { opacity: .6; cursor: not-allowed; }
  .submit:active { transform: translateY(1px); }
  .error { margin: 10px 0 0; color:#b91c1c; background:#fee2e2; border:1px solid #fecaca; padding:8px 10px; border-radius: 10px; font-size: 13px; }
  .spinner { width: 16px; height: 16px; border: 2px solid transparent; border-top-color: #fff; border-right-color:#fff; border-radius: 50%; animation: spin .6s linear infinite; }
  @keyframes spin { to { transform: rotate(360deg); } }
`]
})
export class LoginComponent {
  email = '';
  password = '';
  error = '';
  submitting = false;
  showPassword = false;

  constructor(private auth: AuthService, private router: Router) {}

  submit() {
    if (this.submitting) {
      return;
    }
    
    this.submitting = true;

    this.auth.login(this.email, this.password).subscribe({
      next: (response: any) => {
        this.auth.setToken(response.accessToken);
        const role = this.auth.role();

        if (role && role !== 'Student') {
          this.error = 'Not a Student account.';
          this.auth.logout();
          this.submitting = false;
          return;
        }

        this.router.navigateByUrl('/dashboard');
      },
      error: () => {
        this.error = 'Invalid credentials';
        this.submitting = false;
      }
    });
  }
}
