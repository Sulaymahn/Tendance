import { Component, inject } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { ClickOutsideDirective } from '../../directives/ClickOutside/click-outside.directive';
import { DialogComponent } from '../dialog/dialog.component';
import { Classroom, ClassroomForCreation, ClassroomService } from '../../services/classroom/classroom.service';
import { Teacher, TeacherService } from '../../services/teacher/teacher.service';
import { StudentService } from '../../services/student/student.service';
import { Room, RoomService } from '../../services/room/room.service';
import { Course, CourseService } from '../../services/course/course.service';
import { Router } from '@angular/router';


enum ClassroomModalState {
  None,
  CreateClassroom,
  EditClassroom
}

class ClassroomModalContext {
  classroomOption: Classroom | null = null;
}

class CreateClassroomForm {
  choice: 'course' | 'teacher' | 'room' | null = null;
  course: Course | null = null;
  teacher: Teacher | null = null;
  room: Room | null = null;
  valid: () => boolean = () => (this.course != null && this.teacher != null && this.room != null);
  invalid: () => boolean = () => !this.valid();
  reset: () => void = () => {
    this.course = null;
    this.teacher = null;
    this.room = null;
  }
}

@Component({
  selector: 'app-classrooms',
  imports: [DialogComponent, DatePipe, ClickOutsideDirective, ReactiveFormsModule],
  templateUrl: './classrooms.component.html',
  styleUrl: './classrooms.component.scss'
})
export class ClassroomsComponent {
  private readonly router = inject(Router);
  private readonly api = inject(ClassroomService);
  private readonly teacherApi = inject(TeacherService);
  private readonly studentApi = inject(StudentService);
  private readonly courseApi = inject(CourseService);
  private readonly roomApi = inject(RoomService);

  classrooms: Classroom[] = [];

  courses: Course[] = [];
  teachers: Teacher[] = [];
  rooms: Room[] = [];

  modalState: ClassroomModalState = ClassroomModalState.None;
  modalStates = ClassroomModalState;
  modalContext: ClassroomModalContext = new ClassroomModalContext();

  createClassroomForm: CreateClassroomForm = new CreateClassroomForm();

  ngOnInit(): void {
    this.fetchClassrooms();
  }

  viewDetails(classroom: Classroom) {
    this.router.navigate(['classrooms', classroom.id.toString()]);
  }

  optionClick(classroom: Classroom) {
    this.modalContext.classroomOption = classroom;
  }

  selectRoom(room: Room) {
    this.createClassroomForm.room = room;
    this.createClassroomForm.choice = null;
  }

  selectCourse(course: Course) {
    this.createClassroomForm.course = course;
    this.createClassroomForm.choice = null;
  }
  selectTeacher(teacher: Teacher) {
    this.createClassroomForm.teacher = teacher;
    this.createClassroomForm.choice = null;
  }

  showRooms() {
    this.roomApi.get().subscribe({
      next: (rooms: Room[]) => {
        this.rooms = rooms;
        this.createClassroomForm.choice = 'room';
      },
      error: (err) => {
        console.error('Failed to load teachers:', err);
      }
    });
  }

  showTeachers() {
    this.teacherApi.get().subscribe({
      next: (teachers: Teacher[]) => {
        this.teachers = teachers;
        this.createClassroomForm.choice = 'teacher';
      },
      error: (err) => {
        console.error('Failed to load teachers:', err);
      }
    });
  }

  showCourses() {
    this.courseApi.get().subscribe({
      next: (courses: Course[]) => {
        this.courses = courses;
        this.createClassroomForm.choice = 'course';
      },
      error: (err) => {
        console.error('Failed to load courses:', err);
      }
    });
  }

  deleteClassroom(classroom: Classroom) {
    this.api.delete(classroom).subscribe({
      complete: () => {
        this.fetchClassrooms();
        this.closeOption();
      }
    });
  }

  closeOption() {
    this.modalContext.classroomOption = null;
  }

  fetchClassrooms(): void {
    this.api.get().subscribe({
      next: (classrooms: Classroom[]) => {
        this.classrooms = classrooms;
      },
      error: (err) => {
        console.error('Failed to load classrooms:', err);
      }
    });
  }

  selectClassroom(classroom: Classroom) {
    throw new Error('Method not implemented.');
  }

  createClassroom(): void {
    if (this.createClassroomForm.invalid()) {
      return;
    }

    const classroomData: ClassroomForCreation = {
      roomId: this.createClassroomForm.room!.id,
      teacherId: this.createClassroomForm.teacher!.id,
      courseId: this.createClassroomForm.course!.id,
    };

    this.api.create(classroomData).subscribe({
      complete: () => {
        this.closeModal();
        this.fetchClassrooms();
      },
      error: (err) => {
        console.error('Failed to create teacher:', err);
      }
    });
  }

  closeModal(): void {
    this.modalState = ClassroomModalState.None;
    this.createClassroomForm.reset();
  }
}
