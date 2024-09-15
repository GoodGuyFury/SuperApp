import { Component, OnInit } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';

@Component({
  selector: 'app-intitial-animation',
  standalone: true,
  templateUrl: './intitial-animation.component.html',
  styleUrls: ['./intitial-animation.component.scss'],
  animations: [
    trigger('slideUp', [
      state('visible', style({ transform: 'translateY(0)', opacity: 1 })),
      state('hidden', style({ transform: 'translateY(-100%)', opacity: 0 })),
      transition('visible => hidden', [animate('1s ease-out')]), // Adjust duration if needed
    ]),
  ],
})
export class IntitialAnimationComponent implements OnInit {
  isVisible = true;

  ngOnInit() {
    // Hide the animation after 3 seconds with a sliding effect
    setTimeout(() => {
      this.isVisible = false;
    }, 3000); // 3 seconds
  }
}
