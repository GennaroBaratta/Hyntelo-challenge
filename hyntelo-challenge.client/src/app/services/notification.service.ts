import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'  // This makes the service available throughout the application.
})
export class NotificationService {
  constructor(private snackBar: MatSnackBar) { }

  /**
   * Displays an error message using Angular Material's snackbar.
   * 
   * @param message - The error message to display.
   */
  showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['error-snackbar']
    });
  }

  /**
   * Displays a success message using Angular Material's snackbar.
   * 
   * @param message - The success message to display.
   */
  showSuccess(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: ['success-snackbar']
    });
  }
}
