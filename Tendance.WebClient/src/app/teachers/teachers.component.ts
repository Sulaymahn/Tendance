import { Component, inject, OnInit } from '@angular/core';
import { DialogComponent } from "../dialog/dialog.component";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ClickOutsideDirective } from '../../directives/ClickOutside/click-outside.directive';
import { DatePipe } from '@angular/common';
import { Teacher, TeacherForCreation, TeacherService } from '../../services/teacher/teacher.service';

enum TeachersModalState {
  None,
  CreateTeacher,
  EditTeacher
}

class TeacherModalContext {
  teacherOption: Teacher | null = null;
}

@Component({
  selector: 'app-teachers',
  imports: [DialogComponent, DatePipe, ClickOutsideDirective, ReactiveFormsModule],
  templateUrl: './teachers.component.html',
  styleUrl: './teachers.component.scss'
})
export class TeachersComponent implements OnInit {
  private readonly api = inject(TeacherService);

  teachers: Teacher[] = [];
  modalState: TeachersModalState = TeachersModalState.None;
  modalStates = TeachersModalState;
  modalContext: TeacherModalContext = new TeacherModalContext();

  createTeacherForm = new FormGroup({
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
    this.fetchTeachers();
  }

  optionClick(teacher: Teacher) {
    this.modalContext.teacherOption = teacher;
  }

  deleteTeacher(teacher: Teacher) {
    this.api.delete(teacher).subscribe({
      complete: () => {
        this.fetchTeachers();
        this.closeOption();
      }
    });
  }

  closeOption() {
    console.log('some');
    this.modalContext.teacherOption = null;
  }

  fetchTeachers(): void {
    this.api.get().subscribe({
      next: (teachers: Teacher[]) => {
        this.teachers = teachers;
      },
      error: (err) => {
        console.error('Failed to load teachers:', err);
      }
    });
  }

  selectTeacher(teacher: Teacher) {
    throw new Error('Method not implemented.');
  }

  createTeacher(): void {
    const teacherData = new TeacherForCreation();

    teacherData.email = this.createTeacherForm.value.email || '';
    teacherData.firstName = this.createTeacherForm.value.firstName || '';
    teacherData.middleName = this.createTeacherForm.value.middleName || '';
    teacherData.lastName = this.createTeacherForm.value.lastName || '';

    this.api.create(teacherData).subscribe({
      complete: () => {
        this.closeModal();
        this.fetchTeachers();
      },
      error: (err) => {
        console.error('Failed to create teacher:', err);
      }
    });
  }

  closeModal(): void {
    this.modalState = TeachersModalState.None;
    this.createTeacherForm.reset();
  }
}
