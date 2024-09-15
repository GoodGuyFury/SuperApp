import { Component, OnInit, ViewChild, ElementRef, NgZone, EventEmitter, Output } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { LoginLogoutService } from '../../services/login-logout.service';
import { AuthService } from '../../../auth.service';

declare var google: any;

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.scss'],
  standalone: true,
  imports: []
})
export class SignInComponent implements OnInit {

  @Output() signInResponse = new EventEmitter<boolean>();

  constructor(
    private loginLogoutService: LoginLogoutService,
    private authService: AuthService,
    private router: Router,
    private ngZone: NgZone,
  ) { }

  userData: any;

  @ViewChild('googleBtn', { static: true }) googleBtn!: ElementRef;

  ngOnInit(): void {
    this.renderGoogleSignInButton();
  }

  private renderGoogleSignInButton(): void {
    if (typeof google !== 'undefined' && google.accounts && google.accounts.id) {
      google.accounts.id.initialize({
        client_id: environment.googleClientId,
        callback: (response: any) => {
          this.ngZone.run(() => {
            this.verifyGoogleSignin(response.credential);
          });
        }
      });
      google.accounts.id.renderButton(
        this.googleBtn.nativeElement,
        {
          theme: 'outline',
          size: 'large',
          width: 100
        }
      );
    } else {
      console.error('Google Sign-In API not loaded');
    }
  }

  async verifyGoogleSignin(googleCred: any) {
    this.loginLogoutService.signInWithGoogle(googleCred).subscribe({
      next: (data) => {
        if (data.verificationResult.status === "authorized") {
          this.router.navigate(['/home']);
        }
      },
      error: (error) => {
        console.error('Error fetching data:', error);
        // Handle error (e.g., show error message to user)
      }
    });
  }

  createNewAccount() {
    this.signInResponse.emit(true);
  }
}
