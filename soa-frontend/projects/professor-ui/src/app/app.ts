import { Component, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <nav>
      <a [routerLink]="['/login']" routerLinkActive="active">Login</a>
      <a [routerLink]="['/dashboard']" routerLinkActive="active">Dashboard</a>
    </nav>
    <router-outlet></router-outlet>
  `,
})
export class App {
  protected readonly title = signal('professor-ui');
}
