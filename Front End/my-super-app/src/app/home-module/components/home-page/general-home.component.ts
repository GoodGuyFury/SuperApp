import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { HeaderComponent } from '../header/header.component';
import { SideNavComponent } from "../side-nav/side-nav.component";
import { LoginLogoutService } from '../../../auth-module/services/login-logout.service';
import { AdminHomeComponent } from '../admin-tab/admin-home.component';
import { CommonModule } from '@angular/common';
import { DashboardComponent } from '../dashboard/dashboard.component';

@Component({
  selector: 'app-general-home',
  standalone: true,
  imports: [HeaderComponent, SideNavComponent,AdminHomeComponent,CommonModule,DashboardComponent],
  templateUrl: './general-home.component.html',
  styleUrls: ['./general-home.component.scss']
})
export class GeneralHomeComponent implements OnInit {
  isSidenavOpen = false;
  currentTab = 'dashboard';

  constructor(private loginLogoutService: LoginLogoutService, private location: Location, private router: Router) {}

  ngOnInit() {
    // Set the initial URL to /home/dashboard
    this.location.go('/home/dashboard');
  }

  toggleSidenav() {
    this.isSidenavOpen = !this.isSidenavOpen;
  }

  logout() {
    this.loginLogoutService.logout().subscribe({
      next: () => {
        console.log('Logged out successfully');
        this.router.navigate(['/login']);
      },
      error: (error) => {
        console.error('Logout failed', error);
        // Optionally, you can still navigate to login on error
        this.router.navigate(['/login']);
      }
    });
  }

  onTabChange(tab: string) {
    this.currentTab = tab;
    this.location.go(`/home/${tab}`);
  }
}
