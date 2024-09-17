import { Routes } from '@angular/router';
import { LoginPageComponent } from './components/auth-module/components/login-page/login-page.component';
import { GeneralHomeComponent } from './components/home-module/components/home-page/general-home.component';
import { AuthGuard } from './auth-guard/auth-guard.service';
import { DashboardComponent } from './components/home-module/components/dashboard/dashboard.component';
import { AdminHomeComponent } from './components/home-module/components/admin-tab/admin-home.component';

export const routes: Routes = [
  { path: '', redirectTo: '/authentication', pathMatch: 'full' },
  { path: 'authentication', component: LoginPageComponent },
  { path: 'home', component: GeneralHomeComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: '/authentication' } // Catch-all route
];
