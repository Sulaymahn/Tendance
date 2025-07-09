import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CourseMinimal } from '../course/course.service';
import { Room, RoomMinimal } from '../room/room.service';
import { StudentMinimal } from '../student/student.service';
import { TeacherMinimal } from '../teacher/teacher.service';

export interface Classroom {
  id: number;
  created: string;
  room: Room;
  course: ClassroomCourse;
  teacher: ClassroomTeacher;
  students: ClassroomStudent[];
}

export interface ClassroomMinimal {
  id: number;
  room: RoomMinimal;
  course: CourseMinimal;
  teacher: TeacherMinimal;
  students: StudentMinimal[];
}

export interface ClassroomForCreation {
  courseId: number;
  teacherId: number;
  roomId: number;
}

export interface ClassroomCourse {
  id: string;
  name: string;
}

export interface ClassroomTeacher {
  id: string;
  firstName: string;
  middleName: string | null;
  lastName: string;
}

export interface ClassroomStudent {
  id: string;
  firstName: string;
  middleName: string | null;
  lastName: string;
}

@Injectable({
  providedIn: 'root'
})
export class ClassroomService {
  private http = inject(HttpClient);

  getAll(): Observable<Classroom[]> {
    return this.http.get<Classroom[]>(`${environment.backendBaseUrl}classrooms`);
  }

  getAllMinimal(): Observable<ClassroomMinimal[]> {
    return this.http.get<ClassroomMinimal[]>(`${environment.backendBaseUrl}classrooms`, {
      headers: {
        'X-Minimal': 'true'
      }
    });
  }

  create(classroom: ClassroomForCreation): Observable<void> {
    return this.http.post<void>(`${environment.backendBaseUrl}classrooms`, classroom);
  }

  delete(classroom: Classroom): Observable<void> {
    return this.http.delete<void>(`${environment.backendBaseUrl}classrooms`, {
      headers: {
        'X-Classroom-Id': classroom.id.toString()
      }
    });
  }
}
