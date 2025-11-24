import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../core/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-student-dashboard',
  standalone: true,
  imports: [
    CommonModule
  ],
  template: `
    <h2>My Grades</h2>
    <ul><li *ngFor="let g of grades">{{g.course}} – {{g.value}}</li></ul>
    <h3>Live GPA updates</h3>
    <ul>
        <li *ngFor="let e of events">{{e.course}} → GPA {{e.gpa}}</li>
    </ul>
  `
})
export class DashboardComponent implements OnInit {
  grades:any[]=[]; events:any[]=[];

  constructor(private http: HttpClient, private auth: AuthService) {}

  ngOnInit() {
    this.http.get<any[]>('http://localhost/api/grades/mine',
       { headers: this.auth.authHeaders() })
      .subscribe(d => this.grades = d);

    const es = new EventSource('http://localhost/api/notifications/events/gpa');
    es.onmessage = ev => {
      const e = JSON.parse(ev.data);
      this.events.unshift(e);
    };
  }
}
