import { Component, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatListModule } from '@angular/material/list';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { SidenavService } from './sidenav.service';
import { AuthService } from '../../../core/auth.service';

@Component({
  selector: 'app-sidenav',
  standalone: true,
  imports: [CommonModule, RouterLink, MatListModule, MatSidenavModule],
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss']
})
export class SidenavComponent {
   @ViewChild('sidenav') sidenav!: MatSidenav;

   constructor(private sidenavService: SidenavService, private authService: AuthService) {}
    isAdmin : boolean = false;
    isSuperAdmin : boolean = false;
   ngOnInit() {
    this.isAdmin = this.authService.isAdmin();
    this.isSuperAdmin = this.authService.isSuperAdmin();
     this.sidenavService.sidenavToggle$.subscribe(() => {
       if (this.sidenav) {
         this.sidenav.toggle();
       }
     });
   }
  open() {
    this.sidenav.open();
  }

  close() {
    this.sidenav.close();
  }
}
