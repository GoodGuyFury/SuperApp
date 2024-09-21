import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelect } from '@angular/material/select';
import { MatOption } from '@angular/material/core';
import { FormsModule, NgModel } from '@angular/forms';
import { AdminDashboardService } from './admin-dashboard.service';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { MatListModule } from '@angular/material/list';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatGridListModule } from '@angular/material/grid-list';

interface User {
  id: string;
  email: string;
  name: string;
  msg: string;
  role: string;
}

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatCardModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelect,
    MatOption,
    MatListModule,
    FormsModule,
    MatAutocompleteModule,MatGridListModule
  ],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit{

  searchText: string = '';
  searchResults: User[] = [];
  selectedUsers: User[] = [];
  private searchSubject = new Subject<string>();

  users: any[] = [];

  newUser: any = {
    name: '',
    email: '',
    role: '',
    message: '',
    isLocked: false
  };

  constructor(private adminService: AdminDashboardService) { }

  ngOnInit(): void {
    this.setupSearch();
  }
  createUser(){

  }
  setupSearch(): void {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(searchText => {
      if (searchText.length >= 3) {
        this.performSearch(searchText);
      } else {
        this.searchResults = [];
      }
    });
  }
  onSearchChange(searchValue: string): void {
    this.searchSubject.next(searchValue);
  }

  performSearch(searchText: string): void {
    this.adminService.fetchUserList(searchText).subscribe({
      next: (response: User[]) => {
        this.searchResults = response.filter(user =>
          !this.selectedUsers.some(selectedUser => selectedUser.id === user.id)
        );
      },
      error: (error) => {
        console.error('Error searching users:', error);
      }
    });
  }

  selectUser(user: User): void {
    debugger;
    this.selectedUsers.push({...user});
    this.searchResults = this.searchResults.filter(u => u.id !== user.id);
    this.searchText = '';
  }

  saveChanges(): void {
    // Implement logic to save changes to selected users
    console.log('Saving changes:', this.selectedUsers);
  }

  fetchUserList(): void {
    this.adminService.fetchUserList('').subscribe({
      next: (response) => {
        this.users = response;
      },
      error: (error) => {
        console.error('Error fetching user list:', error);
        // Handle error (e.g., show error message to user)
      }
    });
  }

}
