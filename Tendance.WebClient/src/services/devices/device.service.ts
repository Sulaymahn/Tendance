import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

export interface CaptureDevice {
  id: string;
  classroomId: number;
  nickname: string;
  clientKey: string;
  type: string;
  mode: string;
  created: string;
}

export interface CaptureDeviceMinimal {
  id: string;
  classroomId: number;
  nickname: string;
  type: string;
  mode: string;
  created: string;
}

export interface CaptureDeviceForCreation {
  classroomId: number | null;
  nickname: string;
  type: string;
}

@Injectable({
  providedIn: 'root'
})
export class DeviceService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<CaptureDevice[]> {
    return this.http.get<CaptureDevice[]>(`${environment.backendBaseUrl}devices`);
  }

  getAllMinimal(): Observable<CaptureDeviceMinimal[]> {
    return this.http.get<CaptureDeviceMinimal[]>(`${environment.backendBaseUrl}devices`, {
      headers: {
        'X-Minimal': 'true'
      }
    });
  }

  getCaptureDeviceTypes(): Observable<string[]> {
    return this.http.get<string[]>(`${environment.backendBaseUrl}devices/types`);
  }

  create(captureDevice: CaptureDeviceForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}devices`, captureDevice);
  }

  delete(captureDevice: CaptureDevice): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}devices`, {
      headers: {
        'X-Device-Id': captureDevice.id
      }
    });
  }
}
