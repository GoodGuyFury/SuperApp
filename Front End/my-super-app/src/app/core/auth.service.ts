// auth.service.ts

import { Injectable } from '@angular/core';

// Add this interface at the top of the file
export interface AuthResponse {
  verificationResult: {
    status: string;
    message: string;
  };
  userInfo: {
    firstName: string;
    role: string;
    lastName: string;
    userId: string;
    email: string;
  };
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private isAuthenticated: boolean = false;
  private userRole: string | null = null;
  private userEmail: string | null = null;
  private userId: string | null = null;
  private firstName: string | null = null;
  private lastName: string | null = null;
  private justloggedOut: boolean = false;

  handleAuthResponse(data: AuthResponse): void {
    if (data?.verificationResult?.status.toLowerCase() === "authorized") {
      this.isAuthenticated = true;
      this.setAuthData(data.userInfo);
    } else {
      this.clearAuthData();
      return;
    }
  }

  setAuthData(userInfo: AuthResponse['userInfo']): void {
    this.userRole = userInfo.role?.toUpperCase() || null;
    this.userEmail = userInfo.email || null;
    this.userId = userInfo.userId || null;
    this.firstName = userInfo.firstName || null;
    this.lastName = userInfo.lastName || null;
    this.justloggedOut = false;
  }

  clearAuthData(): void {
    this.isAuthenticated = false;
    this.userRole = null;
    this.userEmail = null;
    this.userId = null;
    this.firstName = null;
    this.lastName = null;
    this.justloggedOut = true;
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
    return (this.firstName + '' + this.lastName);
  }

  isAdmin(): boolean {
    return this.userRole === 'ADMIN';
  }

  getJustLoggedOut(): boolean {
    return this.justloggedOut;
  }

  // Remove loadAuthData method as we're no longer using localStorage
}
