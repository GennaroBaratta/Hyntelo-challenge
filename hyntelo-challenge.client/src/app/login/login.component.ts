import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { CookieService } from 'ngx-cookie-service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NotificationService } from '../services/notification.service';  // Import the NotificationService

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  rememberMe: boolean = false;
  loading: boolean = false;
  errorMessage: string = '';

  /**
   * Constructor for LoginComponent.
   * 
   * @param authService - Service used to authenticate the user.
   * @param router - Router used for navigation after successful login.
   * @param cookieService - Service used to handle cookies (for "remember me" functionality).
   * @param snackBar - Angular Material service for showing error messages.
   */
  constructor(
    private authService: AuthService,
    private router: Router,
    private cookieService: CookieService,
    private snackBar: MatSnackBar,
    private notification: NotificationService
  ) { }

  /**
   * Handles the login process by authenticating the user.
   * If the credentials are correct, the user is redirected to the posts page.
   * If 'Remember Me' is selected, a cookie is set for persistent login.
   */
  async login(): Promise<void> {
    this.loading = true;
    this.errorMessage = '';

    this.authService.authenticate(this.username, this.password).subscribe(
      (isAuthenticated) => {
        this.loading = false;
        if (isAuthenticated) {
          // Set cookie for remember me
          if (this.rememberMe) {
            this.cookieService.set('rememberMe', 'true', { expires: 7, secure: true, sameSite: 'Strict' });
          }

          // Navigate to posts page upon successful login
          this.router.navigate(['/posts']);
        } else {
          this.notification.showError('Invalid credentials');
        }
      },
      (error) => {
        this.loading = false;
        this.notification.showError('An error occurred. Please try again.');
      }
    );
  }
}
