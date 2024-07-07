import { Routes } from '@angular/router';
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CanActivateGuard implements CanActivate {
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    return true;
  }
}

import { SignInComponent } from './login/sign-in-google/sign-in.component';
import { CreateUserComponent } from './login/create-user/create-user.component';
import { LoginPageComponent } from './login/login-page/login-page.component';
import { AdminHomeComponent } from './admin-module/admin-home/admin-home.component';
import { UserHomeComponent } from './user-module/user-home/user-home.component';
import { AuthGuard } from './auth-guard/auth-guard.service';

export const routes: Routes = [
  { path: '', redirectTo: '/authentication', pathMatch: 'full' },
  {path: "authentication", component: LoginPageComponent},
//   {path: "login", children:[
//   {path: "new-user", component : LoginPageComponent},
//   {path: "sign-in", component: SignInComponent}]
// },
{path:"admin-home", component:AdminHomeComponent, canActivate: [AuthGuard]},
{path:"user-home", component:UserHomeComponent}
];
