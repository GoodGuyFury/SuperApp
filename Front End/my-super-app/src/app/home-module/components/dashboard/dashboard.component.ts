import { Component, OnInit } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';

interface UserData {
  user: string;
  status: string;
  lastLogin: Date;
}

@Component({
  standalone: true,
  imports: [MatTableModule, MatIconModule,CommonModule],
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  displayedColumns: string[] = ['user', 'status', 'lastLogin', 'actions'];
  dataSource: MatTableDataSource<UserData> = new MatTableDataSource<UserData>();

  ngOnInit() {
    const users: UserData[] = [
      { user: 'John Doe', status: 'Active', lastLogin: new Date('2023-05-01T10:30:00') },
      { user: 'Jane Smith', status: 'Inactive', lastLogin: new Date('2023-04-28T14:45:00') },
      { user: 'Bob Johnson', status: 'Active', lastLogin: new Date('2023-05-02T09:15:00') },
    ];
    this.dataSource = new MatTableDataSource(users);
  }

  editUser(user: UserData) {
    console.log('Edit user:', user);
    // Implement edit logic
  }

  deleteUser(user: UserData) {
    console.log('Delete user:', user);
    // Implement delete logic
  }
}
