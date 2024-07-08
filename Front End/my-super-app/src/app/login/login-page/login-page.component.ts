import { Component, OnInit } from '@angular/core';
import { SignInComponent } from '../sign-in-google/sign-in.component';
import { CreateUserComponent } from '../create-user/create-user.component';
import { NgIf } from '@angular/common';
import { SuperServiceService } from '../../services/super-service.service';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
  standalone : true,
  imports : [SignInComponent, CreateUserComponent, NgIf]
})
export class LoginPageComponent implements OnInit {

  showSignIn: boolean = true;
  showCreateUser: boolean = false;

  constructor(private superservice:SuperServiceService) { }

  ngOnInit() {
    debugger;
    this.appInitializer();
  }

  async appInitializer(){
    debugger
    (await this.superservice.appInitialize()).subscribe({
      next: (data) => {
        console.log(data);
      }})
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
