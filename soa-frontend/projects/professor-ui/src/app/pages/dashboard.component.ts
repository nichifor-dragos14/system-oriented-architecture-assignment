import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../core/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-professor-dashboard',
  standalone: true,
  imports: [
    CommonModule
  ],
  template: `
    <h2>Students</h2>
    <ul>
      <li *ngFor="let s of students"> {{s.name}} ({{s.id}}) </li>
    </ul>
  `
})
export class DashboardComponent implements OnInit {
  students:any[]=[];

  constructor(private http: HttpClient, private auth: AuthService) {}

  ngOnInit() {
    this.http.get<any[]>('http://localhost/api/students',
      { headers: this.auth.authHeaders() })
      .subscribe(d => this.students = d);
  }
}
