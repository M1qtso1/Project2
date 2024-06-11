using University.Models;
using Microsoft.EntityFrameworkCore;

namespace University.Data
{
    public class UniversityContext : DbContext
    {
        public UniversityContext()
        {
        }

        public UniversityContext(DbContextOptions<UniversityContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("UniversityDb");
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subject>().Ignore(s => s.IsSelected);
            modelBuilder.Entity<Book>().Ignore(s => s.IsSelected);

            modelBuilder.Entity<Student>().HasData(
                new Student { StudentId = 1, Name = "Wienczysław", LastName = "Nowakowicz", PESEL = "PESEL1", 
                    BirthDate = new DateTime(1987, 05, 22), Gender = "Male", PlaceOfBirth = "Warsaw", PlaceOfResidence = "Warsaw",
                    AddressLine1 = "123 Main St", AddressLine2 = "Apt 4B", PostalCode = "80-001" },

                new Student { StudentId = 2, Name = "Stanislaw", LastName = "Nowakowicz", PESEL = "PESEL2", 
                    BirthDate = new DateTime(2019, 06, 25), Gender = "Male", PlaceOfBirth = "Krakow", PlaceOfResidence = "Gdansk",
                    AddressLine1 = "456 Elm St", AddressLine2 = "Suite 1A", PostalCode = "80-002" },

                new Student{ StudentId = 3, Name = "Eugenia", LastName = "Nowakowicz", PESEL = "PESEL3", 
                    BirthDate = new DateTime(2021, 06, 08), Gender = "Female", PlaceOfBirth = "Gdansk", PlaceOfResidence = "Warsaw",
                    AddressLine1 = "789 Oak St", AddressLine2 = "Unit 5C", PostalCode = "80-003" });

            modelBuilder.Entity<Subject>().HasData(
                new Subject { SubjectId = 1, Name = "Matematyka", Semester = "1", Lecturer = "Michalina Warszawa" },
                new Subject { SubjectId = 2, Name = "Biologia", Semester = "2", Lecturer = "Halina Katowice" },
                new Subject { SubjectId = 3, Name = "Chemia", Semester = "3", Lecturer = "Jan Nowak" });

            modelBuilder.Entity<Book>().HasData(
                new Book { BookId = 1, Author = "Orwell", Description = "Blablabla", Genre = "fantasy", ISBN = "1231132", 
                    Language = "English", PageCount = 123, PublicationDate = new DateTime(12, 12, 12), Publisher = "Dududu",
                 Title = "Tratrata"}
            );
            modelBuilder.Entity<Classroom>().HasData(
                new Classroom
                {
                    ClassroomId = 1, Location = "Building A, Room 101", Capacity = 30, AvailableSeats = 30,
                    Projector = true, Whiteboard = true, Microphone = false, Description = "Standard classroom with projector and whiteboard"
                });
        }
    }
}
