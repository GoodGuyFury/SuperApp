import { Component, EventEmitter, NgZone, OnInit, Output, Inject, PLATFORM_ID, AfterViewInit } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { environment } from '../../../environments/environment';
import { ReactiveFormsModule,FormsModule  } from '@angular/forms';
import { RouterModule } from '@angular/router';
import {Location} from '@angular/common';
import { LoginLogoutService } from '../services/login-logout.service';

declare var google :any;

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.scss'],
  standalone : true,
  imports: [ReactiveFormsModule,FormsModule,RouterModule]
})
export class SignInComponent implements OnInit {

  @Output() signInResponse = new EventEmitter<boolean>();

  constructor(private loginLogout : LoginLogoutService, private ngZone: NgZone, private location : Location) { }

  userData:any;

  ngOnInit() {
    // this.location.replaceState("/login/sign-in/");
      google.accounts.id.initialize({
        client_id: googleClientId,
        callback:(resp:any)=>{
          this.ngZone.run(() => {
            this.verifyGoogleSignin(resp.credential);
          });
        }
      });
      google.accounts.id.prompt();


    google.accounts.id.renderButton(document.getElementById("google-btn"),{
      type: "standard",
      theme: "filled_blue",
      size: "large",
      shape: "rectangle",
      width: 100,
      logo_alignment: "center",
      height: "auto",
    })
  }

  async verifyGoogleSignin(googleCred: any) {

    (await this.loginLogout.signInWithGoogle(googleCred)).subscribe({
      next: (data) => {
        // Handle the response data here
        if(data.verificationResult.status == "authorized"){
          this.userData = data;
          sessionStorage.setItem("email", this.userData.userInfo.email)
          sessionStorage.setItem("role", this.userData.userInfo.role)
        }
      },
      error: (error) => {
        console.error('Error fetching data:', error);
      },
      complete: () => {
        // Optional: Handle completion (if needed)
      }
    });
  }

  createNewAccount(){
    // debugger
    this.signInResponse.emit(true);
  }
}
