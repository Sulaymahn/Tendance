import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export class User {
  username: string = '';
  email: string = '';
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly http = inject(HttpClient);

  get(): Observable<User> {
    return this.http.get<User>(`${environment.backendBaseUrl}user`);
  }
}
