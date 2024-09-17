import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';
import { provideAnimations } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-intitial-animation',
  standalone: true,
  templateUrl: './intitial-animation.component.html',
  styleUrls: ['./intitial-animation.component.scss'],
  providers: [provideAnimations()],
  imports: [RouterModule, CommonModule],
  animations: [
    trigger('welcomeAnimation', [
      state('visible', style({
        opacity: 1,
        transform: 'translateY(0)', // Centered
      })),
      state('hidden', style({
        opacity: 0,
        transform: 'translateY(100%)', // Falls down out of view
      })),
      transition('visible => hidden', [
        animate('1s ease-in-out') // 1 second to fall down
      ]),
      transition('void => visible', [
        style({ opacity: 0 }),
        animate('1s ease-in') // Fades in initially
      ])
    ])
  ]
})
export class IntitialAnimationComponent implements OnInit, OnDestroy {
  @Output() animationComplete = new EventEmitter<void>();
  animationState = 'visible'; // Initially visible
  private timeoutIds: NodeJS.Timeout[] = [];

  ngOnInit() {
    this.startAnimation();
  }

  startAnimation() {
    this.animationState = 'visible';

    const hideTimeoutId = setTimeout(() => {
      this.hideAnimation();
    }, 1000); // Show for 3 seconds
    this.timeoutIds.push(hideTimeoutId);
  }

  hideAnimation() {
    this.animationState = 'hidden';

    const completeTimeoutId = setTimeout(() => {
      this.animationComplete.emit();
    }, 500); // 1 second for the hiding animation
    this.timeoutIds.push(completeTimeoutId);
  }

  ngOnDestroy() {
    // Clear all timeouts to prevent memory leaks
    this.timeoutIds.forEach(id => clearTimeout(id));
  }
}
