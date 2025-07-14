using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Tendance.API.Entities;

namespace Tendance.API.Data
{
    public class ApplicationDbContext(IConfiguration configuration) : DbContext
    {
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<RoomEntity> Rooms => Set<RoomEntity>();
        public DbSet<SchoolEntity> Schools => Set<SchoolEntity>();
        public DbSet<CourseEntity> Courses => Set<CourseEntity>();
        public DbSet<TeacherEntity> Teachers => Set<TeacherEntity>();
        public DbSet<StudentEntity> Students => Set<StudentEntity>();
        public DbSet<ClassroomEntity> Classrooms => Set<ClassroomEntity>();
        public DbSet<CaptureDeviceEntity> Devices => Set<CaptureDeviceEntity>();
        public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

        public DbSet<CourseTeacherEntity> CourseTeachers => Set<CourseTeacherEntity>();
        public DbSet<ClassroomStudentEntity> ClassroomStudents => Set<ClassroomStudentEntity>();
        public DbSet<ClassroomSessionEntity> ClassroomSessions => Set<ClassroomSessionEntity>();

        public DbSet<AttendanceEntity> Attendances => Set<AttendanceEntity>();
        public DbSet<StudentAttendanceEntity> StudentAttendances => Set<StudentAttendanceEntity>();
        public DbSet<TeacherAttendanceEntity> TeacherAttendances => Set<TeacherAttendanceEntity>();

        public DbSet<FaceEntity> Faces => Set<FaceEntity>();
        public DbSet<StudentFaceEntity> StudentFaces => Set<StudentFaceEntity>();
        public DbSet<TeacherFaceEntity> TeacherFaces => Set<TeacherFaceEntity>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = configuration["TENDANCE_DB_CONNECTION"];
            ArgumentNullException.ThrowIfNullOrEmpty(connectionString);
            optionsBuilder.UseSqlServer(connectionString);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<TeacherEntity>()
                .HasMany(t => t.Courses)
                .WithMany(c => c.Teachers)
                .UsingEntity<CourseTeacherEntity>();

            modelBuilder.Entity<FaceEntity>()
                .HasDiscriminator<string>("Type")
                .HasValue<StudentFaceEntity>("Student")
                .HasValue<TeacherFaceEntity>("Teacher");

            modelBuilder.Entity<StudentFaceEntity>()
                .HasOne(face => face.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<TeacherFaceEntity>()
                .HasOne(face => face.Teacher)
                .WithMany()
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.ClientCascade);

            DateTime date = new DateTime(
                date: new DateOnly(2025, 7, 7),
                time: new TimeOnly(),
                kind: DateTimeKind.Utc);

            SchoolEntity school = new SchoolEntity
            {
                Id = new Guid("227A8BD2-A317-4891-A885-2623819F6642"),
                Email = "tendance@iqacademy.com",
                Name = "IQ Academy",
                Joined = date
            };

            UserEntity user = new UserEntity
            {
                Id = new Guid("1AC38D88-6806-4773-83F8-E59EC675CC25"),
                SchoolId = school.Id,
                Email = "sulmuk28@gmail.com",
                Password = "Admin23!",
                Username = "Unghostdude",
                Created = date
            };

            TeacherEntity teacher = new TeacherEntity
            {
                Id = 1,
                SchoolId = school.Id,
                Email = "shlomodo28@gmail.com",
                Created = date,
                FirstName = "David",
                LastName = "Solomon",
            };

            StudentEntity student = new StudentEntity
            {
                Id = 1,
                SchoolId = school.Id,
                Created = date,
                Email = "eeshan@gmail.com",
                FirstName = "Aisha",
                MiddleName = "Alhassan",
                LastName = "Usman",
            };

            CourseEntity course = new CourseEntity
            {
                Id = 1,
                SchoolId = school.Id,
                Created = date,
                Name = "Control Systems",
            };

            RoomEntity room = new RoomEntity
            {
                Id = 1,
                SchoolId = school.Id,
                Name = "Remote",
            };

            ClassroomEntity classroom = new ClassroomEntity
            {
                Id = 1,
                SchoolId = school.Id,
                CourseId = course.Id,
                RoomId = room.Id,
                Created = date,
                TeacherId = teacher.Id,
            };

            CaptureDeviceEntity device = new CaptureDeviceEntity
            {
                Id = 1,
                SchoolId = school.Id,
                ClassroomId = classroom.Id,
                ClientKey = $"dev-FBB630A0-72CE-43D5-B364-2534BE0137E5",
                Created = date,
                Type = CaptureDeviceType.FacialRecognition,
                Mode = CaptureDeviceMode.Recognition,
                Nickname = "Default",
            };

            modelBuilder.Entity<RoomEntity>().HasData(room);
            modelBuilder.Entity<UserEntity>().HasData(user);
            modelBuilder.Entity<SchoolEntity>().HasData(school);
            modelBuilder.Entity<StudentEntity>().HasData(student);
            modelBuilder.Entity<TeacherEntity>().HasData(teacher);
            modelBuilder.Entity<CourseEntity>().HasData(course);
            modelBuilder.Entity<ClassroomEntity>().HasData(classroom);
            modelBuilder.Entity<CaptureDeviceEntity>().HasData(device);
        }
    }
}
