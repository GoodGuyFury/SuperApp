import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: '/auth/login', pathMatch: 'full' },
  { path: 'home', loadComponent: () => import('./features/home/home-page/home-page.component').then(m => m.HomePageComponent) },
  {
    path: 'auth',
    loadComponent: () => import('./features/authentication/auth-home/auth-home.component').then(m => m.AuthHomeComponent),
    children: [
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      { path: 'login', loadComponent: () => import('./features/authentication/login-box/login-box.component').then(m => m.LoginBoxComponent) },
      { path: 'signup', loadComponent: () => import('./features/authentication/sign-up-box/sign-up-box.component').then(m => m.SignUpBoxComponent) }
    ]
  }
];
