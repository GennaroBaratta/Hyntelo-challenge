import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  private readonly validUsername = 'admin';
  private readonly validPassword = 'password';

  authenticate(username: string, password: string): boolean {
    if (username === this.validUsername && password === this.validPassword) {
      if (localStorage) {
        localStorage.setItem('isLoggedIn', 'true');
      }
      return true;
    }
    return false;
  }

  isUserLoggedIn(): boolean {
    return localStorage && localStorage.getItem('isLoggedIn') === 'true';
  }

  logout(): void {
    if (localStorage) {
      localStorage.removeItem('isLoggedIn');
    }
  }
}
