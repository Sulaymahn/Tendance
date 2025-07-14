import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { ClassroomSession, ClassroomSessionService } from '../../services/classroom-session/classroom-session.service';
import { DatePipe } from '@angular/common';
import { ClassroomStudent } from '../../services/classroom/classroom.service';

@Component({
  selector: 'app-classroom-session-detail',
  imports: [DatePipe],
  templateUrl: './classroom-session-detail.component.html',
  styleUrl: './classroom-session-detail.component.scss'
})
export class ClassroomSessionDetailComponent implements OnInit {
  router = inject(Router);
  route = inject(ActivatedRoute);
  sessionApi = inject(ClassroomSessionService);

  sessionId: number | null = null;
  session: ClassroomSession | null = null;

  ngOnInit(): void {
    this.route.paramMap.subscribe({
      next: (params: ParamMap) => {
        const id = params.get('id');
        if (id) {
          this.sessionId = +(id);
          this.fetch(this.sessionId);
        }
      }
    })
  }

  checkOutTimeStamp(student: ClassroomStudent) {
    if (this.session) {
      const att = this.session.attendances.filter(att => att.checkedOut && att.userId == student.id)[0];
      return att?.checkOutTimeStamp;
    }

    return null;
  }

  isCheckedOut(student: ClassroomStudent): boolean {
    if (this.session) {
      return this.session.attendances.filter(att => att.checkedOut && att.userId == student.id).length > 0;
    }

    return false;
  }

  checkInTimeStamp(student: ClassroomStudent): string | null {
    if (this.session) {
      const att = this.session.attendances.filter(att => att.checkedIn && att.userId == student.id)[0];
      return att?.checkInTimeStamp;
    }

    return null;
  }

  isCheckedIn(student: ClassroomStudent) {
    if (this.session) {
      return this.session.attendances.filter(att => att.checkedIn && att.userId == student.id).length > 0;
    }

    return false;
  }

  fetch(id: number) {
    return this.sessionApi.getById(id).subscribe({
      next: (value: ClassroomSession) => {
        this.session = value;
      }
    })
  }

  goBack() {
    this.router.navigate(['sessions']);
  }
}
