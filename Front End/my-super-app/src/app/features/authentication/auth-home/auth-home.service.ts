import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../enviorments/enviorment';
import { AuthService, AuthResponse } from '../../../core/auth.service';
import { Observable, catchError, tap, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthHomeService {

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) { }

  initializeApp(): Observable<AuthResponse> {
    return this.http.get<AuthResponse>(`${environment.apiUrl}/appinitialize`).pipe(
      tap(response => {
        this.authService.handleAuthResponse(response);
      }),
      catchError(error => {
        return this.handleError(error);
      })
    );
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof Error) {
      // Client-side or network error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Backend returned an unsuccessful response code
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    return throwError(() => new Error(errorMessage));
  }
}
