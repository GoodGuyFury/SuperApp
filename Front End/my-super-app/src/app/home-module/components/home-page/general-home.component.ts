import { Component } from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { SideNavComponent } from "../side-nav/side-nav.component";

@Component({
  selector: 'app-general-home',
  standalone: true,
  imports: [HeaderComponent, SideNavComponent],
  templateUrl: './general-home.component.html',
  styleUrl: './general-home.component.scss'
})
export class GeneralHomeComponent {
  isSidenavOpen = false;
  toggleSidenav() {
    this.isSidenavOpen = !this.isSidenavOpen;
  }

  logout() {
    // Implement logout logic here
    console.log('Logout clicked');
  }
}
