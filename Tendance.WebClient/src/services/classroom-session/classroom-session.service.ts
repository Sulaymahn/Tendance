import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Classroom } from '../classroom/classroom.service';

export interface ClassroomSession {
  id: string;
  classroom: Classroom;
  topic: string | null;
  from: string;
  to: string;
  checkInFrom: string;
  checkInTo: string;
  checkOutFrom: string;
  checkOutTo: string;
  note: string | null;
  created: string;
  attendances: ClassroomSessionAttendance[];
}

export interface ClassroomSessionAttendance {
  userId: number;
  firstName: string;
  middleName: string | null;
  lastName: string;
  role: string;
  checkedIn: boolean;
  checkedOut: boolean;
  checkInTimeStamp: string;
  checkOutTimeStamp: string;
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

  get(): Observable<ClassroomSession[]> {
    return this.http.get<ClassroomSession[]>(`${environment.backendBaseUrl}sessions`);
  }

  getById(id: number): Observable<ClassroomSession> {
    return this.http.get<ClassroomSession>(`${environment.backendBaseUrl}sessions/${id}`);
  }

  create(session: ClassroomSessionForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}sessions`, session);
  }

  delete(session: ClassroomSession): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}sessions/`);
  }
}
