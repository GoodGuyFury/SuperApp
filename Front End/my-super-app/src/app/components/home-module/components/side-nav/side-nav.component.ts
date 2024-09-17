import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
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
  @ViewChild('sidenav') sidenav!: MatSidenav;
  @Input() isOpen = false;
  @Input() currentTab = 'dashboard';
  @Output() tabChange = new EventEmitter<string>();

  constructor(private location: Location, public authService: AuthService) {}

  onNavItemClick(tab: string) {
    this.changeTab(tab);
    this.tabChange.emit(tab);
  }

  changeTab(tab: string) {
    this.currentTab = tab;
    // Add any additional logic for changing tabs
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }
}
