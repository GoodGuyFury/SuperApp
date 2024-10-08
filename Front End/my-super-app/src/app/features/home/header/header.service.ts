import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'; // Import HttpClient
import { AuthService } from '../../../core/auth.service';
import { Router } from '@angular/router';
import { environment } from '../../../../enviorments/enviorment';

@Injectable({
  providedIn: 'root'
})
export class HeaderService {

  constructor(private authService: AuthService, private router: Router, private http: HttpClient) {} // Inject HttpClient

  logout() { // Add logout method
    const logoutUrl = `${environment.apiUrl}/logout`; // Construct logout URL
    this.http.get(logoutUrl, { responseType: 'json' }).subscribe({ // Call logout API with text response
      next: (response) => { // Handle successful response
        console.log('Logout response:', response); // Log the string response
        this.authService.clearAuthData(); // Call logout method from AuthService
        this.router.navigate(['/auth/login']); // Navigate to logout path
      },
      error: (err) => { // Handle error response
        console.error('Logout error:', err); // Log error
      }
    });
  }
}
