import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTableModule } from '@angular/material/table';
import { MatFormField } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelect } from '@angular/material/select';
import { MatOption } from '@angular/material/core';
import { FormsModule, NgModel } from '@angular/forms';
@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [MatTableModule, MatCardModule, MatToolbarModule, MatIconModule, MatButtonModule,MatFormField,MatInputModule,MatSelect, MatOption, FormsModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent {
  users: any[] = [
    { name: 'John Doe', email: 'john@example.com', role: 'admin', message: 'Welcome', isLocked: false },
    { name: 'Jane Smith', email: 'jane@example.com', role: 'user', message: 'Hello', isLocked: true },
    { name: 'Bob Johnson', email: 'bob@example.com', role: 'user', message: 'Hi there', isLocked: false },
  ];

  newUser: any = {
    name: '',
    email: '',
    role: '',
    message: '',
    isLocked: false
  };

  editUser(user: any): void {
    // Implement edit logic
    console.log('Editing user:', user);
  }

  deleteUser(user: any): void {
    this.users = this.users.filter(u => u !== user);
  }

  toggleAccess(user: any): void {
    user.isLocked = !user.isLocked;
  }

  createUser(): void {
    this.users.push({...this.newUser});
    this.newUser = {name: '', email: '', role: '', message: '', isLocked: false};
  }

  fetchUserList(): void {
    // Implement API call to fetch users
    console.log('Fetching user list');
  }
}
