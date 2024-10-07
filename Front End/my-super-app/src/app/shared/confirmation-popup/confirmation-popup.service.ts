import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationPopupComponent } from './confirmation-popup.component';

@Injectable({
  providedIn: 'root'
})
export class ConfirmationPopupService {

  constructor(private dialog: MatDialog) {}

  openConfirmationDialog(title: string, message: string): Promise<boolean> {
    const dialogRef = this.dialog.open(ConfirmationPopupComponent, {
      width: '400px', // Adjust the width as needed
      data: { title, message },
    });

    return dialogRef.afterClosed().toPromise(); // Return a promise that resolves when the dialog closes
  }
}
