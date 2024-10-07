import { environment } from './../../enviorments/enviorment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SuperServiceService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}


   checkRouteAcess(): boolean{
    return false;
   }
   appInitialize(): Observable<any> {
    return this.http.get<any>(this.apiUrl + "appinitialize");
  }
}
