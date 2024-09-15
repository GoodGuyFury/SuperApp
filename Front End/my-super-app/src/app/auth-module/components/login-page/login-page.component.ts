import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { LoginLogoutService } from '../../services/login-logout.service';
import { LoaderService } from '../../../shared-module/services/loader.service';
import { SignInComponent } from '../sign-in-google/sign-in.component';
import { CreateUserComponent } from '../create-user/create-user.component';
import { NgIf } from '@angular/common';
import { LoaderComponent } from "../../../shared-module/components/loader/loader.component";

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
  standalone: true,
  imports: [SignInComponent, CreateUserComponent, NgIf, LoaderComponent],
  providers: []
})
export class LoginPageComponent implements OnInit {
  showSignIn: boolean = true;
  showCreateUser: boolean = false;

  constructor(
    private loginLogoutService: LoginLogoutService,
    private router: Router,
    private loaderService: LoaderService
  ) { }

  async ngOnInit() {
    await this.appInitializer();
  }

  private async appInitializer() {
    this.loaderService.show();
    try {
      const data = await firstValueFrom(this.loginLogoutService.appInitialize());
      if (data.verificationResult.status.toLowerCase() === "authorized") {
        this.router.navigate(['/home']);
      } else {
      }
    } catch (error) {
      console.error('Error initializing app:', error);
      // Handle error (e.g., show error message to user)
    } finally {
      this.loaderService.hide();
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
