import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';

export interface StudentDto {
  id: string;
  name: string;
  email?: string;
}

export interface GradeDto {
  id: string;
  course: string;
  value: number;
  studentId: string;
}

@Injectable({ providedIn: 'root' })
export class ProfessorApiService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  private studentsBase = '/Students';
  private gradesBase = '/Grades';

  listStudents() {
    return this.http.get<StudentDto[]>(`${this.studentsBase}`, {
      headers: this.auth.authHeaders(),
    });
  }

  listGradesForStudent(studentId: string) {
    return this.http.get<GradeDto[]>(`${this.gradesBase}/student/${studentId}`, {
      headers: this.auth.authHeaders(),
    });
  }

  addGrade(dto: Omit<GradeDto, 'id'>) {
    return this.http.post<GradeDto>(`${this.gradesBase}`, dto, {
      headers: this.auth.authHeaders(),
    });
  }

  updateGrade(id: string, dto: Partial<GradeDto>) {
    return this.http.put<GradeDto>(`${this.gradesBase}/${id}`, dto, {
      headers: this.auth.authHeaders(),
    });
  }

  deleteGrade(id: string) {
    return this.http.delete<void>(`${this.gradesBase}/${id}`, {
      headers: this.auth.authHeaders(),
    });
  }
}
