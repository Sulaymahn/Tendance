import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Room {
  id: number;
  name: string;
  building: string;
}

export interface RoomForCreation {
  name: string;
  building: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private readonly http = inject(HttpClient);

  get(): Observable<Room[]> {
    return this.http.get<Room[]>(`${environment.backendBaseUrl}rooms`);
  }

  create(room: RoomForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}rooms`, room);
  }

  delete(room: Room): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}rooms`, {
      headers: {
        'X-Course-Id': room.id.toString()
      }
    });
  }
}
