import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../enviorments/enviorment';

interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  directLogin(username: string, password: string): Observable<LoginResponse> {
    debugger;
    const url = `${this.apiUrl}/login/directlogin`;

    const formData = new FormData();
    formData.append('username', username);
    formData.append('password', password);

    const headers = new HttpHeaders({
      'Accept': 'application/json'
    });

    return this.http.post<LoginResponse>(url, formData, { headers });
  }
}
