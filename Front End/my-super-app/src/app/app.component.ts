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
  showAnimation = false;
  showRouterOutlet = false;

  constructor(private router: Router) {}

  ngOnInit() {
    const animationLoaded = sessionStorage.getItem('animationLoaded') === 'true';

    if (animationLoaded) {
      this.showRouterOutlet = true;
    } else {
      this.showAnimation = true;
    }
  }

  onAnimationComplete() {
    this.showAnimation = false;
    this.showRouterOutlet = true;
    sessionStorage.setItem('animationLoaded', 'true');
    this.router.navigate(['/authentication']);
  }
}




