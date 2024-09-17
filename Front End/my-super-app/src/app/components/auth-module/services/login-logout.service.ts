import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthService } from '../../../auth.service';

@Injectable({
  providedIn: 'root'
})
export class LoginLogoutService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient, private authService: AuthService) { }

  signInWithGoogle(jwt: string): Observable<any> {
    const headers = new HttpHeaders().set('googjwt', jwt);
    return this.http.get<any>(this.apiUrl + 'SignInWithGoogle', { headers: headers }).pipe(
      tap(data => this.authService.handleAuthResponse(data))
    );
  }

  appInitialize(): Observable<any> {
    return this.http.get<any>(this.apiUrl + "appinitialize").pipe(
      tap(data => this.authService.handleAuthResponse(data))
    );
  }

  logout(): Observable<any> {
    return this.http.get<any>(this.apiUrl + "logout").pipe(
      tap(() => this.authService.clearAuthData())
    );
  }
}
