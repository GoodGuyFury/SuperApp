import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/auth.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { SidenavService } from '../sidenav/sidenav.service';
import { HeaderService } from './header.service'; // Import HeaderService

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatToolbarModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  userName: string | null = null;

  constructor(private authService: AuthService, private sidenavService: SidenavService, private headerService: HeaderService) {} // Inject HeaderService

  ngOnInit() {
    this.userName = this.authService.getFullName();
  }

  toggleSidenav() {
    this.sidenavService.toggle();
  }

  logout() { // Add logout method
    this.headerService.logout(); // Call logout method from HeaderService
  }
}
