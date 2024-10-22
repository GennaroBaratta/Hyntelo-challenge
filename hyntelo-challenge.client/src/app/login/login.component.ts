import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { CookieService } from 'ngx-cookie-service';

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

  constructor(private authService: AuthService, private router: Router, private cookieService: CookieService) { }

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
          this.errorMessage = 'Invalid credentials';
        }
      },
      (error) => {
        this.loading = false;
        this.errorMessage = 'An error occurred. Please try again.';
      }
    );
  }
}
