// auth.service.ts

import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private isAuthenticated: boolean = false;
  private userRole: string | null = null;
  private userEmail: string | null = null;
  private userId: string | null = null;
  private fullName: string | null = null;

  handleAuthResponse(data: any): void {
    if (data?.verificationResult?.status.toLowerCase() === "authorized") {
      this.setAuthData(data.userInfo);
    } else {
      this.clearAuthData();
    }
  }

  setAuthData(userInfo: any): void {
    this.isAuthenticated = true;
    this.userRole = userInfo.role?.toUpperCase() || null;
    this.userEmail = userInfo.email || null;
    this.userId = userInfo.userId || null;
    this.fullName = userInfo.fullName || null;
  }

  clearAuthData(): void {
    this.isAuthenticated = false;
    this.userRole = null;
    this.userEmail = null;
    this.userId = null;
    this.fullName = null;
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

  getUserId(): string | null {
    return this.userId;
  }

  getFullName(): string | null {
    return this.fullName;
  }

  isAdmin(): boolean {
    return this.userRole === 'ADMIN';
  }

  // Remove loadAuthData method as we're no longer using localStorage
}
