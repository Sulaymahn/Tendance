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
  created: string;
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

  get(): Observable<Teacher[]> {
    return this.http.get<Teacher[]>(`${environment.backendBaseUrl}teachers`);
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
