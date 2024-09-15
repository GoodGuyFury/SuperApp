import { Routes } from '@angular/router';
import { LoginPageComponent } from './auth-module/components/login-page/login-page.component';
import { GeneralHomeComponent } from './home-module/components/home-page/general-home.component';
import { AuthGuard } from './auth-guard/auth-guard.service';
import { AppComponent } from './app.component';

export const routes: Routes = [
  { path: '', redirectTo: '/welcome', pathMatch: 'full' },
  { path: 'welcome', component: AppComponent },
  { path: 'authentication', component: LoginPageComponent },
  { path: 'home', component: GeneralHomeComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: '/welcome' }
];
