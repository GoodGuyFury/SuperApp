import { Component, OnInit } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { AuthHomeService } from './auth-home.service';
import { AuthService } from '../../../core/auth.service';

@Component({
  selector: 'app-auth-home',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatCardModule, MatButtonModule],
  templateUrl: './auth-home.component.html',
  styleUrl: './auth-home.component.scss'
})
export class AuthHomeComponent implements OnInit {
  constructor(
    private authHomeService: AuthHomeService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.authHomeService.initializeApp().subscribe({
      next: () => {
        if (this.authService.isLoggedIn()) {
          this.router.navigate(['/home/dashboard']);
        }
      },
      error: (error) => {
        console.error('Error initializing app:', error);
        // You might want to show an error message to the user here//
        this.router.navigate(['/auth/login']);
      }
    });
  }
}
