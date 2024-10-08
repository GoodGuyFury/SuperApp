import { NgIf } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogRef } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-global-alert-popup',
  standalone: true,
  imports: [NgIf, MatCardModule, MatButtonModule, MatIcon],
  templateUrl: './global-alert-popup.component.html',
  styleUrl: './global-alert-popup.component.scss'
})
export class GlobalAlertComponent implements OnInit{
    message: string | null = null;
    type: 'success' | 'error' = 'error';
    defaultMessage: string = 'An error occurred trying to get the response';

  constructor(public globalAlretRef: MatDialogRef<GlobalAlertComponent>) {}
    ngOnInit(): void {}

    show(message: string | null, type: 'success' | 'error') {
      this.message = message ? message : this.defaultMessage;
      this.type = type;
    }

    close() {
      this.globalAlretRef.close();
    }
}
