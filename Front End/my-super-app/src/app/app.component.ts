import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { IntitialAnimationComponent } from "./shared-module/components/intitial-animation/intitial-animation.component";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, IntitialAnimationComponent,CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'my-super-app';
  showRouterOutlet = false; // Control the visibility of the router outlet
  animationComplete$ = new BehaviorSubject<boolean>(false); // Observable to emit animation completion

  constructor(private router: Router) {}

  // Method to be called by IntitialAnimationComponent when animation completes
  onAnimationComplete() {
    this.showRouterOutlet = true;
    this.animationComplete$.next(true);
  }
}




