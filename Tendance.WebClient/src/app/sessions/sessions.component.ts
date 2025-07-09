import { Component, inject } from '@angular/core';
import { ClassroomSession, ClassroomSessionService } from '../../services/classroom-session/classroom-session.service';
import { ClassroomMinimal, ClassroomService } from '../../services/classroom/classroom.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Room } from '../../services/room/room.service';
import { switchMap } from 'rxjs';

enum SessionsModalState {
  None,
  Create,
  Edit
}

class SessionModalContext {
  sessionOption: ClassroomSession | null = null;
}


@Component({
  selector: 'app-sessions',
  imports: [],
  templateUrl: './sessions.component.html',
  styleUrl: './sessions.component.scss'
})
export class SessionsComponent {
  private readonly api = inject(ClassroomSessionService);

  sessions: ClassroomSession[] = [];

  modalState: SessionsModalState = SessionsModalState.None;
  modalStates = SessionsModalState;
  modalContext: SessionModalContext = new SessionModalContext();

  createClassroomSessionForm = new FormGroup({
    name: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(50)
    ]),
    building: new FormControl<string | null>(null)
  });

  ngOnInit(): void {
    this.fetchSessions();
  }

  fetchSessions(): void {
    this.api.getAll().subscribe({
      next: (sessions: ClassroomSession[]) => {
        this.sessions = sessions;
      },
      error: (err) => {
        console.error('Failed to load teachers:', err);
      }
    });
  }
}
