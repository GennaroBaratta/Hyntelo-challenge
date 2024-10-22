import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { map } from 'rxjs/operators';

import { jwtDecode } from "jwt-decode";
import { CookieService } from 'ngx-cookie-service';

interface JwtPayload {
  sub: string; // user ID
  username: string;
  exp: number; // expiry time
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly authUrl = '/api/auth';

  private isLoggedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private tokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  constructor(private http: HttpClient, private router: Router, private cookieService: CookieService) {
    const token = this.cookieService.get('authToken');
    if (token) {
      this.isLoggedInSubject.next(true);
      this.tokenSubject.next(token);
    }
  }

  authenticate(username: string, password: string): Observable<boolean> {
    return new Observable<boolean>((observer) => {
      // API call to authenticate user
      this.http.post<{ token: string }>(`${this.authUrl}/login`, { username, password }).subscribe(
        (response) => {
          const token = response.token;

          // Store the token in a cookie
          this.cookieService.set('authToken', token, { expires: 1, secure: true, sameSite: 'Strict' });

          this.isLoggedInSubject.next(true);
          this.tokenSubject.next(token);

          observer.next(true);
          observer.complete();
        },
        () => {
          observer.next(false);
          observer.complete();
        }
      );
    });
  }

  isUserLoggedIn(): Observable<boolean> {
    return this.isLoggedInSubject.asObservable();
  }

  getToken(): string | null {
    return this.tokenSubject.getValue();
  }

  getUsername(): Observable<string | null> {
    return this.getUserInfo().pipe(
      map(userInfo => userInfo ? userInfo.username : null)
    );
  }

  getUserInfo(): Observable<{ userId: string; username: string } | null> {
    return new Observable<{ userId: string; username: string } | null>((observer) => {
      const token = this.getToken();
      if (token) {
        try {
          const decoded: any = jwtDecode(token);
          const userId = decoded.userId || decoded.sub; // Use custom claim or fallback to sub
          observer.next({ userId: userId, username: decoded.username });
        } catch (e) {
          console.error('Invalid token', e);
          observer.next(null);
        }
      } else {
        observer.next(null);
      }
      observer.complete();
    });
  }

  logout(): void {
    this.cookieService.delete('rememberMe')
    this.cookieService.delete('authToken');
    this.isLoggedInSubject.next(false);
    this.tokenSubject.next(null);
    this.router.navigate(['/login']);
  }

  // Attach the JWT token to the headers of the requests
  getAuthHeaders(): { [header: string]: string } {
    const token = this.getToken();
    return token ? { Authorization: `Bearer ${token}` } : {};
  }
}
