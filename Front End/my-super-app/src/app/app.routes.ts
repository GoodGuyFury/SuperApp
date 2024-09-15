import { Routes } from '@angular/router';
import { SignInComponent } from './auth-module/components/sign-in-google/sign-in.component';
import { LoginPageComponent } from './auth-module/components/login-page/login-page.component';
import { AdminHomeComponent } from './home-module/components/admin-tab/admin-home.component';
import { GeneralHomeComponent } from './home-module/components/home-page/general-home.component';
import { AuthGuard } from './auth-guard/auth-guard.service';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginPageComponent },
  { path: 'authentication', component: LoginPageComponent },
  { path: 'home', component: GeneralHomeComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: '/login' } // Catch-all route
];
