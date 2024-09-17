import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AdminService } from '../../services/admin.service';

interface User {
  id: string;
  email: string;
  name: string;
  msg: string;
  role: string;
}

@Component({
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  selector: 'app-admin-home',
  templateUrl: './admin-home.component.html',
  styleUrls: ['./admin-home.component.scss']
})
export class AdminHomeComponent implements OnInit {
  isCreateUserFormVisible = false;
  isDeleteUserFormVisible = false;
  isUpdateUserFormVisible = false;
  createUserForm!: FormGroup;
  deleteUserForm!: FormGroup;
  updateUserForm!: FormGroup;
  isFetchUserListVisible = false;
  fetchUserForm!: FormGroup;
  userList: User[] = [];

  constructor(private fb: FormBuilder, private adminService: AdminService) {}

  ngOnInit() {
    this.initCreateUserForm();
    this.initDeleteUserForm();
    this.initUpdateUserForm();
    this.initFetchUserForm();
  }

  initCreateUserForm() {
    this.createUserForm = this.fb.group({
      id: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      name: ['', Validators.required],
      msg: ['', Validators.required],
      role: ['', Validators.required]
    });
  }

  initDeleteUserForm() {
    this.deleteUserForm = this.fb.group({
      id: ['', Validators.required]
    });
  }

  initUpdateUserForm() {
    this.updateUserForm = this.fb.group({
      id: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      name: ['', Validators.required],
      msg: ['', Validators.required],
      role: ['', Validators.required]
    });
  }

  initFetchUserForm() {
    this.fetchUserForm = this.fb.group({
      searchText: [''],
      fetchAll: [false]
    });

    this.fetchUserForm.get('fetchAll')?.valueChanges.subscribe(fetchAll => {
      const searchTextControl = this.fetchUserForm.get('searchText');
      if (fetchAll) {
        searchTextControl?.disable();
        searchTextControl?.setValue('');
      } else {
        searchTextControl?.enable();
        searchTextControl?.setValue('');
      }
    });
  }

  showCreateUserForm() {
    this.isCreateUserFormVisible = true;
    this.isDeleteUserFormVisible = false;
    this.isUpdateUserFormVisible = false;
  }

  showDeleteUserForm() {
    this.isDeleteUserFormVisible = true;
    this.isCreateUserFormVisible = false;
    this.isUpdateUserFormVisible = false;
  }

  showUpdateUserForm() {
    this.isUpdateUserFormVisible = true;
    this.isCreateUserFormVisible = false;
    this.isDeleteUserFormVisible = false;
  }

  showFetchUserList() {
    this.isFetchUserListVisible = true;
    this.isCreateUserFormVisible = false;
    this.isDeleteUserFormVisible = false;
    this.isUpdateUserFormVisible = false;
  }

  createUser() {
    if (this.createUserForm.valid) {
      this.adminService.createUser(this.createUserForm.value).subscribe({
        next: (response) => {
          console.log('User created successfully', response);
          this.resetForms();
        },
        error: (error) => {
          console.error('Error creating user', error);
          // Handle error (e.g., show an error message)
        }
      });
    } else {
      Object.values(this.createUserForm.controls).forEach(control => {
        control.markAsTouched();
      });
    }
  }

  deleteUser() {
    if (this.deleteUserForm.valid) {
      console.log('Deleting user with ID:', this.deleteUserForm.value.id);
      // Implement user deletion logic here
      this.resetForms();
    }
  }

  updateUser() {
    if (this.updateUserForm.valid) {
      console.log('Updating user:', this.updateUserForm.value);
      // Implement user update logic here
      this.resetForms();
    }
  }

  fetchUsers() {
    const fetchAll = this.fetchUserForm.get('fetchAll')?.value;
    const searchText = fetchAll ? '6699a98ll' : this.fetchUserForm.get('searchText')?.value;

    this.adminService.fetchUsers(searchText).subscribe({
      next: (users) => {
        this.userList = users;
        console.log('Users fetched successfully', users);
      },
      error: (error) => {
        console.error('Error fetching users', error);
        // Handle error (e.g., show an error message)
      }
    });
  }

  capitalizeRole(formName: 'createUserForm' | 'updateUserForm') {
    const roleControl = (this[formName] as FormGroup).get('role');
    if (roleControl) {
      roleControl.setValue(roleControl.value.toUpperCase(), { emitEvent: false });
    }
  }

  resetForms() {
    this.createUserForm.reset();
    this.deleteUserForm.reset();
    this.updateUserForm.reset();
    this.fetchUserForm.reset();
    this.isCreateUserFormVisible = false;
    this.isDeleteUserFormVisible = false;
    this.isUpdateUserFormVisible = false;
    this.isFetchUserListVisible = false;
    this.userList = [];
  }
}
