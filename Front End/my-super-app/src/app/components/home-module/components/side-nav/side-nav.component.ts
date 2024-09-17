import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Location } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatExpansionModule } from '@angular/material/expansion';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../auth.service';

@Component({
  selector: 'app-side-nav',
  standalone: true,
  imports: [CommonModule, MatSidenavModule, MatListModule, MatExpansionModule, RouterModule],
  templateUrl: './side-nav.component.html',
  styleUrls: ['./side-nav.component.scss']
})
export class SideNavComponent {
  @Input() isOpen = false;
  @Input() currentTab = 'dashboard';
  @Output() tabChange = new EventEmitter<string>();

  constructor(private location: Location, public authService: AuthService) {}

  changeTab(tab: string) {
    this.tabChange.emit(tab);

    // Update URL without navigating
    const url = `/home/${tab}`;
    this.location.go(url);
  }

  isAdmin(): boolean {
    // Implement your admin check logic here
    return true; // Placeholder return value
  }
}
