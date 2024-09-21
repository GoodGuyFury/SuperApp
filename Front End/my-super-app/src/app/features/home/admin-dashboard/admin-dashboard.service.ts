import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../enviorments/enviorment';

@Injectable({
  providedIn: 'root'
})
export class AdminDashboardService {

  constructor(private http: HttpClient) { }

  fetchUserList(searchText: string): Observable<any> {
    const apiUrl = `${environment.apiUrl}/admin/fetchuserlist`;
    return this.http.post<any>(apiUrl, JSON.stringify(searchText), {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }
}
