import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../enviorment';


@Injectable({
  providedIn: 'root'
})
export class LoginLogoutService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }
  signInWithGoogle(jwt : string): Observable<any> {
    const headers = new HttpHeaders().set('googjwt', jwt);
    return this.http.get<any>(this.apiUrl+'SignInWithGoogle', { headers : headers});
  }
}
