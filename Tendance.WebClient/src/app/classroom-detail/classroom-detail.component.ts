import { Component, inject, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { ClassroomSessionService, ClassroomSession } from '../../services/classroom-session/classroom-session.service';
import { Classroom, ClassroomService, ClassroomStudent } from '../../services/classroom/classroom.service';
import { Student, StudentService } from '../../services/student/student.service';
import { DialogComponent } from '../dialog/dialog.component';

enum ClassroomModalState {
  None,
  AddStudent,
}

class ClassroomModalContext {
  studentOption: Student | null = null;
}

@Component({
  selector: 'app-classroom-detail',
  imports: [DialogComponent],
  templateUrl: './classroom-detail.component.html',
  styleUrl: './classroom-detail.component.scss'
})
export class ClassroomDetailComponent implements OnInit {
  router = inject(Router);
  route = inject(ActivatedRoute);
  classroomApi = inject(ClassroomService);
  studentApi = inject(StudentService);

  classroomId: number | null = null;
  classroom: Classroom | null = null;
  students: Student[] = [];
  selectedStudents: Student[] = [];

  modalState: ClassroomModalState = ClassroomModalState.None;
  modalStates = ClassroomModalState;
  modalContext: ClassroomModalContext = new ClassroomModalContext();

  ngOnInit(): void {
    this.route.paramMap.subscribe({
      next: (params: ParamMap) => {
        const id = params.get('id');
        if (id) {
          this.classroomId = +(id);
          this.fetch(this.classroomId);
        }
      }
    })
  }

  selectedStudent(student: Student) {
    if (this.selectedStudents.some(s => s.id == student.id)) {
      this.selectedStudents = this.selectedStudents.filter(s => s.id != student.id);
    } else {
      this.selectedStudents.push(student);
    }
  }

  manageEnrollment() {
    this.studentApi.get().subscribe({
      next: (value: Student[]) => {
        this.students = value;
        if (this.classroom) {
          this.selectedStudents = this.students.filter(s => this.classroom?.students.map(s => s.id).some(r => r == s.id));
          this.modalState = ClassroomModalState.AddStudent;
        }
      }
    })
  }

  updateStudents() {
    if (this.classroom) {
      this.classroomApi.updateStudents(this.classroom, this.selectedStudents.map(s => s.id)).subscribe({
        next: () => {
          this.modalState = ClassroomModalState.None;
          if (this.classroomId) {
            this.fetch(this.classroomId);
          }
        }
      })
    }
  }

  isSelected(student: Student) {
    return this.selectedStudents.some(s => s.id == student.id);
  }

  closeModal() {
    this.modalState = ClassroomModalState.None;
  }

  fetch(id: number) {
    return this.classroomApi.getById(id).subscribe({
      next: (value: Classroom) => {
        this.classroom = value;
      }
    })
  }

  fetchStudents() {
    return this.studentApi.get().subscribe({
      next: (value: Student[]) => {
        this.students = value;
      }
    })
  }

  viewDetails(classroom: Classroom) {
    throw new Error('Method not implemented.');
  }

  deleteClassroom(classroom: Classroom) {
    throw new Error('Method not implemented.');
  }

  closeOption() {
    throw new Error('Method not implemented.');
  }

  optionClick(student: ClassroomStudent) {
    throw new Error('Method not implemented.');
  }

  goBack() {
    this.router.navigate(['sessions']);
  }
}
