// auth.service.ts

import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private isAuthenticated: boolean = false;

  constructor() {}

  // Simulate login (replace with actual login logic)
  login(username: string, password: string): boolean {
    // Replace with actual authentication logic
    if (username === 'user' && password === 'password') {
      this.isAuthenticated = true;
      return true;
    } else {
      this.isAuthenticated = false;
      return false;
    }
  }

  // Simulate logout (replace with actual logout logic)
  logout(): void {
    this.isAuthenticated = false;
  }

  // Check if user is authenticated
  isLoggedIn(): boolean {
    return this.isAuthenticated;
  }
}
