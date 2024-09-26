import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { LoginBoxService } from './login-box.service';

@Component({
  selector: 'app-login-box',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './login-box.component.html',
  styleUrl: './login-box.component.scss'
})
export class LoginBoxComponent implements OnInit {
  loginForm!: FormGroup;

  constructor(
    private formBuilder: FormBuilder,
    private loginBoxService: LoginBoxService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const { email, password } = this.loginForm.value;
      this.loginBoxService.directLogin(email, password).subscribe({
        next: (response) => {
          this.router.navigate(['/home/dashboard']);
          // Handle successful login
        },
        error: (error) => {
          console.error('Login failed', error);
          // Handle login error
        }
      });
    }
  }
}
