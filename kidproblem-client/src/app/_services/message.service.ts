import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  messages: string[] = [];

  constructor(private snackBar: MatSnackBar) { }

  add(message: string) {
    this.messages.push(message);
  }

  clear() {
    this.messages = [];
  }

  replace(message: string) {
    this.messages = [];
    this.messages.push(message);
  }

  openSnackBar(message: string, duration = 5000) {
    this.snackBar.open(message, 'Dismiss', { duration: duration, });
  }
}
