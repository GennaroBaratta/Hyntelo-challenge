import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'hyntelo-challenge.client';

  constructor(private http: HttpClient, private router: Router, private cookieService: CookieService, private authService: AuthService) { }

  ngOnInit(): void {
    const rememberMe = this.cookieService.get('rememberMe') === 'true';
    const token = this.authService.getToken();

    if (!(rememberMe && token)) {
      this.router.navigate(['/login']);
    } else if (this.router.url == '/') {
      this.router.navigate(['/posts']);
    }
  }
}
