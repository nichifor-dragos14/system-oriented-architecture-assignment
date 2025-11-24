import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private key = 'jwt';
  constructor(private http: HttpClient) {}

  login(email: string, password: string) {
    return this.http.post<{ access_token: string }>('http://localhost/auth/token', { email, password });
  }
  
  setToken(t: string) { 
    localStorage.setItem(this.key, t); 
  }

  get token() { 
    return localStorage.getItem(this.key); 
  }

  logout() { 
    localStorage.removeItem(this.key); location.href = '/'; 
  }

  role(): string | null {
    const t = this.token; if (!t) return null;
    const p = JSON.parse(atob(t.split('.')[1]));
    return p.role || p['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;
  }

  authHeaders(): HttpHeaders {
    const t = this.token;
    return t ? new HttpHeaders({ Authorization: `Bearer ${t}` }) : new HttpHeaders();
  }
}
