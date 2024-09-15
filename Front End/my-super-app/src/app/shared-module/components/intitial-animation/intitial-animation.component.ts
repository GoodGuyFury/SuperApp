import { Component, Output, EventEmitter } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';
import { LoginLogoutService } from '../../../auth-module/services/login-logout.service';
import { Router } from '@angular/router';
import { LoaderService } from '../../services/loader.service';
import { firstValueFrom } from 'rxjs';
import { provideAnimations } from '@angular/platform-browser/animations';
import { Route, RouterModule } from '@angular/router';

@Component({
  selector: 'app-intitial-animation',
  standalone: true,
  templateUrl: './intitial-animation.component.html',
  styleUrls: ['./intitial-animation.component.scss'],
  providers: [provideAnimations()],
  imports: [RouterModule],
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
  constructor(
    private loginLogoutService: LoginLogoutService,
    private router: Router,
    private loaderService: LoaderService
  ) { }
  @Output() animationComplete = new EventEmitter<void>();
  animationState = 'visible';

  ngOnInit() {
    this.initializeComponent();
    setTimeout(() => {
      this.animationState = 'hidden'; // Start the animation to make the message fall down
    }, 2000); // Show the welcome message for 1 second

    setTimeout(() => {
      this.animationComplete.emit(); // Notify the parent component when the animation is complete
    }, 2500); // Delay to complete the animation before triggering the event

  }

  async initializeComponent() {
    this.loaderService.show();
    try {
      const data = await firstValueFrom(this.loginLogoutService.appInitialize());
      if (data.verificationResult.status.toLowerCase() === "authorized") {
        this.router.navigate(['/home']);
      } else {
        this.router.navigate(['/authentication']);

      }
    } catch (error) {
      this.router.navigate(['/authentication']);
      console.error('Error initializing app:', error);
      // Handle error (e.g., show error message to user)
    } finally {
      this.loaderService.hide();
    }
  }

}
