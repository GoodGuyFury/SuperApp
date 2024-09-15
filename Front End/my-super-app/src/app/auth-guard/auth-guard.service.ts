// auth-guard.service.ts

import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return false;
    }

    const userRole = this.authService.getUserRole();
    const requestedUrl = state.url;

    if (requestedUrl.startsWith('/admin')) {
      if (userRole?.toLowerCase() === 'admin') {
        return true;
      } else {
        this.router.navigate(['/home']);
        return false;
      }
    }

    if (requestedUrl === '/home') {
      return true; // Both admin and regular members can access the home page
    }

    // For any other routes, redirect to home
    this.router.navigate(['/home']);
    return false;
  }
}
