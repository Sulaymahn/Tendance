import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Classroom {
  id: number;
  created: string;
  room: ClassroomRoom;
  course: ClassroomCourse;
  teacher: ClassroomTeacher;
  students: ClassroomStudent[];
}

export interface ClassroomForCreation {
  courseId: number;
  teacherId: number;
  roomId: number;
}

export interface ClassroomCourse {
  id: string;
  name: string;
}

export interface ClassroomTeacher {
  id: string;
  firstName: string;
  middleName: string | null;
  lastName: string;
}

export interface ClassroomStudent {
  id: number;
  firstName: string;
  middleName: string | null;
  lastName: string;
}

export interface ClassroomRoom {
  id: number;
  name: string;
  building: string;
}

@Injectable({
  providedIn: 'root'
})
export class ClassroomService {
  private http = inject(HttpClient);

  get(): Observable<Classroom[]> {
    return this.http.get<Classroom[]>(`${environment.backendBaseUrl}classrooms`);
  }

  getById(id: number): Observable<Classroom> {
    return this.http.get<Classroom>(`${environment.backendBaseUrl}classrooms/${id}`);
  }

  updateStudents(classroom: Classroom, ids: number[]): Observable<void> {
    return this.http.put<void>(`${environment.backendBaseUrl}classrooms/${classroom.id}/students`, ids);
  }

  create(classroom: ClassroomForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}classrooms`, classroom);
  }

  delete(classroom: Classroom): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}classrooms/${classroom.id}`);
  }
}
