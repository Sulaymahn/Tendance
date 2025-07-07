import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { CaptureDevice, CaptureDeviceForCreation, CaptureDeviceMinimal, Classroom, ClassroomForCreation, ClassroomMinimal, Course, CourseForCreation, CourseMinimal, Room, RoomForCreation, RoomMinimal, Student, StudentForCreation, StudentMinimal, Teacher, TeacherForCreation, TeacherMinimal, User } from './backend.service.model';

@Injectable({
  providedIn: 'root'
})
export class BackendService {
  private readonly http = inject(HttpClient);

  //#region Teacher
  getTeachers(): Observable<Teacher[]> {
    return this.http.get<Teacher[]>(`${environment.backendBaseUrl}teachers`);
  }

  getTeachersMinimal(): Observable<TeacherMinimal[]> {
    return this.http.get<Teacher[]>(`${environment.backendBaseUrl}teachers`, {
      headers: {
        'X-Minimal': "true"
      }
    });
  }

  createTeacher(teacher: TeacherForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}teachers`, teacher);
  }

  deleteTeacher(teacher: Teacher): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}teachers`, {
      headers: {
        'X-Teacher-Id': teacher.id.toString()
      }
    });
  }
  //#endregion

  //#region Course
  getCourses(): Observable<Course[]> {
    return this.http.get<Course[]>(`${environment.backendBaseUrl}courses`);
  }

  getCoursesMinimal(): Observable<CourseMinimal[]> {
    return this.http.get<CourseMinimal[]>(`${environment.backendBaseUrl}courses`, {
      headers: {
        'X-Minimal': "true"
      }
    });
  }

  createCourse(course: CourseForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}courses`, course);
  }

  deleteCourse(course: Course): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}courses`, {
      headers: {
        'X-Course-Id': course.id.toString()
      }
    });
  }
  //#endregion

  //#region Room
  getRooms(): Observable<Room[]> {
    return this.http.get<Room[]>(`${environment.backendBaseUrl}rooms`);
  }

  getRoomsMinimal(): Observable<RoomMinimal[]> {
    return this.http.get<RoomMinimal[]>(`${environment.backendBaseUrl}rooms`, {
      headers: {
        'X-Minimal': "true"
      }
    });
  }

  createRoom(room: RoomForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}rooms`, room);
  }

  deleteRoom(room: Room): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}rooms`, {
      headers: {
        'X-Course-Id': room.id.toString()
      }
    });
  }
  //#endregion

  //#region Student
  getStudents(): Observable<Student[]> {
    return this.http.get<Student[]>(`${environment.backendBaseUrl}students`);
  }

  getStudentsMinimal(): Observable<StudentMinimal[]> {
    return this.http.get<Student[]>(`${environment.backendBaseUrl}students`, {
      headers: {
        'X-Minimal': "true"
      }
    });
  }

  createStudent(student: StudentForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}students`, student);
  }

  deleteStudent(student: Student): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}students`, {
      headers: {
        'X-Student-Id': student.id
      }
    });
  }
  //#endregion

  //#region Classroom
  getClassrooms(): Observable<Classroom[]> {
    return this.http.get<Classroom[]>(`${environment.backendBaseUrl}classrooms`);
  }

  getClassroomsMinimal(): Observable<ClassroomMinimal[]> {
    return this.http.get<ClassroomMinimal[]>(`${environment.backendBaseUrl}classrooms`, {
      headers: {
        'X-Minimal': 'true'
      }
    });
  }

  createClassroom(classroom: ClassroomForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}classrooms`, classroom);
  }

  deleteClassroom(classroom: Classroom): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}classrooms`, {
      headers: {
        'X-Classroom-Id': classroom.id.toString()
      }
    });
  }
  //#endregion

  //#region Capture Device
  getCaptureDevices(): Observable<CaptureDevice[]> {
    return this.http.get<CaptureDevice[]>(`${environment.backendBaseUrl}devices`);
  }

  getCaptureDevicesMinimal(): Observable<CaptureDeviceMinimal[]> {
    return this.http.get<CaptureDeviceMinimal[]>(`${environment.backendBaseUrl}devices`, {
      headers: {
        'X-Minimal': 'true'
      }
    });
  }

  getCaptureDeviceTypes(): Observable<string[]> {
    return this.http.get<string[]>(`${environment.backendBaseUrl}devices/types`);
  }

  createCaptureDevice(captureDevice: CaptureDeviceForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}devices`, captureDevice);
  }

  deleteCaptureDevice(captureDevice: CaptureDevice): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}devices`, {
      headers: {
        'X-Device-Id': captureDevice.id
      }
    });
  }
  //#endregion

  getUser(): Observable<User> {
    return this.http.get<User>(`${environment.backendBaseUrl}user`);
  }
}