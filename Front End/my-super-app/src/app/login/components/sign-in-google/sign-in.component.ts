import { Component, OnInit, ViewChild, ElementRef, NgZone, EventEmitter, Output } from '@angular/core';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { environment } from '../../../../environments/environment';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Location } from '@angular/common';
import { LoginLogoutService } from '../../services/login-logout.service';

declare var google :any;

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.scss'],
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, RouterModule]
})
export class SignInComponent implements OnInit {

  @Output() signInResponse = new EventEmitter<boolean>();

  constructor(private loginLogout: LoginLogoutService, private ngZone: NgZone, private location: Location, private router: Router) { }

  userData: any;

  @ViewChild('googleBtn', { static: true }) googleBtn!: ElementRef;

  ngOnInit(): void {
    // Render the Google Sign-In button as soon as the component initializes
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
          // Set the width to match the container
        }
      );
    } else {
      console.error('Google Sign-In API not loaded');
    }
  }

  async verifyGoogleSignin(googleCred: any) {
    (await this.loginLogout.signInWithGoogle(googleCred)).subscribe({
      next: (data) => {
        if (data.verificationResult.status == "authorized") {
          this.userData = data;
          sessionStorage.setItem("email", this.userData.userInfo.email)
          sessionStorage.setItem("role", this.userData.userInfo.role)

          // Redirect to home page based on user role
          debugger
          if (this.userData.userInfo.role.toLowerCase() === 'admin') {
            this.router.navigate(['/admin-home']);
          } else {
            this.router.navigate(['/general-home']);
          }
        }
      },
      error: (error) => {
        console.error('Error fetching data:', error);
        // Handle error (e.g., show error message to user)
      }
    });
  }

  createNewAccount() {
    // debugger
    this.signInResponse.emit(true);
  }
}
