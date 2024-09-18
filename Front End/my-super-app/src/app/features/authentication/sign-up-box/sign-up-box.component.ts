import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-sign-up-box',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './sign-up-box.component.html',
  styleUrl: './sign-up-box.component.scss'
})
export class SignUpBoxComponent {
  // Add any necessary sign-up logic here
}
