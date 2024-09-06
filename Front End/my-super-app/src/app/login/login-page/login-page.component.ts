import { Component, OnInit } from '@angular/core';
import { SignInComponent } from '../sign-in-google/sign-in.component';
import { CreateUserComponent } from '../create-user/create-user.component';
import { NgIf } from '@angular/common';
import { SuperServiceService } from '../../services/super-service.service';
import { firstValueFrom } from 'rxjs';

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
    this.appInitializer();
  }

  async appInitializer(){
    try {
      const data = await firstValueFrom(this.superservice.appInitialize());
      if(data.message.toLowerCase() === "success"){
        this.redirectToLandingPage();
      }
    } catch (error) {
      console.error('Error initializing app:', error);
      // Handle error (e.g., show error message to user)
    }
  }

  onSignInResponse(response: boolean) {
    this.showSignIn = !response;
    this.showCreateUser = response;
  }

  onCreateUserResponse(response: boolean) {
    this.showCreateUser = !response;
    this.showSignIn = response;
  }

  redirectToLandingPage(){

  }
}
