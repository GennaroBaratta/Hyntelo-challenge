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
  /**
   * API endpoint for authentication.
   */
  private readonly authUrl = '/api/auth';

  /**
   * Subject to track the login status of the user.
   */
  private isLoggedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  /**
   * Subject to hold the current JWT token.
   */
  private tokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  /**
   * Initializes the service by checking if a token exists in cookies.
   * If a token is found, the login status and token are set.
   * 
   * @param http - HttpClient for making HTTP requests.
   * @param router - Router to handle navigation after logout.
   * @param cookieService - Service to manage cookies.
   */
  constructor(private http: HttpClient, private router: Router, private cookieService: CookieService) {
    const token = this.cookieService.get('authToken');
    if (token) {
      this.isLoggedInSubject.next(true);
      this.tokenSubject.next(token);
    }
  }

  /**
   * Authenticates the user by sending a request to the login API.
   * On success, stores the JWT token in cookies and updates login status.
   * 
   * @param username - The username provided by the user.
   * @param password - The password provided by the user.
   * @returns An observable that emits `true` if authentication succeeds, `false` otherwise.
   */
  authenticate(username: string, password: string): Observable<boolean> {
    return new Observable<boolean>((observer) => {
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

  /**
   * Returns an observable indicating whether the user is logged in.
   * 
   * @returns An observable of a boolean representing the login status.
   */
  isUserLoggedIn(): Observable<boolean> {
    return this.isLoggedInSubject.asObservable();
  }

  /**
   * Retrieves the current JWT token.
   * 
   * @returns The current JWT token or `null` if no token is stored.
   */
  getToken(): string | null {
    return this.tokenSubject.getValue();
  }

  /**
   * Retrieves the username from the stored JWT token.
   * 
   * @returns An observable of the username or `null` if not logged in.
   */
  getUsername(): Observable<string | null> {
    return this.getUserInfo().pipe(
      map(userInfo => userInfo ? userInfo.username : null)
    );
  }

  /**
   * Decodes the JWT token and extracts the user information.
   * 
   * @returns An observable with the user ID and username if the token is valid, or `null` if not logged in.
   */
  getUserInfo(): Observable<{ userId: string; username: string } | null> {
    return new Observable<{ userId: string; username: string } | null>((observer) => {
      const token = this.getToken();
      if (token) {
        try {
          const decoded: JwtPayload = jwtDecode(token);
          const userId = decoded.sub;
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

  /**
   * Logs out the user by deleting authentication-related cookies and resetting login status.
   * Redirects the user to the login page after logout.
   */
  logout(): void {
    this.cookieService.delete('rememberMe');
    this.cookieService.delete('authToken');
    this.isLoggedInSubject.next(false);
    this.tokenSubject.next(null);
    this.router.navigate(['/login']);
  }

  /**
   * Attaches the JWT token to the request headers for authenticated requests.
   * 
   * @returns An object containing the Authorization header with the Bearer token, or an empty object if no token exists.
   */
  getAuthHeaders(): { [header: string]: string } {
    const token = this.getToken();
    return token ? { Authorization: `Bearer ${token}` } : {};
  }
}
