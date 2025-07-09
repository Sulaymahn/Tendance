import { Component, inject, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { User, UserService } from '../../services/user/user.service';

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent implements OnInit {
  private api = inject(UserService);
  user: User | null = null;

  ngOnInit(): void {
    this.api.get().subscribe({
      next: (user) => this.user = user,
      error: () => this.user = null
    });
  }
}
