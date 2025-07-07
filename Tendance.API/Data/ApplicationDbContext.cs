using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Tendance.API.Entities;

namespace Tendance.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<School> Schools => Set<School>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Attendance> TeacherAttendances => Set<Attendance>();
        public DbSet<Classroom> Classrooms => Set<Classroom>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<User> Users => Set<User>();
        public DbSet<CaptureDevice> Devices => Set<CaptureDevice>();
        public DbSet<StudentFace> StudentFaces => Set<StudentFace>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<CourseTeacher> CourseTeachers => Set<CourseTeacher>();
        public DbSet<ClassroomStudent> ClassroomStudents => Set<ClassroomStudent>();

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
                .HasOne(ta => ta.Teacher)
                .WithMany(t => t.Attendances)
                .HasForeignKey(t => t.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
                .HasOne(ta => ta.Student)
                .WithMany(t => t.Attendances)
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.Courses)
                .WithMany(c => c.Teachers)
                .UsingEntity<CourseTeacher>();
        }
    }
}
