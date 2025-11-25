import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { AuthService } from '../core/auth.service';

@Component({
  selector: 'app-student-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page">
      <div class="topbar">
        <h1>Welcome, <span class="accent">{{ authService.name }}</span>!</h1>
      </div>

      <section class="cards">
        <article class="card">
          <h2>My Grades</h2>

          <div *ngIf="loading(); else loaded">
            <p class="muted">Loading grades…</p>
          </div>

          <ng-template #loaded>
            <ul class="list" *ngIf="grades().length; else emptyGrades">
              @for (grade of grades(); track grade.id ?? $index) {
                <li class="item">
                  <span class="course">{{ grade.course }}</span>
                  <span class="dot">•</span>
                  <span class="value">Grade: <strong>{{ grade.value }}</strong></span>
                </li>
              }
            </ul>
            <ng-template #emptyGrades>
              <p class="muted">No grades available yet.</p>
            </ng-template>
          </ng-template>
        </article>

        <article class="card">
          <h2>Live GPA updates</h2>
          <ul class="list live">
            @for (event of events(); track $index) {
              <li class="item">
                <span class="course">{{ event.course }}</span>
                <span class="dot">→</span>
                <span class="value">GPA <strong>{{ event.gpa }}</strong></span>
              </li>
            }
          </ul>
        </article>
      </section>
    </div>
  `,
  styles: `
    .page { max-width: 960px; margin: 32px auto; padding: 0 16px; font-family: ui-sans-serif, system-ui, -apple-system, Segoe UI, Roboto, "Helvetica Neue", Arial; gap: 32px }
    .topbar h1 { margin: 0; font-size: 28px; font-weight: 700; }
    .subtitle { margin: 4px 0 0; color: #6b7280; }
    .accent { color: #2563eb; }
    .cards { display: grid; grid-template-columns: 1fr; gap: 16px; }
    @media (min-width: 840px) { .cards { grid-template-columns: 1fr 1fr; } }
    .card { background: #fff; border: 1px solid #e5e7eb; border-radius: 14px; padding: 16px 18px; box-shadow: 0 1px 2px rgba(0,0,0,.04); }
    .card h2 { margin: 0 0 12px; font-size: 18px; }
    .list { list-style: none; padding: 0; margin: 0; display: grid; gap: 8px; }
    .item { display: flex; align-items: baseline; gap: 8px; background: #f9fafb; border: 1px solid #e5e7eb; border-radius: 10px; padding: 8px 10px; }
    .course { font-weight: 600; }
    .value strong { font-weight: 700; }
    .dot { color: #9ca3af; }
    .muted { color: #6b7280; }
    .live .item { background: #f3f8ff; border-color: #bfdbfe; }
  `
})
export class DashboardComponent implements OnInit {
  grades = signal<any[]>([]);
  events = signal<any[]>([]);
  loading = signal<boolean>(true);

  private httpClient = inject(HttpClient);
  public authService = inject(AuthService);

  ngOnInit() {
    const userId = this.authService.userId;
    this.loadGrades(userId);

    const es = new EventSource('/api/notifications/events/gpa');
    es.onmessage = event => {
      try {
        const e = JSON.parse(event.data);
        this.events.update(list => [e, ...list].slice(0, 50));

        // when a GPA event arrives for this user, refresh grades
        if (!userId || !e?.studentId || e.studentId === userId) {
          this.loadGrades(userId);
        }
      } catch (err) {
        console.error('SSE parse error', err);
      }
    };
    es.onerror = err => console.error('SSE error', err);
  }

  private loadGrades(userId: string | null) {
    if (!userId) { this.loading.set(false); return; }
    this.loading.set(true);
    this.httpClient.get<any[]>(
      `http://localhost/Grades/student/${userId}`,
      { headers: this.authService.authHeaders() }
    ).subscribe({
      next: d => { this.grades.set(d ?? []); this.loading.set(false); },
      error: e => { console.error('Grades error', e); this.loading.set(false); }
    });
  }
}
