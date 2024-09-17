import { Component, Output, EventEmitter } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { LoginLogoutService } from '../../../auth-module/services/login-logout.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [MatToolbarModule, MatButtonModule, MatIconModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  @Output() toggleSidenav = new EventEmitter<void>();

  constructor(
    private loginLogoutService: LoginLogoutService,
    private router: Router
  ) {}

  logout() {
    this.loginLogoutService.logout().subscribe({
      next: () => {

      },
      error: (error) => {
        console.error('Logout failed', error);
        // Handle logout error (e.g., show an error message to the user)
      }
    });
  }
}
