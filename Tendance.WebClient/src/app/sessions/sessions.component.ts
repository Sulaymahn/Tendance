import { Component, inject } from '@angular/core';
import { ClassroomSession, ClassroomSessionService } from '../../services/classroom-session/classroom-session.service';
import { ClassroomMinimal, ClassroomService } from '../../services/classroom/classroom.service';
import { FormGroup, FormControl, Validators, FormsModule } from '@angular/forms';
import { Room } from '../../services/room/room.service';
import { switchMap } from 'rxjs';
import { DatePipe } from '@angular/common';
import { DialogComponent } from '../dialog/dialog.component';
import { ClickOutsideDirective } from '../../directives/ClickOutside/click-outside.directive';

enum SessionsModalState {
  None,
  Create,
  Edit
}

class SessionModalContext {
  sessionOption: ClassroomSession | null = null;
}

class CreateClassroomSessionForm {
  choice: 'classroom' | null = null;
  classroom: ClassroomMinimal | null = null;
  topic: string | null = null;
  from: string | null = null;
  to: string | null = null;
  checkInFrom: string | null = null;
  checkInTo: string | null = null;
  checkOutFrom: string | null = null;
  checkOutTo: string | null = null;
  note: string | null = null;

  valid: () => boolean = () => {
    const a = (this.from != null) && (this.to != null);
    const b = (this.checkInFrom != null) && (this.checkInTo != null);
    const c = (this.checkOutFrom != null) && (this.checkOutTo != null);
    if (a && b && c) {
      const aa = new Date(this.from!) < new Date(this.to!);
      const bb = new Date(this.checkInFrom!) < new Date(this.checkInTo!);
      const cc = new Date(this.checkOutFrom!) < new Date(this.checkOutTo!);
      const dd = new Date(this.checkInTo!) < new Date(this.checkOutFrom!);
      return (aa && bb && cc && dd);
    } else {
      return false;
    }
  };
  invalid: () => boolean = () => !this.valid();
  reset: () => void = () => {
    this.classroom = null;
    this.topic = null;
    this.from = null;
    this.to = null;
    this.checkInFrom = null;
    this.checkInTo = null;
    this.checkOutFrom = null;
    this.checkOutTo = null;
    this.note = null;
  };
}

@Component({
  selector: 'app-sessions',
  imports: [DatePipe, DialogComponent, ClickOutsideDirective, FormsModule],
  templateUrl: './sessions.component.html',
  styleUrl: './sessions.component.scss'
})
export class SessionsComponent {
  private readonly api = inject(ClassroomSessionService);
  private readonly classroomApi = inject(ClassroomService);

  sessions: ClassroomSession[] = [];
  classrooms: ClassroomMinimal[] = [];

  modalState: SessionsModalState = SessionsModalState.None;
  modalStates = SessionsModalState;
  modalContext: SessionModalContext = new SessionModalContext();
  createClassroomSessionForm = new CreateClassroomSessionForm();

  showClassrooms() {
    this.classroomApi.getAllMinimal().subscribe({
      next: (value: ClassroomMinimal[]) => {
        this.classrooms = value;
        this.createClassroomSessionForm.choice = 'classroom';
      },
      error: (err) => {
        console.error('Failed to load classroom:', err);
      }
    })
  }

  deleteSession(session: ClassroomSession) {
    this.api.delete(session).subscribe({
      next: () => {
        this.fetchSessions();
      },
      error: (err) => {
        console.error(err);
      }
    })
  }

  closeOption() {
    this.modalContext.sessionOption = null;
  }

  optionClick(session: ClassroomSession) {
    this.modalContext.sessionOption = session;
  }

  selectSession(session: ClassroomSession): void {
    console.log(`session selected: ${session.id}`)
  }

  createSession() {
    if (this.createClassroomSessionForm.invalid()) {
      return;
    }

    this.api.create({
      classroomId: this.createClassroomSessionForm.classroom!.id,
      topic: this.createClassroomSessionForm.topic,
      from: this.createClassroomSessionForm.from!,
      to: this.createClassroomSessionForm.to!,
      checkInFrom: this.createClassroomSessionForm.checkInFrom!,
      checkInTo: this.createClassroomSessionForm.checkInTo!,
      checkOutFrom: this.createClassroomSessionForm.checkOutFrom!,
      checkOutTo: this.createClassroomSessionForm.checkOutTo!,
      note: this.createClassroomSessionForm.note
    }).subscribe({
      next: () => {
        this.closeModal();
        this.fetchSessions();
      },
      error: (err) => {
        console.error(err);
      }
    })
  }

  selectClassroom(classroom: ClassroomMinimal) {
    this.createClassroomSessionForm.classroom = classroom;
    this.createClassroomSessionForm.choice = null;
  }

  ngOnInit(): void {
    this.fetchSessions();
  }

  toString(session: ClassroomSession): string {
    return `${session.classroom.course.name} - ${session.classroom.room.name}`;
  }

  toInputDate(dateInput: Date | string | null): string | null {
    if (dateInput == null || dateInput === '') {
      return null;
    }

    let dateObj: Date;
    if (dateInput instanceof Date) {
      dateObj = dateInput;
    } else if (typeof dateInput === 'string') {
      dateObj = new Date(dateInput);
    } else {
      console.warn('toInputDate received unexpected type or value for parsing:', dateInput);
      return null;
    }
    if (isNaN(dateObj.getTime())) {
      console.error('toInputDate: Invalid Date object created from input string/value:', dateInput);
      return null;
    }

    const year = dateObj.getFullYear();
    const month = (dateObj.getMonth() + 1).toString().padStart(2, '0');
    const day = dateObj.getDate().toString().padStart(2, '0');
    const hours = dateObj.getHours().toString().padStart(2, '0');
    const minutes = dateObj.getMinutes().toString().padStart(2, '0');

    return `${year}-${month}-${day}T${hours}:${minutes}`;
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

  closeModal() {
    this.modalState = SessionsModalState.None;
  }
}
