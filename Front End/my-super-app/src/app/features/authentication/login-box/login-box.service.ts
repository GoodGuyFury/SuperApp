import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../enviorments/enviorment';
import { AuthService, AuthResponse } from '../../../core/auth.service';

@Injectable({
  providedIn: 'root'
})
export class LoginBoxService {
  private apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) { }

  directLogin(email: string, password: string): Observable<AuthResponse> {
    const url = `${this.apiUrl}/login/directlogin`;

    const formData = new FormData();
    formData.append('email', email);
    formData.append('password', password);

    const headers = new HttpHeaders({
      'Accept': 'application/json'
    });

    return this.http.post<AuthResponse>(url, formData, { headers }).pipe(
      tap(response => this.authService.handleAuthResponse(response))
    );
  }
}
