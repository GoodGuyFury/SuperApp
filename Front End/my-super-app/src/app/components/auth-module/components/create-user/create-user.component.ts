import { Component, EventEmitter, NgZone, OnInit, Output, ViewChild, ElementRef } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { Location } from '@angular/common';

declare var google: any;

@Component({
  selector: 'app-create-user',
  templateUrl: './create-user.component.html',
  styleUrls: ['./create-user.component.scss'],
  standalone: true,
  imports: []
})
export class CreateUserComponent implements OnInit {

  @Output() createUserResponse = new EventEmitter<boolean>();

  @ViewChild('googleBtn', { static: true }) googleBtn!: ElementRef;

  constructor(private ngZone: NgZone, private location: Location) { }

  ngOnInit() {
    this.renderGoogleSignInButton();
  }

  private renderGoogleSignInButton(): void {
    if (typeof google !== 'undefined' && google.accounts && google.accounts.id) {
      google.accounts.id.initialize({
        client_id: environment.googleClientId,
        callback: this.handleCredentialResponse.bind(this)
      });
      google.accounts.id.renderButton(
        this.googleBtn.nativeElement, // Reference from the template
        {
          theme: 'outline',
          size: 'large',
          width: 130
        }
      );
    } else {
      console.error('Google Sign-In API not loaded');
    }
  }

  private handleCredentialResponse(response: any): void {
    // Handle the sign-in response here
    console.log('Encoded JWT ID token: ' + response.credential);
    // You can send this token to your backend for verification
  }

  goBackToSignIn() {
    this.createUserResponse.emit(true);
  }
}
