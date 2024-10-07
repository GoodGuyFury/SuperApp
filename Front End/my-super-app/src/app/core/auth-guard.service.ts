import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthResponse, AuthService } from './auth.service';
import { Observable, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../enviorments/enviorment'; // Adjust the path if necessary

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router,
    private http: HttpClient // Inject HttpClient to make API requests
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    const isLoggedIn = this.authService.isLoggedIn();
    const requestedUrl = state.url; // Get the requested URL
// debugger;
    if (!isLoggedIn) {
      // Call the API if the user is not logged in
      return this.http.get(`${environment.apiUrl}/appinitialize`).pipe(
        switchMap(response => {
          // Handle the authentication response
          this.authService.handleAuthResponse(response as AuthResponse);

          // Recheck the login status after handling the response
          const isLoggedInAfterAuth = this.authService.isLoggedIn();
          const isAdminAfterAuth = this.authService.isAdmin();

          if (isLoggedInAfterAuth) {
            // If the requested URL is for the admin dashboard
            if (requestedUrl === '/home/admin-dashboard' && isAdminAfterAuth) {
              return of(true);
            }

            // Allow access for logged-in users (admin or regular user)
            return of(true);
          }

          // Redirect to login if access is denied
          return of(this.router.createUrlTree(['/auth/login']));
        }),
        catchError(() => {
          // Handle any error, e.g., API failure
          return of(this.router.createUrlTree(['/auth/login']));
        })
      );
    } else {
      // Proceed with the regular checks if already logged in
      const isAdmin = this.authService.isAdmin();

      if (requestedUrl === '/home/admin-dashboard') {
        if (isLoggedIn && isAdmin) {
          return true;
        }
      } else {
        if (isLoggedIn) {
          return true;
        }
      }

      // Redirect to login if access is denied
      return this.router.createUrlTree(['/auth/login']);
    }
  }
}
