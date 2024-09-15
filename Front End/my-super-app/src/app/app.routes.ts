import { Routes } from '@angular/router';
import { SignInComponent } from './auth-module/components/sign-in-google/sign-in.component';
import { LoginPageComponent } from './auth-module/components/login-page/login-page.component';
import { AdminHomeComponent } from './home-module/components/admin-tab/admin-home.component';
import { GeneralHomeComponent } from './home-module/components/home-page/general-home.component';
import { AuthGuard } from './auth-guard/auth-guard.service';
import { IntitialAnimationComponent } from './shared-module/components/intitial-animation/intitial-animation.component';

export const routes: Routes = [
  { path: '', redirectTo: '/welcome', pathMatch: 'full' },
  { path: 'welcome', component: IntitialAnimationComponent },
  { path: 'authentication', component: LoginPageComponent },
  { path: 'home', component: GeneralHomeComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: '/welcome' } // Catch-all route
];
