import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
@Component({
  standalone: true,
  imports: [CommonModule],
  selector: 'app-admin-home',
  templateUrl: './admin-home.component.html',
  styleUrls: ['./admin-home.component.scss']
})
export class AdminHomeComponent {

  createUser() {
    console.log('Create user method called');
    // Implement user creation logic here
  }

  deleteUser() {
    console.log('Delete user method called');
    // Implement user deletion logic here
  }

  banUser() {
    console.log('Ban user method called');
    // Implement user banning logic here
  }

  updateUser() {
    console.log('Update user method called');
    // Implement user update logic here
  }
}
