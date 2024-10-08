import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-confirmation-popup',
  standalone: true,
  templateUrl: './confirmation-popup.component.html',
  styleUrls: ['./confirmation-popup.component.scss'],
  imports: [CommonModule, MatCardModule, MatButtonModule], // Include necessary modules here
})
export class ConfirmationPopupComponent {
  constructor(
    public dialogRef: MatDialogRef<ConfirmationPopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { title: string; message: string }
  ) {}

  onProceed(): void {
    this.dialogRef.close(true); // Close dialog and return true for proceed
  }

  onCancel(): void {
    this.dialogRef.close(false); // Close dialog and return false for cancel
  }
}
