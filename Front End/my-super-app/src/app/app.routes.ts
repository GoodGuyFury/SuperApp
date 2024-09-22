import { AdminDashboardComponent } from './features/home/admin-dashboard/admin-dashboard.component';
import { Routes } from '@angular/router';
import { AuthGuard } from './core/auth-guard.service';

export const routes: Routes = [
  { path: '', redirectTo: 'auth', pathMatch: 'full' },
  {
    path: 'auth',
    loadComponent: () => import('./features/authentication/auth-home/auth-home.component').then(m => m.AuthHomeComponent),
    children: [
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      { path: 'login', loadComponent: () => import('./features/authentication/login-box/login-box.component').then(m => m.LoginBoxComponent) },
      { path: 'signup', loadComponent: () => import('./features/authentication/sign-up-box/sign-up-box.component').then(m => m.SignUpBoxComponent) }
    ]
  },
  {
    path: 'home',
    loadComponent: () => import('./features/home/home-page/home-page.component').then(m => m.HomePageComponent),
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/home/user-dashboard/user-dashboard.component').then(m => m.UserDashboardComponent)
      },
      {
        path: 'admin-dashboard',
        loadComponent: () => import('./features/home/admin-dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent),
        canActivate: [AuthGuard]
      }
    ]
  }

];
