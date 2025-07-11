import { Component, inject } from '@angular/core';
import { NotificationLevel, NotificationService } from '../../services/notification/notification.service';

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  notificationService = inject(NotificationService);
  n = 0;

  notify(): void {
    this.notificationService.notify(`This is the ${this.n++} test message`, this.n % 4);
  }
}
