import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from './login-box.service';

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
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const { username, password } = this.loginForm.value;
      this.authService.directLogin(username, password).subscribe({
        next: (response) => {
          console.log('Login successful', response);
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
