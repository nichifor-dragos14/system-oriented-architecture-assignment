import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { Router, NavigationEnd, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './core/auth.service';
import { filter, Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    CommonModule
  ],
  template: `
    <nav class="nav">
      <a *ngIf="!loggedIn()" [routerLink]="['/login']" routerLinkActive="active">Login</a>
      <a *ngIf="loggedIn()" (click)="logout()" class="logout">Logout</a>
      <a [routerLink]="['/dashboard']" routerLinkActive="active">Dashboard</a>
    </nav>

    <router-outlet></router-outlet>
  `,
  styles: [`
    .nav { display:flex; gap:12px; padding:10px 14px; border-bottom:1px solid #e5e7eb; background:#fff; }
    .nav a { text-decoration:none; color:#111827; padding:6px 10px; border-radius:8px; }
    .nav a.active { background:#eef2ff; }
    .logout { cursor:pointer; }
  `]
})
export class App implements OnInit, OnDestroy {
  public authService = inject(AuthService);
  private router = inject(Router);

  loggedIn = signal<boolean>(this.authService.isLoggedIn());

  private sub?: Subscription;
  private storageHandler = () => this.refreshAuthState();
  private pulse?: number;

  protected readonly title = signal('student-ui');

  ngOnInit(): void {
    this.sub = this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe(() => this.refreshAuthState());

    window.addEventListener('storage', this.storageHandler);

    this.pulse = window.setInterval(() => this.refreshAuthState(), 800);
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
    window.removeEventListener('storage', this.storageHandler);
    if (this.pulse) window.clearInterval(this.pulse);
  }

  logout(): void {
    this.authService.logout();
    this.refreshAuthState();
  }

  private refreshAuthState(): void {
    this.loggedIn.set(this.authService.isLoggedIn());
  }
}
