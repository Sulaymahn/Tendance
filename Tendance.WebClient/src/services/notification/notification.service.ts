import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export enum NotificationLevel {
  Information,
  Success,
  Warning,
  Error,
}

export interface AppNotification {
  order: number;
  message: string;
  level: NotificationLevel;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private order = 0;
  notifier = new Subject<AppNotification>();

  notify(message: string, level: NotificationLevel): void {
    this.notifier.next({
      message: message,
      level: level,
      order: this.order++
    });
  }
}
