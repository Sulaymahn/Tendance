import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Student {
  id: string;
  firstName: string;
  middleName: string | null;
  lastName: string;
  email: string;
  attendanceRate: number;
  created: Date;
}

export interface StudentMinimal {
  id: string;
  firstName: string;
  middleName: string | null;
  lastName: string;
}

export interface StudentForCreation {
  id: string;
  firstName: string;
  middleName: string | null;
  lastName: string;
  email: string;
}

@Injectable({
  providedIn: 'root'
})
export class StudentService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<Student[]> {
    return this.http.get<Student[]>(`${environment.backendBaseUrl}students`);
  }

  getAllMinimal(): Observable<StudentMinimal[]> {
    return this.http.get<Student[]>(`${environment.backendBaseUrl}students`, {
      headers: {
        'X-Minimal': "true"
      }
    });
  }

  create(student: StudentForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}students`, student);
  }

  delete(student: Student): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}students`, {
      headers: {
        'X-Student-Id': student.id
      }
    });
  }
}
