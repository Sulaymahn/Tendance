import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

export interface Student {
  id: number;
  firstName: string;
  middleName: string | null;
  lastName: string;
  email: string;
  created: Date;
}

export interface StudentForCreation {
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

  get(): Observable<Student[]> {
    return this.http.get<Student[]>(`${environment.backendBaseUrl}students`);
  }

  create(student: StudentForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}students`, student);
  }

  delete(student: Student): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}students/${student.id}`);
  }
}
