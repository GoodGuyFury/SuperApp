import { Component, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { IntitialAnimationComponent } from "./components/shared-module/components/intitial-animation/intitial-animation.component";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, IntitialAnimationComponent, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'my-super-app';
  showAnimation = true;
  showRouterOutlet = false;

  constructor(private router: Router) {}

  ngOnInit() {
    // The animation will start automatically when the component is initialized
  }

  onAnimationComplete() {
    this.showAnimation = false;
    this.showRouterOutlet = true;
    // Navigate to the authentication route after animation
    this.router.navigate(['/authentication']);
  }
}




