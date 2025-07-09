import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Teacher {
  id: number;
  firstName: string;
  middleName: string | null;
  lastName: string;
  email: string;
  attendanceRate: number;
  created: string;
}

export interface TeacherMinimal {
  id: number;
  firstName: string;
  middleName: string | null;
  lastName: string;
}

export class TeacherForCreation {
  firstName: string = '';
  middleName: string | null = null;
  lastName: string = '';
  email: string = '';
}

@Injectable({
  providedIn: 'root'
})
export class TeacherService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<Teacher[]> {
    return this.http.get<Teacher[]>(`${environment.backendBaseUrl}teachers`);
  }

  getAllMinimal(): Observable<TeacherMinimal[]> {
    return this.http.get<Teacher[]>(`${environment.backendBaseUrl}teachers`, {
      headers: {
        'X-Minimal': "true"
      }
    });
  }

  create(teacher: TeacherForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}teachers`, teacher);
  }

  delete(teacher: Teacher): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}teachers`, {
      headers: {
        'X-Teacher-Id': teacher.id.toString()
      }
    });
  }
}
