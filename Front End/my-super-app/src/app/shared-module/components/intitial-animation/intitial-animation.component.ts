import { Component, EventEmitter, Output } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';
import { LoginLogoutService } from '../../../auth-module/services/login-logout.service';
import { Router } from '@angular/router';
import { LoaderService } from '../../services/loader.service';
import { firstValueFrom } from 'rxjs';
import { provideAnimations } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-intitial-animation',
  standalone: true,
  templateUrl: './intitial-animation.component.html',
  styleUrls: ['./intitial-animation.component.scss'],
  providers: [provideAnimations()],
  imports: [RouterModule,CommonModule ],
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
export class IntitialAnimationComponent {
  @Output() animationComplete = new EventEmitter<void>();
  animationState = 'visible'; // Initially visible
  minAnimationDuration = 1000; // Minimum animation duration (in milliseconds)
  apiCallComplete = false;
  animationStarted = false;

  constructor(
    private loginLogoutService: LoginLogoutService,
    private router: Router,
    private loaderService: LoaderService
  ) {}

  ngOnInit() {
    if (!this.animationStarted) {
      this.startImmediateAnimation();
      this.animationStarted = true;
    }
    const animationStartTime = Date.now(); // Record when the animation starts
    this.initializeComponent(animationStartTime);
  }

  startImmediateAnimation() {
    this.animationState = 'visible';
  }

  async initializeComponent(animationStartTime: number) {
    this.loaderService.show(); // Show loader as API call starts

    try {
      const data = await firstValueFrom(this.loginLogoutService.appInitialize());

      this.apiCallComplete = true; // Mark API call as complete

      const elapsedTime = Date.now() - animationStartTime; // Calculate time spent
      const remainingTime = this.minAnimationDuration - elapsedTime;

      if (remainingTime > 0) {
        setTimeout(() => this.hideAnimationAndNavigate(data), remainingTime);
      } else {
        this.hideAnimationAndNavigate(data);
      }
    } catch (error) {
      this.apiCallComplete = true;
      console.error('Error initializing app:', error);

      const elapsedTime = Date.now() - animationStartTime;
      const remainingTime = this.minAnimationDuration - elapsedTime;

      if (remainingTime > 0) {
        setTimeout(() => this.hideAnimationAndNavigate({ verificationResult: { status: 'unauthorized' } }), remainingTime);
      } else {
        this.hideAnimationAndNavigate({ verificationResult: { status: 'unauthorized' } });
      }
    } finally {
      this.loaderService.hide();
    }
  }

  hideAnimationAndNavigate(data: any) {
    this.animationState = 'hidden'; // Start hiding the animation

    setTimeout(() => {
      this.animationComplete.emit(); // Notify the AppComponent that animation is done

      // Navigate only after the animation is fully hidden
      if (data.verificationResult.status.toLowerCase() === "authorized") {
        this.router.navigate(['/home']);
      } else {
        this.router.navigate(['/authentication']);
      }
    }, 500); // Wait for 1 second to finish the hiding animation
  }
}
