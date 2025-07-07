import { inject, Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import {
  BehaviorSubject,
  catchError,
  filter,
  finalize,
  map,
  Observable,
  switchMap,
  take,
  tap,
  throwError
} from 'rxjs';
import { HttpClient, HttpEvent, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

export class SignupCredential {
  schoolName: string = '';
  schoolEmail: string = '';
  username: string = '';
  email: string = '';
  password: string = '';
}

export class LoginCredential {
  email: string = '';
  password: string = '';
}

export class AuthorizationToken {
  accessToken: string = '';
  refreshToken: string = '';
}

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private http = inject(HttpClient);
  private cookieService = inject(CookieService);

  accessToken: string | null = null;
  isRefreshing: boolean = false;
  refreshTokenSubject = new BehaviorSubject<string | null>(null);

  loginUrl = environment.backendBaseUrl + 'auth/login';
  signUpUrl = environment.backendBaseUrl + 'auth/register';
  refreshTokenUrl = environment.backendBaseUrl + 'auth/refresh';

  login(credential: LoginCredential): Observable<void> {
    return this.http.post<AuthorizationToken>(this.loginUrl, credential).pipe(
      catchError((error) => {
        console.error('Login failed: ', error);
        return throwError(() => error);
      }),
      map((response: AuthorizationToken) => {
        this.accessToken = response.accessToken;
        this.setLocalRefreshToken(response.refreshToken);
      })
    );
  }

  signup(credential: SignupCredential): Observable<void> {
    return this.http.post<AuthorizationToken>(this.signUpUrl, credential).pipe(
      map((response: AuthorizationToken) => {
        this.accessToken = response.accessToken;
        this.setLocalRefreshToken(response.refreshToken);
      }),
      catchError((error) => {
        console.error('Signup failed: ', error);
        return throwError(() => error);
      })
    );
  }

  logout(): void {
    this.accessToken = null;
    this.cookieService.delete('RTID');
    this.refreshTokenSubject.next(null);
  }

  refreshToken(): Observable<AuthorizationToken> {
    const refreshToken = this.getLocalRefreshToken();

    if (refreshToken == null) {
      return throwError(() => 'No refresh token found');
    }

    return this.http.post<AuthorizationToken>(this.refreshTokenUrl, {
      refreshToken: refreshToken
    }).pipe(
      tap((response: AuthorizationToken) => {
        this.accessToken = response.accessToken;
        this.setLocalRefreshToken(response.refreshToken);
        this.refreshTokenSubject.next(response.refreshToken);
      }),
      catchError((error) => {
        console.error('Refresh token failed: ', error);
        return throwError(() => error);
      })
    );
  }

  isAuthenticated(): boolean {
    return this.getLocalRefreshToken() != null;
  }

  getLocalRefreshToken(): string | null {
    const token = this.cookieService.get('RTID');
    return token.length > 0 ? token : null;
  }

  setLocalRefreshToken(token: string): void {
    this.cookieService.set('RTID', token, {
      path: '/',
      expires: 7,
      secure: environment.production,
      sameSite: 'Strict'
    });
  }
}

const reqWithAccessToken = (request: HttpRequest<unknown>, accessToken: string) => request.clone({
  headers: request.headers.append('Authorization', 'Bearer ' + accessToken)
});

export function authenticationInterceptor(request: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> {
  const auth = inject(AuthenticationService);
  const router = inject(Router);

  // Skip adding token for auth-related endpoints (like login or refresh)
  if (request.url.includes('auth')) {
    return next(request);
  }

  // If we have an access token, add it to the request
  if (auth.accessToken) {
    return next(reqWithAccessToken(request, auth.accessToken)).pipe(
      catchError((error) => {
        // Only handle 401 Unauthorized errors specifically
        if (error.status === 401) {
          // If already refreshing, wait for the new token
          if (auth.isRefreshing) {
            return auth.refreshTokenSubject.pipe(
              filter(token => token !== null),
              take(1),
              switchMap(() => {
                // Retry the request with the new token
                return next(reqWithAccessToken(request, auth.accessToken!));
              }),
              catchError(retryError => {
                // If retrying with new token also fails, pass through the error
                return throwError(() => retryError);
              })
            );
          } else {
            // Start the refresh process
            auth.isRefreshing = true;

            // Reset subject value before starting the refresh
            auth.refreshTokenSubject.next(null);

            return auth.refreshToken().pipe(
              switchMap((tokens: AuthorizationToken) => {
                // Retry the request with new token
                return next(reqWithAccessToken(request, tokens.accessToken));
              }),
              catchError(refreshError => {
                // If refresh fails, redirect to login
                auth.logout();
                router.navigate(['login']);
                return throwError(() => 'Session expired. Please login again.');
              }),
              finalize(() => {
                // Ensure isRefreshing is reset even if an error occurs
                auth.isRefreshing = false;
              })
            );
          }
        }

        // For all other errors (including conflict status 409), immediately pass them through
        // This ensures that non-401 errors don't get stuck in the interceptor
        return throwError(() => error);
      })
    );
  } else {
    // No access token, try to refresh
    if (auth.getLocalRefreshToken()) {
      // If already refreshing, wait for it to complete
      if (auth.isRefreshing) {
        return auth.refreshTokenSubject.pipe(
          filter(token => token !== null),
          take(1),
          switchMap(() => {
            // At this point we should have a valid access token
            if (auth.accessToken) {
              return next(reqWithAccessToken(request, auth.accessToken));
            } else {
              auth.logout();
              router.navigate(['login']);
              return throwError(() => 'Authentication failed');
            }
          }),
          catchError(waitError => {
            // Make sure any error here is also passed through
            return throwError(() => waitError);
          })
        );
      }

      // Start refresh process
      auth.isRefreshing = true;

      return auth.refreshToken().pipe(
        switchMap((tokens: AuthorizationToken) => {
          return next(reqWithAccessToken(request, tokens.accessToken));
        }),
        catchError(error => {
          auth.logout();
          router.navigate(['login']);
          return throwError(() => 'Authentication required');
        }),
        finalize(() => {
          auth.isRefreshing = false;
        })
      );
    } else {
      // No refresh token either, redirect to login
      auth.logout();
      router.navigate(['login']);
      return throwError(() => 'Authentication required');
    }
  }
}