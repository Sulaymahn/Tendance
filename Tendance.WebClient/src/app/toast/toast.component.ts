import { Component, inject, OnInit } from '@angular/core';
import { AppNotification, NotificationLevel, NotificationService } from '../../services/notification/notification.service';
import { finalize, interval, Subscription, take } from 'rxjs';

@Component({
  selector: 'app-toast',
  imports: [],
  templateUrl: './toast.component.html',
  styleUrl: './toast.component.scss'
})
export class ToastComponent implements OnInit {
  notificationService = inject(NotificationService);
  notificationLevels = NotificationLevel;
  notifications: AppNotification[] = [];
  timers: {
    [key: number]: Subscription
  } = {};

  timer: Subscription | undefined;
  show: boolean = false;

  ngOnInit(): void {
    this.notificationService.notifier.subscribe({
      next: (value: AppNotification) => {
        this.notifications.push(value);
        this.start(value);
      },
      error: (err: any) => {
        console.error(err);
      }
    });
  }

  start(notification: AppNotification): void {
    this.timers[notification.order] = this.timer = interval(4000).pipe(
      take(1)
    ).subscribe({
      complete: () => {
        this.stop(notification);
      }
    })
  }

  stop(notification: AppNotification): void {
    this.timers[notification.order].unsubscribe();
    this.notifications = this.notifications.filter(n => n.order != notification.order);
  }
}
