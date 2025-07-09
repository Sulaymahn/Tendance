import { Component, inject } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { ClickOutsideDirective } from '../../directives/ClickOutside/click-outside.directive';
import { DialogComponent } from '../dialog/dialog.component';
import { Classroom, ClassroomForCreation, ClassroomService } from '../../services/classroom/classroom.service';
import { TeacherMinimal, TeacherService } from '../../services/teacher/teacher.service';
import { StudentService } from '../../services/student/student.service';
import { RoomMinimal, RoomService } from '../../services/room/room.service';
import { CourseMinimal, CourseService } from '../../services/course/course.service';


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
  course: CourseMinimal | null = null;
  teacher: TeacherMinimal | null = null;
  room: RoomMinimal | null = null;
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
  private readonly api = inject(ClassroomService);
  private readonly teacherApi = inject(TeacherService);
  private readonly studentApi = inject(StudentService);
  private readonly courseApi = inject(CourseService);
  private readonly roomApi = inject(RoomService);

  classrooms: Classroom[] = [];

  courses: CourseMinimal[] = [];
  teachers: TeacherMinimal[] = [];
  rooms: RoomMinimal[] = [];

  modalState: ClassroomModalState = ClassroomModalState.None;
  modalStates = ClassroomModalState;
  modalContext: ClassroomModalContext = new ClassroomModalContext();

  createClassroomForm: CreateClassroomForm = new CreateClassroomForm();

  ngOnInit(): void {
    this.fetchClassrooms();
  }

  optionClick(classroom: Classroom) {
    this.modalContext.classroomOption = classroom;
  }

  selectRoom(room: RoomMinimal) {
    this.createClassroomForm.room = room;
    this.createClassroomForm.choice = null;
  }

  selectCourse(course: CourseMinimal) {
    this.createClassroomForm.course = course;
    this.createClassroomForm.choice = null;
  }
  selectTeacher(teacher: TeacherMinimal) {
    this.createClassroomForm.teacher = teacher;
    this.createClassroomForm.choice = null;
  }

  showRooms() {
    this.roomApi.getAllMinimal().subscribe({
      next: (rooms: RoomMinimal[]) => {
        this.rooms = rooms;
        this.createClassroomForm.choice = 'room';
      },
      error: (err) => {
        console.error('Failed to load teachers:', err);
      }
    });
  }

  showTeachers() {
    this.teacherApi.getAllMinimal().subscribe({
      next: (teachers: TeacherMinimal[]) => {
        this.teachers = teachers;
        this.createClassroomForm.choice = 'teacher';
      },
      error: (err) => {
        console.error('Failed to load teachers:', err);
      }
    });
  }

  showCourses() {
    this.courseApi.getAll().subscribe({
      next: (courses: CourseMinimal[]) => {
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
    this.api.getAll().subscribe({
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
