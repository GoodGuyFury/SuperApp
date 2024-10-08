import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { GlobalAlertComponent } from './global-alert-popup.component';

@Injectable({
  providedIn: 'root'
})
export class GlobalAlertPopupService {

  constructor(private dialog: MatDialog) { }

  showAlert(message: string, type: 'error' | 'success' = 'error'): void {
    const globalAlertRef = this.dialog.open(GlobalAlertComponent, {
      width: '400px',
      data: { message, type },
    });

    // Set the message and type for the popup (success/error)
    globalAlertRef.componentInstance.show(message || 'An error occurred', type);
  }

}
