import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../services/login.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  rememberMe: boolean = false;

  constructor(private loginService: LoginService, private router: Router) { }

  login(): void {
    if (this.loginService.authenticate(this.username, this.password)) {
      this.router.navigate(['/posts']);
    } else {
      alert('Invalid credentials');
    }
  }
}
