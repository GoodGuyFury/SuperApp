import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { LoginLogoutService } from '../../services/login-logout.service';
import { LoaderService } from '../../../shared-module/services/loader.service';
import { SignInComponent } from '../sign-in-google/sign-in.component';
import { CreateUserComponent } from '../create-user/create-user.component';
import { NgIf } from '@angular/common';
// import { LoaderComponent } from "../../../shared-module/components/loader/loader.component";
import { provideAnimations } from '@angular/platform-browser/animations';
import { IntitialAnimationComponent } from '../../../shared-module/components/intitial-animation/intitial-animation.component';
import { AuthService } from '../../../../auth.service';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
  standalone: true,
  imports: [SignInComponent, CreateUserComponent, NgIf, IntitialAnimationComponent],
  providers: [provideAnimations()]
})
export class LoginPageComponent implements OnInit {
  isInitialized = false;
  showSignIn: boolean = true;
  showCreateUser: boolean = false;

  constructor(
    private loginLogoutService: LoginLogoutService,
    private router: Router,
    private loaderService: LoaderService,
    private authService: AuthService
  ) { }

  async ngOnInit() {
    if (this.authService.getJustLoggedOut()) {
      this.isInitialized = true;
    }
    else{
      try {
        await firstValueFrom(this.loginLogoutService.appInitialize());
        this.checkAuthAndRedirect();
      } catch (error) {
        console.error('Error during app initialization:', error);
        this.isInitialized = true; // Show login page on error
      }
    }
  }

  private checkAuthAndRedirect() {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/home']);
    } else {
      this.isInitialized = true; // Show login page if not authenticated
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
}
