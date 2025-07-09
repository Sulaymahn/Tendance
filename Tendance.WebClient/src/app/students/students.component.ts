import { DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ClickOutsideDirective } from '../../directives/ClickOutside/click-outside.directive';
import { DialogComponent } from '../dialog/dialog.component';
import { Student, StudentForCreation, StudentService } from '../../services/student/student.service';

enum StudentModalState {
  None,
  CreateStudent,
  EditStudent
}

class StudentModalContext {
  studentOption: Student | null = null;
}

@Component({
  selector: 'app-students',
  imports: [DialogComponent, DatePipe, ClickOutsideDirective, ReactiveFormsModule],
  templateUrl: './students.component.html',
  styleUrl: './students.component.scss'
})
export class StudentsComponent {
  private readonly api = inject(StudentService);

  students: Student[] = [];
  modalState: StudentModalState = StudentModalState.None;
  modalStates = StudentModalState;
  modalContext: StudentModalContext = new StudentModalContext();

  createStudentForm = new FormGroup({
    id: new FormControl('', [
      Validators.required
    ]),
    firstName: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(50)
    ]),
    middleName: new FormControl(null),
    lastName: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(50)
    ]),
    email: new FormControl('', [
      Validators.required,
      Validators.email,
      Validators.maxLength(100)
    ])
  });

  ngOnInit(): void {
    this.fetchStudents();
  }

  optionClick(student: Student) {
    this.modalContext.studentOption = student;
  }

  deleteStudent(student: Student) {
    this.api.delete(student).subscribe({
      complete: () => {
        this.fetchStudents();
        this.closeOption();
      }
    });
  }

  closeOption() {
    this.modalContext.studentOption = null;
  }

  fetchStudents(): void {
    this.api.getAll().subscribe({
      next: (students: Student[]) => {
        this.students = students;
      },
      error: (err) => {
        console.error('Failed to load students:', err);
      }
    });
  }

  selectStudent(student: Student) {
    throw new Error('Method not implemented.');
  }

  createStudent(): void {
    const studentData: StudentForCreation = {
      id: this.createStudentForm.value.id || '',
      email: this.createStudentForm.value.email || '',
      firstName: this.createStudentForm.value.firstName || '',
      middleName: this.createStudentForm.value.middleName || '',
      lastName: this.createStudentForm.value.lastName || '',
    };

    this.api.create(studentData).subscribe({
      complete: () => {
        this.closeModal();
        this.fetchStudents();
      },
      error: (err) => {
        console.error('Failed to create teacher:', err);
      }
    });
  }

  closeModal(): void {
    this.modalState = StudentModalState.None;
    this.createStudentForm.reset();
  }
}
