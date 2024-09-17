import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap, catchError, of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthResponse, AuthService } from '../../../auth.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class LoginLogoutService {
  private apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  signInWithGoogle(jwt: string): Observable<AuthResponse> {
    const headers = new HttpHeaders().set('googjwt', jwt);
    return this.http.get<AuthResponse>(`${this.apiUrl}SignInWithGoogle`, { headers }).pipe(
      tap(data => this.authService.handleAuthResponse(data))
    );
  }

  appInitialize(): Observable<AuthResponse> {
    return this.http.get<AuthResponse>(`${this.apiUrl}appinitialize`).pipe(
      tap(data => this.authService.handleAuthResponse(data))
    );
  }

  logout(): Observable<string> {
    return this.http.get(`${this.apiUrl}logout`, { responseType: 'text' }).pipe(
      tap((response: string) => {
        if (response === 'Logged out successfully.') {
          this.authService.clearAuthData();
          this.showLogoutSuccessPopup();
          this.router.navigate(['/authentication']);
        }
      }),
      catchError((error) => {
        console.error('Logout failed', error);
        this.showLogoutErrorPopup();
        return of('Logout failed');
      })
    );
  }

  private showLogoutSuccessPopup() {
    this.snackBar.open('Logged out successfully', 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'top',
    });
  }

  private showLogoutErrorPopup() {
    this.snackBar.open('Logout failed. Please try again.', 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'top',
    });
  }
}
