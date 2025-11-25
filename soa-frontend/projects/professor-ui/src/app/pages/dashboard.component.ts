import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  ProfessorApiService,
  StudentDto,
  GradeDto,
} from '../core/professor-api.service';
import { AuthService } from '../core/auth.service';

@Component({
  selector: 'app-professor-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <main class="page">
      <header class="topbar">
        <h1>Professor Console</h1>
        <div class="who">
          <span>Signed in as</span>
          <strong>{{ auth.name }}</strong>
          <button class="btn ghost" (click)="logout()">Logout</button>
        </div>
      </header>

      <section class="grid">
        <article class="card">
          <div class="card-head">
            <h2>Students</h2>
            <button class="btn" (click)="reloadStudents()">Reload</button>
          </div>

          <div *ngIf="studentsLoading(); else studentsLoaded" class="muted">Loading students…</div>
          <ng-template #studentsLoaded>
            <div class="search">
              <input [(ngModel)]="studentFilter" placeholder="Filter by name or email…" />
            </div>

            <ul class="list">
              @for (s of filteredStudents(); track s.id) {
                <li class="row" [class.active]="s.id===selectedId()" (click)="selectStudent(s)">
                  <div class="col grow">
                    <div class="name">{{ s.name }}</div>
                    <div class="sub">{{ s.email || '—' }}</div>
                  </div>
                  <div class="col">
                    <button class="btn tiny" [class.primary]="s.id===selectedId()">Select</button>
                  </div>
                </li>
              }
            </ul>
          </ng-template>
        </article>

        <article class="card">
          <div class="card-head">
            <h2>Grades</h2>
            <div class="muted" *ngIf="!selectedId()">Select a student to view/edit grades.</div>
          </div>

          <ng-container *ngIf="selectedId()">
            <div *ngIf="gradesLoading(); else gradesLoaded" class="muted">Loading grades…</div>
            <ng-template #gradesLoaded>
              <ul class="list">
                @for (g of grades(); track g.id) {
                  <li class="row">
                    <div class="col grow">
                      <div class="name">{{ g.course }}</div>
                      <div class="sub">Grade: <strong>{{ g.value }}</strong></div>
                    </div>
                    <div class="col actions">
                      <button class="btn tiny" (click)="beginEdit(g)">Edit</button>
                      <button class="btn tiny danger" (click)="remove(g)">Delete</button>
                    </div>
                  </li>
                }
              </ul>
              <div *ngIf="!grades().length" class="muted">No grades yet.</div>
            </ng-template>

            <form class="editor" (ngSubmit)="submit()">
              <h3>{{ editId() ? 'Update grade' : 'Add grade' }}</h3>
              <div class="row">
                <label>Course</label>
                <input [(ngModel)]="course" name="course" required placeholder="e.g., OOP" />
              </div>
              <div class="row">
                <label>Value</label>
                <input [(ngModel)]="value" name="value" type="number" min="1" max="10" step="1" required />
              </div>
              <div class="actions">
                <button class="btn primary" [disabled]="saving()">{{ editId() ? 'Save' : 'Add' }}</button>
                <button class="btn ghost" type="button" (click)="resetEditor()" [disabled]="saving()">Cancel</button>
              </div>
              <p class="error" *ngIf="error()">{{ error() }}</p>
            </form>
          </ng-container>
        </article>
      </section>
    </main>
  `,
  styles: `
    .page { max-width: 1100px; margin: 28px auto; padding: 0 16px; font-family: ui-sans-serif, system-ui; }
    .topbar { display:flex; align-items:center; justify-content:space-between; gap: 16px; margin-bottom: 12px; }
    .topbar h1 { margin:0; font-size: 24px; }
    .who { display:flex; align-items:center; gap: 10px; color:#334155; }
    .grid { display:grid; grid-template-columns: 1fr 1.2fr; gap: 16px; }
    @media (max-width: 880px){ .grid { grid-template-columns: 1fr; } }
    .card { background:#fff; border:1px solid #e5e7eb; border-radius:14px; padding:14px; }
    .card-head { display:flex; align-items:center; justify-content:space-between; margin-bottom: 8px; }
    .muted { color:#6b7280; }
    .search { margin-bottom: 8px; }
    .search input { width:100%; border:1px solid #e5e7eb; border-radius:10px; padding:8px 10px; }
    .list { list-style:none; margin:0; padding:0; display:grid; gap:8px; }
    .row { display:flex; align-items:center; gap:10px; border:1px solid #e5e7eb; background:#f9fafb; border-radius:10px; padding:8px 10px; cursor:pointer; }
    .row.active { border-color:#93c5fd; background:#f3f8ff; }
    .row .col { display:flex; align-items:center; gap:10px; }
    .row .col.grow { flex:1; min-width:0; }
    .name { font-weight:600; color:#0f172a; }
    .sub { color:#6b7280; font-size:13px; }
    .actions { gap:8px; }
    .editor { margin-top: 12px; display:grid; gap:10px; border-top:1px dashed #e5e7eb; padding-top:10px; }
    .editor .row { display:grid; gap:6px; }
    .editor input { border:1px solid #e5e7eb; border-radius:10px; padding:8px 10px; }
    .btn { border:1px solid #e5e7eb; background:#fff; border-radius:10px; padding:6px 10px; cursor:pointer; }
    .btn.ghost { background:#f9fafb; }
    .btn.primary { background:#2563eb; color:#fff; border-color:#2563eb; }
    .btn.tiny { padding:4px 8px; font-size:12px; }
    .btn.danger { background:#fee2e2; border-color:#fecaca; color:#991b1b; }
    .error { color:#b91c1c; background:#fee2e2; border:1px solid #fecaca; padding:8px 10px; border-radius: 10px; }
  `
})
export class DashboardComponent implements OnInit {
  private api = inject(ProfessorApiService);
  public auth = inject(AuthService);

  students = signal<StudentDto[]>([]);
  studentsLoading = signal<boolean>(true);
  studentFilter = '';
  selectedId = signal<string | null>(null);

  grades = signal<GradeDto[]>([]);
  gradesLoading = signal<boolean>(false);


  editId = signal<string | null>(null);
  course = '';
  value: number | null = null;
  saving = signal<boolean>(false);
  error = signal<string>('');

  ngOnInit(): void {
    this.reloadStudents();
  }

  logout() { this.auth.logout(); }

  filteredStudents() {
    const q = this.studentFilter.trim().toLowerCase();
    if (!q) return this.students();
    return this.students().filter(s =>
      s.name.toLowerCase().includes(q) || (s.email || '').toLowerCase().includes(q)
    );
  }

  reloadStudents() {
    this.studentsLoading.set(true);
    this.api.listStudents().subscribe({
      next: list => { this.students.set(list ?? []); this.studentsLoading.set(false); },
      error: err => { console.error(err); this.studentsLoading.set(false); }
    });
  }

  selectStudent(s: StudentDto) {
    if (this.selectedId() === s.id) return;
    this.selectedId.set(s.id);
    this.resetEditor();
    this.loadGrades(s.id);
  }

  loadGrades(studentId: string) {
    this.gradesLoading.set(true);
    this.api.listGradesForStudent(studentId).subscribe({
      next: list => { this.grades.set(list ?? []); this.gradesLoading.set(false); },
      error: err => { console.error(err); this.gradesLoading.set(false); }
    });
  }

  beginEdit(g: GradeDto) {
    this.editId.set(g.id);
    this.course = g.course;
    this.value = g.value;
  }

  resetEditor() {
    this.editId.set(null);
    this.course = '';
    this.value = null;
    this.error.set('');
  }

  submit() {
    const studentId = this.selectedId();
    if (!studentId) { return; }
    if (!this.course || this.value == null) { this.error.set('Please fill in all fields.'); return; }

    this.saving.set(true);
    const body = { course: this.course, value: Number(this.value), studentId };

    const done = () => { this.saving.set(false); };
    const onErr = (e: any) => { console.error(e); this.error.set('Operation failed.'); done(); };

    if (this.editId()) {
      this.api.updateGrade(this.editId()!, body).subscribe({
        next: g => {
          this.grades.update(list => list.map(x => x.id === g.id ? g : x));
          this.resetEditor();
          done();
        },
        error: onErr
      });
    } else {
      this.api.addGrade(body).subscribe({
        next: g => {
          this.grades.update(list => [g, ...list]);
          this.resetEditor();
          done();
        },
        error: onErr
      });
    }
  }

  remove(g: GradeDto) {
    if (!confirm(`Delete grade ${g.course} (${g.value})?`)) return;
    this.api.deleteGrade(g.id).subscribe({
      next: () => {
        this.grades.update(list => list.filter(x => x.id !== g.id));
      },
      error: e => { console.error(e); this.error.set('Delete failed.'); }
    });
  }
}
