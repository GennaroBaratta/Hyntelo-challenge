import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from './services/auth.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
  title = 'hyntelo-challenge.client';

  constructor(private http: HttpClient, private router: Router, private cookieService: CookieService, private authService: AuthService) {


  }

  ngOnInit(): void {
    const rememberMe = this.cookieService.get('rememberMe') === 'true';
    const token = this.authService.getToken();


    if (!(rememberMe && token)) {
      this.router.navigate(['/login']);
    } else  {
      this.router.events
        .pipe(filter(event => event instanceof NavigationEnd))
        .subscribe((event) => {
          if (event instanceof NavigationEnd && event.url === '/') {
            this.router.navigate(['/posts']);
          }
        });
    }
  }
}
