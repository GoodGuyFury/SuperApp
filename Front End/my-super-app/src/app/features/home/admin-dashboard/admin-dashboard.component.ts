import { GlobalAlertPopupService } from './../../../shared/components/global-alert-popup/global-alert-popup.service';
import { ConfirmationPopupService } from '../../../shared/components/confirmation-popup/confirmation-popup.service';
import { Component, HostListener, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule, NgFor } from '@angular/common';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AdminDashboardService } from './admin-dashboard.service';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { AuthResponse } from '../../../core/auth.service';
import { ClickOutsideDirective } from '../../../shared/directives/click-outside.directive';
import { MaterialModule } from '../../../shared/modules/material.module';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

type User = AuthResponse['userInfo'];

interface UserDialogData {
  user: User;
  isEditMode: boolean;
}

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    NgFor,ClickOutsideDirective,ReactiveFormsModule
  ],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss'] // Fixed typo to use 'styleUrls' for array
})
export class AdminDashboardComponent implements OnInit {
  activeMenuId: number = 0;
  adminMenuGrid1 = [
    { menuName: 'User Management', menuId: 1 },
    { menuName: 'Raise Request', menuId: 2 },
  ];
  isNewUser :boolean =  false;
  searchText: string = '';
  searchResults: User[] = [];
  selectedUsers: User[] = [];
  private searchSubject = new Subject<string>();

  @ViewChild('userPopup') userPopup!: TemplateRef<any>;
  @ViewChild('passwordPopup') passwordPopup!: TemplateRef<any>;
  private dialogRef!: MatDialogRef<UserDialogData>;

  users: User[] = []; // Changed to a more specific User[] type
  isEditMode: boolean = false;
  cols: number = 6;
  passwordForm!: FormGroup;
  selectedUserToModify: User = {
    firstName: '',
    role: '',
    lastName: '',
    userId: null,
    email: '',
  };
  usersTableDataSource = new MatTableDataSource<User>(this.selectedUsers);

  constructor(private fb: FormBuilder, private adminService: AdminDashboardService, private dialog: MatDialog,private confirmationPopupService: ConfirmationPopupService, private globalAlertPopupService : GlobalAlertPopupService) {}

  ngOnInit(): void {
    this.setupSearch();
    this.adjustGrid();
  }

  @HostListener('window:resize')
  onResize() {
    this.adjustGrid();
  }
  getCombinedUserInfo(user: any): string {
    return `${user.firstName} ${user.lastName} (${user.email})`;
  }
  adjustGrid() {
    const screenWidth = window.innerWidth;
    if (screenWidth <= 600) {
      this.cols = 2; // Mobile
    } else if (screenWidth <= 960) {
      this.cols = 3; // Tablet
    } else {
      this.cols = 6; // Desktop
    }
  }
  openUserDialog(user: User) {
    this.selectedUserToModify = { ...user }; // Clone the user data

    this.dialogRef = this.dialog.open(this.userPopup, {
      data: { user: this.selectedUserToModify, isEditMode: this.isEditMode } as UserDialogData,
    });
  }
  openPasswordDialog(user: User){
    this.selectedUserToModify = user;
    this.passwordForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]], // Example: minimum length of 6
    });
    this.dialogRef = this.dialog.open(this.passwordPopup,{data:{user}});
  }

  onTileClick(item: { menuId: number }) {
    this.activeMenuId = item.menuId;
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

  removeUser(user: User) {
    this.selectedUsers = this.selectedUsers.filter(u => u.userId !== user.userId);
    this.usersTableDataSource.data = this.selectedUsers;
  }

  performSearch(searchText: string): void {
    this.adminService.fetchUserList(searchText).subscribe({
      next: (response: User[]) => {
        this.searchResults = response.filter(user =>
          !this.selectedUsers.some(selectedUser => selectedUser.userId === user.userId)
        );
      },
      error: (error) => {
        console.error('Error searching users:', error);
        // Display user-friendly error message (e.g., using a snackbar)
      },
    });
  }

  editUser(user: User) {
    this.isNewUser = false;
    this.isEditMode = true;
    this.openUserDialog(user);
  }

  viewUserDetails(user: User) {
    this.isEditMode = false;
    this.openUserDialog(user);
  }

  closeSearchResults(){
    this.searchText='';
    this.searchResults = [];
    this.searchSubject.next('');
  }
  selectUser(user: User): void {
    this.selectedUsers.push({ ...user }); // Add user to selected list
    this.usersTableDataSource.data = this.selectedUsers;
    this.searchResults = this.searchResults.filter(u => u.userId !== user.userId);
    this.searchText = '';
  }

  fetchUserList(): void {
    this.adminService.fetchUserList('↺').subscribe({
      next: (response) => {
        this.selectedUsers = response;
        this.usersTableDataSource.data = this.selectedUsers;
      },
      error: (error) => {
        console.error('Error fetching user list:', error);
        // Handle error (e.g., show error message to user)
      }
    });
  }
addUser():void{
  this.isNewUser = true;
  let newuser: User = {firstName: "",role: "",lastName: "",userId: null,email: "",};
   this.isEditMode = true;
  this.openUserDialog(newuser);
}
async deleteUser(user: User) {
  // Show confirmation dialog and wait for the user's response
  const proceed = await this.confirmationPopupService.openConfirmationDialog(
    'Confirm Deletion',
    `Are you sure you want to delete user ${user.firstName + " " + user.lastName}?`
  );

  if (proceed) {
    this.adminService.deleteUser(user).subscribe({
      next: (response) => {

        this.globalAlertPopupService.showAlert(response.message, response.status);

        if(response.status == "success"){
          this.selectedUsers = this.selectedUsers.filter(u => u.userId !== user.userId);
          this.usersTableDataSource.data = this.selectedUsers;
        }
      },
      error: (err) => {
        this.globalAlertPopupService.showAlert(err, 'error');
      },
    });
  } else {
    this.globalAlertPopupService.showAlert('User deletion cancelled.', 'error');
  }
}
saveUser(user: User, newUser: boolean) {
  if (newUser) {
    this.adminService.addUser(user).subscribe({
      next: (response) => {
        this.globalAlertPopupService.showAlert(response.message, response.status);
        // Update selectedUsers if userId matches
        if (response.status === "success") {
          this.selectedUsers.push(response.data);
          this.usersTableDataSource.data = this.selectedUsers; // Add new user
        }

        if (this.dialogRef) {
          this.dialogRef.close();
          this.closeUserDialog();
        }
      },
      error: (error) => {
        console.error('Error adding user:', error);
        // Show an error message to the user
      }
    });
  } else {
    this.adminService.updateUser(user).subscribe({
      next: (response) => {

        this.globalAlertPopupService.showAlert(response.message, response.status);

        // Update selectedUsers if userId matches
        if (response.status === "success") {
          const index = this.selectedUsers.findIndex(u => u.userId === user.userId);
          if (index !== -1) {
            // Create a new object for the updated user
            this.selectedUsers[index] = { ...this.selectedUsers[index], ...user }; // Merge old and new user data
            this.usersTableDataSource.data = this.selectedUsers;
          }
        }
        if (this.dialogRef) {
          this.dialogRef.close();
          this.closeUserDialog();
        }
      },
      error: (error) => {
        console.error('Error updating user:', error);
        // Show an error message to the user
      }
    });
  }
}

updatePassword() {
  if (this.passwordForm.valid) {
    const newPassword = this.passwordForm.get('newPassword')?.value;
    const payload = {
      userModel: this.selectedUserToModify, // User object that matches UserModel
      password: newPassword                   // New password
  };
    this.adminService.updatePassword(payload).subscribe({
      next: (response) => {
        // Check the response status and handle accordingly
        if (response.status === 'success') {
          console.log('Password updated successfully:', response.message);
          // Optionally, you can close the dialog or show a success message
        } else if (response.status === 'error') {
          console.error('Failed to update password:', response.message);
          // Optionally, show an error message to the user
        }
      },
      error: (err) => {
        console.error('Error occurred while updating password:', err);
        // Optionally, handle HTTP error response (e.g., show an error message)
      }
    });
  } else {
    console.warn('Form is invalid. Please check your inputs.');
  }
}

  closeUserDialog() {
    this.selectedUserToModify = {
      firstName: '',
      role: '',
      lastName: '',
      userId: null,
      email: '',
    };
  }
}
