// auth.service.ts

import { Injectable } from '@angular/core';

// Add this interface at the top of the file
export interface AuthResponse {
  verificationResult: {
    status: string;
    message: string;
  };
  userInfo: {
    fullName: string;
    role: string;
    message: string;
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
  private fullName: string | null = null;
  private userMessage: string | null = null;
  private justloggedOut: boolean = false;

  handleAuthResponse(data: AuthResponse): void {
    if (data?.verificationResult?.status.toLowerCase() === "authorized") {
      this.setAuthData(data.userInfo);
    } else {
      this.clearAuthData();
    }
  }

  setAuthData(userInfo: AuthResponse['userInfo']): void {
    this.isAuthenticated = true;
    this.userRole = userInfo.role?.toUpperCase() || null;
    this.userEmail = userInfo.email || null;
    this.userId = userInfo.userId || null;
    this.fullName = userInfo.fullName || null;
    this.userMessage = userInfo.message || null;
    this.justloggedOut = false;
  }

  clearAuthData(): void {
    this.isAuthenticated = false;
    this.userRole = null;
    this.userEmail = null;
    this.userId = null;
    this.fullName = null;
    this.userMessage = null;
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
    return this.fullName;
  }

  isAdmin(): boolean {
    return this.userRole === 'ADMIN';
  }

  getUserMessage(): string | null {
    return this.userMessage;
  }

  getJustLoggedOut(): boolean {
    return this.justloggedOut;
  }

  // Remove loadAuthData method as we're no longer using localStorage
}
