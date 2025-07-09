import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Course {
  id: number;
  name: string;
  description: string | null;
  created: Date;
}

export interface CourseMinimal {
  id: number;
  name: string;
}

export interface CourseForCreation {
  name: string;
  description: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class CourseService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<Course[]> {
    return this.http.get<Course[]>(`${environment.backendBaseUrl}courses`);
  }

  getAllMinimal(): Observable<CourseMinimal[]> {
    return this.http.get<CourseMinimal[]>(`${environment.backendBaseUrl}courses`, {
      headers: {
        'X-Minimal': "true"
      }
    });
  }

  create(course: CourseForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}courses`, course);
  }

  delete(course: Course): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}courses`, {
      headers: {
        'X-Course-Id': course.id.toString()
      }
    });
  }
}
