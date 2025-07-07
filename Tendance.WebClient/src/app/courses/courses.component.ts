import { Component, inject, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { BackendService } from '../../services/backend/backend.service';
import { Course, CourseForCreation, Teacher, TeacherForCreation } from '../../services/backend/backend.service.model';
import { ClickOutsideDirective } from '../../directives/ClickOutside/click-outside.directive';
import { DialogComponent } from '../dialog/dialog.component';
import { DatePipe } from '@angular/common';

enum CoursesModalState {
  None,
  CreateCourse,
  EditCourse
}

class CourseModalContext {
  courseOption: Course | null = null;
}

@Component({
  selector: 'app-courses',
  imports: [DialogComponent, ClickOutsideDirective, DatePipe, ReactiveFormsModule],
  templateUrl: './courses.component.html',
  styleUrl: './courses.component.scss'
})
export class CoursesComponent implements OnInit {
  private readonly api = inject(BackendService);

  courses: Course[] = [];
  modalState: CoursesModalState = CoursesModalState.None;
  modalStates = CoursesModalState;
  modalContext: CourseModalContext = new CourseModalContext();

  createCourseForm = new FormGroup({
    name: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(50)
    ]),
    description: new FormControl('')
  });

  ngOnInit(): void {
    this.fetchCourses();
  }

  optionDropdownClick(course: Course) {
    this.modalContext.courseOption = course;
  }

  deleteCourse(course: Course) {
    this.api.deleteCourse(course).subscribe({
      complete: () => {
        this.fetchCourses();
        this.closeOption();
      }
    });
  }

  closeOption() {
    this.modalContext.courseOption = null;
  }

  fetchCourses(): void {
    this.api.getCourses().subscribe({
      next: (courses: Course[]) => {
        this.courses = courses;
      },
      error: (err) => {
        console.error('Failed to load teachers:', err);
      }
    });
  }

  selectTeacher(course: Course) {
    throw new Error('Method not implemented.');
  }

  createCourse(): void {
    const courseData: CourseForCreation = {
      name: this.createCourseForm.value.name || '',
      description: null
    }

    if (this.createCourseForm.value.description) {
      courseData.description = this.createCourseForm.value.description;
    }

    this.api.createCourse(courseData).subscribe({
      complete: () => {
        this.closeModal();
        this.fetchCourses();
      },
      error: (err) => {
        console.error('Failed to create teacher:', err);
      }
    });
  }

  closeModal(): void {
    this.modalState = CoursesModalState.None;
    this.createCourseForm.reset();
  }
}
