import { Component } from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { SideNavComponent } from "../side-nav/side-nav.component";
import { LoginLogoutService } from '../../../auth-module/services/login-logout.service';

@Component({
  selector: 'app-general-home',
  standalone: true,
  imports: [HeaderComponent, SideNavComponent],
  templateUrl: './general-home.component.html',
  styleUrl: './general-home.component.scss'
})
export class GeneralHomeComponent {
  isSidenavOpen = false;

  constructor(private loginLogoutService: LoginLogoutService) {}

  toggleSidenav() {
    this.isSidenavOpen = !this.isSidenavOpen;
  }

  logout() {
    this.loginLogoutService.logout().subscribe({
      next: () => {
        console.log('Logged out successfully');
        window.location.reload();
        // Implement redirect to login page or other post-logout logic
      },
      error: (error) => {
        console.error('Logout failed', error);
        window.location.reload();
      }
    });
  }
}
