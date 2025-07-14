import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

export interface Student {
  id: string;
  firstName: string;
  middleName: string | null;
  lastName: string;
}

export interface RegisterResult {
  success: boolean;
  error: string;
  message: string;
}

export interface MatchResult {
  matchId: number | null;
  role: string | null;
  success: boolean;
  error: string;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class StudentsService {
  private readonly http = inject(HttpClient);

  get(): Observable<Student[]> {
    return this.http.get<Student[]>(`${environment.backendBaseUrl}students`, {
      headers: {
        'Authorization': `DeviceAuth ${environment.clientKey}`
      }
    });
  }

  getById(id: number): Observable<Student> {
    return this.http.get<Student>(`${environment.backendBaseUrl}students/${id}`, {
      headers: {
        'Authorization': `DeviceAuth ${environment.clientKey}`
      }
    });
  }

  register(student: Student, image: File): Observable<RegisterResult> {
    const formData = new FormData();
    formData.append('File', image, image.name);
    formData.append('UserRole', 'Student');
    formData.append('UserId', student.id.toString());

    return this.http.post<RegisterResult>(`${environment.backendBaseUrl}capture/register`, formData, {
      headers: {
        'Authorization': `DeviceAuth ${environment.clientKey}`,
      },
    });
  }

  macth(image: File): Observable<MatchResult> {
    const formData = new FormData();
    formData.append('file', image, image.name);

    return this.http.post<MatchResult>(`${environment.backendBaseUrl}capture`, formData, {
      headers: {
        'Authorization': `DeviceAuth ${environment.clientKey}`
      },
    });
  }
}
