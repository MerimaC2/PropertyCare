import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

/** Small wrapper around MatSnackBar for consistent notifications. */
@Injectable({ providedIn: 'root' })
export class ToasterService {
  constructor(private snackBar: MatSnackBar) {}

  success(message: string): void {
    this.snackBar.open(message, 'OK', { duration: 3000, panelClass: 'toast-success' });
  }

  error(message: string): void {
    this.snackBar.open(message, 'Close', { duration: 5000, panelClass: 'toast-error' });
  }
}
