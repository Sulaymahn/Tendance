export class User {
    username: string = '';
    email: string = '';
}

//#region Course
export interface Course {
    id: number;
    name: string;
    description: string | null;
    created: Date;
}

export interface CourseMinimal {
    id: number;
    name: string;
}

export interface CourseForCreation {
    name: string;
    description: string | null;
}
//#endregion

//#region Teacher
export interface Teacher {
    id: number;
    firstName: string;
    middleName: string | null;
    lastName: string;
    email: string;
    attendanceRate: number;
    created: string;
}

export interface TeacherMinimal {
    id: number;
    firstName: string;
    middleName: string | null;
    lastName: string;
}

export class TeacherForCreation {
    firstName: string = '';
    middleName: string | null = null;
    lastName: string = '';
    email: string = '';
}
//#endregion

//#region Student
export interface Student {
    id: string;
    firstName: string;
    middleName: string | null;
    lastName: string;
    email: string;
    attendanceRate: number;
    created: Date;
}

export interface StudentMinimal {
    id: string;
    firstName: string;
    middleName: string | null;
    lastName: string;
}

export interface StudentForCreation {
    id: string;
    firstName: string;
    middleName: string | null;
    lastName: string;
    email: string;
}
//#endregion

//#region Room
export interface Room {
    id: number;
    name: string;
    building: string;
}

export interface RoomMinimal {
    id: number;
    name: string;
}

export interface RoomForCreation {
    name: string;
    building: string | null;
}
//#endregion

//#region Classroom
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
//#endregion

//#region Device
export interface CaptureDevice {
    id: string;
    nickname: string;
    clientKey: string;
    type: string;
    created: string;
    classroom: ClassroomMinimal;
}

export interface CaptureDeviceMinimal {
    id: string;
    nickname: string;
    type: string;
    created: string;
    classroom: ClassroomMinimal;
}

export interface CaptureDeviceForCreation {
    nickname: string;
    type: string;
    classroomId: number | null;
}
//#endregion