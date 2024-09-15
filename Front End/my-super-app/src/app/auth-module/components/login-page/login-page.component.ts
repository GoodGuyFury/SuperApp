import { Component, OnInit } from '@angular/core';
import { SignInComponent } from '../sign-in-google/sign-in.component';
import { CreateUserComponent } from '../create-user/create-user.component';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
  standalone: true,
  imports: [SignInComponent, CreateUserComponent, NgIf],
  providers: []
})
export class LoginPageComponent implements OnInit {
  isInitialized = false;
  showSignIn: boolean = true;
  showCreateUser: boolean = false;

  constructor(
  ) { }

  ngOnInit() {

  }

  onSignInResponse(response: boolean) {
    this.showSignIn = !response;
    this.showCreateUser = response;
  }

  onCreateUserResponse(response: boolean) {
    this.showCreateUser = !response;
    this.showSignIn = response;
  }

}
