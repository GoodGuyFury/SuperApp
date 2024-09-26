import { Component, HostListener, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTableModule } from '@angular/material/table';

import { MatFormFieldModule } from '@angular/material/form-field';

import { MatInputModule } from '@angular/material/input';
import { MatSelect, MatOption } from '@angular/material/select';
import { MatListModule } from '@angular/material/list';

import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatDialogModule, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AdminDashboardService } from './admin-dashboard.service';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { AuthResponse } from '../../../core/auth.service';

type User = AuthResponse['userInfo'];

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
    MatAutocompleteModule,MatGridListModule,MatDialogModule,NgFor
  ],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit{
  activeMenuId : number = 0;
  adminMenuGrid1: {menuName : string , menuId : number}[] =
  [{menuName : 'User Management', menuId : 1},{menuName : 'Raise Request', menuId : 2}]
  searchText: string = '';
  searchResults: User[] = [];
  selectedUsers: User[] = [];
  private searchSubject = new Subject<string>();
  @ViewChild('userPopup') userPopup!: TemplateRef<any>;
   private dialogRef!: MatDialogRef<any>

  users: any[] = [];
  isEditMode : boolean = false;
  cols: number = 6;
  selectedUserEditViewDelete : User ={
    firstName: '',
    role: 'string',
    lastName: 'string',
    userId: 'string',
    email: 'string',
  };;
  constructor(private adminService: AdminDashboardService, private dialog : MatDialog) { }

  ngOnInit(): void {
    this.setupSearch();
    this.adjustGrid();
  }

  @HostListener('window:resize', ['$event'])
  onResize(event : any) {
    this.adjustGrid();
  }

  adjustGrid() {
    const screenWidth = window.innerWidth;
    if (screenWidth <= 600) { // Mobile
      this.cols = 2;
    } else if (screenWidth <= 960) { // Tablet
      this.cols = 3;
    } else { // Desktop
      this.cols = 6;
    }
  }

  onTileClick(item : any){
    this.activeMenuId = item.menuId;
    console.log(this.activeMenuId);

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
  removeUser(user:User, i :number){
    this.selectedUsers = this.selectedUsers.filter(obj => obj.userId !== user.userId);
  }
  performSearch(searchText: string): void {
    this.adminService.fetchUserList(searchText).subscribe({
      next: (response: User[]) => {
        console.log(this.searchResults, response);
// debugger;
        this.searchResults = response.filter(user =>
          !this.selectedUsers.some(selectedUser => selectedUser.userId === user.userId)
        );
        console.log(this.searchResults);

      },
      error: (error) => {
        console.error('Error searching users:', error);
      },
    });
  }

  editUser(user: User, i : number) {
    this.isEditMode = true;
    this.selectedUserEditViewDelete = { ...user }; // Clone the user data to prevent changes
    this.dialogRef = this.dialog.open(this.userPopup, {
      data: { user: this.selectedUserEditViewDelete, isEditMode: this.isEditMode }
    });
  }

  viewUserDetails(user: User , i : number) {
    this.isEditMode = false;
    this.selectedUserEditViewDelete = { ...user }; // Clone the user data to prevent changes
    this.dialogRef = this.dialog.open(this.userPopup, {
      data: { user: this.selectedUserEditViewDelete, isEditMode: this.isEditMode }
    });
  }

  selectUser(user: User): void {
    // debugger;
    this.selectedUsers.push({...user});
    this.searchResults = this.searchResults.filter(u => u.userId !== user.userId);
    this.searchText = '';
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

  saveUser(user: User) {
    console.log('Saving user:', user);

    this.adminService.updateUser(user).subscribe({
      next: (response) => {
        this.selectedUserEditViewDelete = response;
        console.log('User updated successfully:', response);
        if (this.dialogRef) {
          this.dialogRef.close(); // Close the specific dialog instance
        }
      },
      error: (error) => {
        console.error('Error updating user:', error);
      }
    });

  }

}
