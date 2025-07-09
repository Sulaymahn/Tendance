import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-topbar',
  imports: [],
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.scss'
})
export class TopbarComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly activatedRoute = inject(ActivatedRoute);

  currentFirstPathSegment: string = '';
  pageTitle: string = 'Default Title';

  private destroy$ = new Subject<void>();
  title: string = '';

  ngOnInit(): void {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      takeUntil(this.destroy$)
    ).subscribe((event: NavigationEnd) => {
      const fullPath = event.urlAfterRedirects;

      const segments = fullPath.split('/').filter(segment => segment !== ''); // Split by '/', filter out empty strings (from leading/trailing slashes)
      this.currentFirstPathSegment = segments.length > 0 ? segments[0] : '';

      this.updatePropertiesBasedOnFirstSegment(this.currentFirstPathSegment);

      console.log('Current First Path Segment:', this.currentFirstPathSegment);
      console.log('Full Path:', fullPath);
    });

    const initialFullPath = this.router.url;
    const initialSegments = initialFullPath.split('/').filter(segment => segment !== '');
    this.currentFirstPathSegment = initialSegments.length > 0 ? initialSegments[0] : '';
    this.updatePropertiesBasedOnFirstSegment(this.currentFirstPathSegment);
  }

  private updatePropertiesBasedOnFirstSegment(segment: string): void {
    switch (segment) {
      case 'dashboard':
        this.pageTitle = 'Dashboard Overview';
        break;
      case 'teachers':
        this.pageTitle = 'Teacher Management';
        break;
      case 'courses':
        this.pageTitle = 'Course Catalog';
        break;
      case 'students':
        this.pageTitle = 'Student Roster';
        break;
      case 'classrooms':
        this.pageTitle = 'Classroom Overview';
        break;
      case 'webhooks':
        this.pageTitle = 'Webhooks';
        break;
      case 'rooms':
        this.pageTitle = 'School Rooms';
        break;
      case 'devices':
        this.pageTitle = 'Capture Devices';
        break;
      default:
        this.pageTitle = 'Unknown Section';
        break;
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
