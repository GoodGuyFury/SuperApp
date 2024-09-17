import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CreateUserModel } from '../interfaces/admin-interface';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  createUser(userData: CreateUserModel): Observable<any> {
    const formData = new FormData();
    Object.keys(userData).forEach(key => {
      const value = userData[key as keyof CreateUserModel];
      if (value !== null && value !== undefined) {
        formData.append(key, value.toString());
      }
    });

    return this.http.post(`${this.apiUrl}admin/createuser`, formData);
  }

  fetchUsers(searchText?: string): Observable<any> {
    let params = new HttpParams();
    if (searchText) {
      params = params.set('searchText', searchText);
    }
    return this.http.get(`${this.apiUrl}admin/fetchuserlist`, { params });
  }
}
