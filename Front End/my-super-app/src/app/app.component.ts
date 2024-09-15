import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { LoginLogoutService } from './auth-module/services/login-logout.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, MatProgressSpinnerModule, CommonModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'my-super-app';
  isLoading = false;

  constructor(
    private loginLogoutService: LoginLogoutService,
    private router: Router
  ) {}

  ngOnInit() {
    this.appInitialize();
  }

  appInitialize() {
    console.log('App initializing');
    this.isLoading = true;
    this.loginLogoutService.appInitialize().subscribe({
      next: (response) => {
        if (response.verificationResult.status == 'authorized') {
          this.router.navigate(['/home']);
        } else {
          this.router.navigate(['/authentication']);
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('App initialization failed:', err);
        this.router.navigate(['/authentication']);
        this.isLoading = false;
      }
    });
  }
}




