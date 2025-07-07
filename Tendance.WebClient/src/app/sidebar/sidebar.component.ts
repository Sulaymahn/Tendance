import { Component, inject, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { BackendService } from '../../services/backend/backend.service';
import { User } from '../../services/backend/backend.service.model';

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent implements OnInit {
  private api = inject(BackendService);
  user: User | null = null;

  ngOnInit(): void {
    this.api.getUser().subscribe({
      next: (user) => this.user = user,
      error: () => this.user = null
    });
  }
}
