// auth.service.ts

import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private isAuthenticated: boolean = false;
  private userRole: string | null = null;
  private userEmail: string | null = null;

  handleAuthResponse(data: any): void {
    if (data.verificationResult?.status.toLowerCase() === "authorized") {
      this.setAuthData(data.userInfo);
    } else {
      this.clearAuthData();
    }
  }

  setAuthData(userInfo: any): void {
    this.isAuthenticated = true;
    this.userRole = userInfo.role;
    this.userEmail = userInfo.email;
    // You might want to store this info in localStorage as well
  }

  clearAuthData(): void {
    this.isAuthenticated = false;
    this.userRole = null;
    this.userEmail = null;
    // Clear from localStorage if you're using it
  }

  isLoggedIn(): boolean {
    return this.isAuthenticated;
  }

  getUserRole(): string | null {
    return this.userRole;
  }

  getUserEmail(): string | null {
    return this.userEmail;
  }
  isAdmin(): boolean {
    return this.userRole?.toLowerCase() === 'admin';
  }
}
