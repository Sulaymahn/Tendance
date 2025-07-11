import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { ClassroomMinimal } from '../classroom/classroom.service';

export interface ClassroomSession {
  id: string;
  classroom: ClassroomMinimal;
  topic: string | null;
  from: string;
  to: string;
  checkInFrom: string;
  checkInTo: string;
  checkOutFrom: string;
  checkOutTo: string;
  note: string | null;
  created: string;
}

export interface ClassroomSessionForCreation {
  classroomId: number;
  topic: string | null;
  from: string;
  to: string;
  checkInFrom: string;
  checkInTo: string;
  checkOutFrom: string;
  checkOutTo: string;
  note: string | null;
  timezone: string;
}

@Injectable({
  providedIn: 'root'
})
export class ClassroomSessionService {
  private http = inject(HttpClient);

  getAll(): Observable<ClassroomSession[]> {
    return this.http.get<ClassroomSession[]>(`${environment.backendBaseUrl}sessions`);
  }

  create(session: ClassroomSessionForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}sessions`, session);
  }

  delete(session: ClassroomSession): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}sessions`, {
      headers: {
        'X-Classroom-Session-Id': session.id
      }
    });
  }
}
