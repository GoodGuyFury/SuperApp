import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    // If user is not logged in, redirect to authentication
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/authentication']); // Use the correct route here
      return false;
    }

    const userRole = this.authService.getUserRole();
    const requestedUrl = state.url;

    // If user tries to access an admin route

    if (requestedUrl.startsWith('/admin')) {
      if (userRole?.toLowerCase() === 'admin') {
        return true; // Admin is allowed
      } else {
        this.router.navigate(['/home']); // Non-admins are redirected to home
        return false;
      }
    }

    // Both admins and regular users can access the home page
    if (requestedUrl === '/home') {
      return true;
    }

    // For any other invalid routes, redirect to home
    this.router.navigate(['/home']);
    return false;
  }
}
