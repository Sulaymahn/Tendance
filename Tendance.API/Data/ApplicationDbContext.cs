using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Tendance.API.Entities;

namespace Tendance.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<School> Schools => Set<School>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Classroom> Classrooms => Set<Classroom>();
        public DbSet<CaptureDevice> Devices => Set<CaptureDevice>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<CourseTeacher> CourseTeachers => Set<CourseTeacher>();
        public DbSet<ClassroomStudent> ClassroomStudents => Set<ClassroomStudent>();
        public DbSet<ClassroomSession> ClassroomSessions => Set<ClassroomSession>();

        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<StudentAttendance> StudentAttendances => Set<StudentAttendance>();
        public DbSet<TeacherAttendance> TeacherAttendances => Set<TeacherAttendance>();

        public DbSet<Face> Faces => Set<Face>();
        public DbSet<StudentFace> StudentFaces => Set<StudentFace>();
        public DbSet<TeacherFace> TeacherFaces => Set<TeacherFace>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = Environment.GetEnvironmentVariable("TENDANCE_DB_CONNECTION");
            ArgumentNullException.ThrowIfNullOrEmpty(connectionString);
            optionsBuilder.UseSqlServer(connectionString);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Attendance>()
                .HasDiscriminator<string>("Role")
                .HasValue<StudentAttendance>("Student")
                .HasValue<TeacherAttendance>("Teacher");

            modelBuilder.Entity<StudentAttendance>()
                .HasOne(ta => ta.Student)
                .WithMany()
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeacherAttendance>()
                .HasOne(ta => ta.Teacher)
                .WithMany()
                .HasForeignKey(t => t.TeacherId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.Courses)
                .WithMany(c => c.Teachers)
                .UsingEntity<CourseTeacher>();

            modelBuilder.Entity<Face>()
                .HasDiscriminator<string>("Type")
                .HasValue<StudentFace>("Student")
                .HasValue<TeacherFace>("Teacher");

            modelBuilder.Entity<StudentFace>()
                .HasOne(face => face.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<TeacherFace>()
                .HasOne(face => face.Teacher)
                .WithMany()
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.ClientCascade);

            DateTime date = new DateTime(
                date: new DateOnly(2025, 7, 7),
                time: new TimeOnly(),
                kind: DateTimeKind.Utc);

            School school = new School
            {
                Id = new Guid("227A8BD2-A317-4891-A885-2623819F6642"),
                Email = "tendance@iqacademy.com",
                Name = "IQ Academy",
                Joined = date
            };

            User user = new User
            {
                Id = new Guid("1AC38D88-6806-4773-83F8-E59EC675CC25"),
                SchoolId = school.Id,
                Email = "sulmuk28@gmail.com",
                Password = "Admin23!",
                Username = "Unghostdude",
                Created = date
            };

            Teacher teacher = new Teacher
            {
                Id = 1,
                SchoolId = school.Id,
                Email = "shlomodo28@gmail.com",
                Created = date,
                FirstName = "David",
                LastName = "Solomon",
            };

            Student student = new Student
            {
                Id = 1,
                SchoolId = school.Id,
                SchoolAssignedId = "DUT-15001",
                Created = date,
                Email = "eeshan@gmail.com",
                FirstName = "Aisha",
                MiddleName = "Alhassan",
                LastName = "Usman",
            };

            Course course = new Course
            {
                Id = 1,
                SchoolId = school.Id,
                Created = date,
                Name = "Control Systems",
            };

            Room room = new Room
            {
                Id = 1,
                SchoolId = school.Id,
                Name = "Remote",
            };

            Classroom classroom = new Classroom
            {
                Id = 1,
                SchoolId = school.Id,
                CourseId = course.Id,
                RoomId = room.Id,
                Created = date,
                TeacherId = teacher.Id,
            };

            CaptureDevice device = new CaptureDevice
            {
                Id = new Guid("41054BC3-E078-4BC6-A5FE-DAF6985C51D4"),
                SchoolId = school.Id,
                ClassroomId = classroom.Id,
                ClientKey = $"dev-FBB630A0-72CE-43D5-B364-2534BE0137E5",
                Created = date,
                Type = CaptureDeviceType.FacialRecognition,
                Mode = CaptureDeviceMode.Recognition,
                Nickname = "Default",
            };

            modelBuilder.Entity<Room>().HasData(room);
            modelBuilder.Entity<User>().HasData(user);
            modelBuilder.Entity<School>().HasData(school);
            modelBuilder.Entity<Student>().HasData(student);
            modelBuilder.Entity<Teacher>().HasData(teacher);
            modelBuilder.Entity<Course>().HasData(course);
            modelBuilder.Entity<Classroom>().HasData(classroom);
            modelBuilder.Entity<CaptureDevice>().HasData(device);
        }
    }
}
