import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private key = 'jwt';
  constructor(private http: HttpClient) {}

  login(email: string, password: string) {
    return this.http.post<{ accessToken: string }>('http://localhost/Account/login', { email, password });
  }

  logout() { 
    localStorage.removeItem(this.key); 
    location.href = '/students/'; 
  }

  setToken(t: string) { 
    localStorage.setItem(this.key, t); 
  }

  get token() { 
    return localStorage.getItem(this.key); 
  }

  isLoggedIn(): boolean {
    const token = this.token;

    if (!token) {
      return false;
    }
    
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp;
      if (!exp) return true;

      const now = Math.floor(Date.now() / 1000);
      return exp > now;
    } catch (err) {
      console.error('Invalid token:', err);
      return false;
    }
  }

  get userId(): string | null {
    const t = this.token;
    if (!t) return null;
    try {
      const p = JSON.parse(atob(t.split('.')[1]));
      return (
        p.sub ||
        p.userId ||
        p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
        null
      );
    } catch {
      return null;
    }
  }

  get name(): string | null {
    const t = this.token;
    if (!t) return null;

    try {
      const p = JSON.parse(atob(t.split('.')[1]));
      return (
        p.name ||
        p['unique_name'] ||
        p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
        null
      );
    } catch {
      return null;
    }
  }

  role(): string | null {
    const t = this.token; 
    if (!t) {
      return null;
    }

    const p = JSON.parse(atob(t.split('.')[1]));
    return p.role || p['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;
  }

  authHeaders(): HttpHeaders {
    const t = this.token;
    return t ? new HttpHeaders({ Authorization: `Bearer ${t}` }) : new HttpHeaders();
  }
}
