using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using University.Data;
using University.Interfaces;
using University.Models;
using University.Services;
using University.ViewModels;

namespace University.Tests
{
    [TestClass]
    public class AddSubjectViewModelTests
    {
        private IDialogService _dialogService;
        private DbContextOptions<UniversityContext> _options;

        [TestInitialize]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<UniversityContext>()
                .UseInMemoryDatabase(databaseName: "UniversityTestDB")
                .Options;
            SeedTestDB();
        }

        private void SeedTestDB()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var students = new List<Student>
                {
                    new Student { Name = "John Doe" },
                    new Student { Name = "Jane Smith" }
                };

                var books = new List<Book>
                {
                    new Book { Title = "Book1" },
                    new Book { Title = "Book2" }
                };

                context.Students.AddRange(students);
                context.Books.AddRange(books);
                context.SaveChanges();
            }
        }

        private List<ValidationResult> ValidateSubject(Subject subject)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(subject);
            Validator.TryValidateObject(subject, context, results, true);
            return results;
        }

        [TestMethod]
        public void Add_ValidSubject()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                var viewModel = new AddSubjectViewModel(context, _dialogService)
                {
                    Name = "Mathematics",
                    Semester = "Fall 2024",
                    Lecturer = "Dr. Smith"
                };

                viewModel.Save.Execute(null);

                bool subjectExists = context.Subjects.Any(s => s.Name == "Mathematics");
                Assert.IsTrue(subjectExists);
            }
        }
        [TestMethod]
        public void Delete_Subject()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                var subject = new Subject { Name = "Science", Semester = "Spring 2024", Lecturer = "Dr. Jane" };
                context.Subjects.Add(subject);
                context.SaveChanges();

                var subjectToDelete = context.Subjects.FirstOrDefault(s => s.Name == "Science");
                Assert.IsNotNull(subjectToDelete);

                context.Subjects.Remove(subjectToDelete);
                context.SaveChanges();

                bool subjectExists = context.Subjects.Any(s => s.Name == "Science");
                Assert.IsFalse(subjectExists);
            }
        }

        [TestMethod]
        public void Edit_Subject()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                var subject = new Subject { Name = "History", Semester = "Winter 2024", Lecturer = "Dr. John" };
                context.Subjects.Add(subject);
                context.SaveChanges();

                var subjectToEdit = context.Subjects.FirstOrDefault(s => s.Name == "History");
                Assert.IsNotNull(subjectToEdit);

                subjectToEdit.Name = "Modern History";
                context.SaveChanges();

                bool subjectEdited = context.Subjects.Any(s => s.Name == "Modern History");
                Assert.IsTrue(subjectEdited);
            }
        }

        [TestMethod]
        public void Edit_SubjectWithoutName()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                var subject = new Subject { Name = "Geography", Semester = "Summer 2024", Lecturer = "Dr. Alex" };
                context.Subjects.Add(subject);
                context.SaveChanges();

                var subjectToEdit = context.Subjects.FirstOrDefault(s => s.Name == "Geography");
                Assert.IsNotNull(subjectToEdit);

                subjectToEdit.Name = "";
                context.SaveChanges();

                bool subjectEdited = context.Subjects.Any(s => s.Name == "");
                Assert.IsTrue(subjectEdited);
            }
        }

        [TestMethod]
        public void Validate_ValidSubject()
        {
            var subject = new Subject
            {
                Name = "Physics",
                Semester = "Fall 2024",
                Lecturer = "Dr. Albert"
            };

            var results = ValidateSubject(subject);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Validate_SubjectWithoutName()
        {
            var subject = new Subject
            {
                Name = "",
                Semester = "Fall 2024",
                Lecturer = "Dr. Albert"
            };

            var results = ValidateSubject(subject);
            Assert.IsFalse(results.Exists(r => r.ErrorMessage == "The Name field is required."));
        }

        [TestMethod]
        public void Validate_SubjectWithoutSemester()
        {
            var subject = new Subject
            {
                Name = "Chemistry",
                Semester = "",
                Lecturer = "Dr. Watson"
            };

            var results = ValidateSubject(subject);
            Assert.IsFalse(results.Exists(r => r.ErrorMessage == "The Semester field is required."));
        }

        [TestMethod]
        public void Validate_SubjectWithoutLecturer()
        {
            var subject = new Subject
            {
                Name = "Biology",
                Semester = "Winter 2024",
                Lecturer = ""
            };

            var results = ValidateSubject(subject);
            Assert.IsFalse(results.Exists(r => r.ErrorMessage == "The Lecturer field is required."));
        }

        [TestMethod]
        public void Add_ValidStudentToSubject()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                var viewModel = new AddSubjectViewModel(context, _dialogService)
                {
                    Name = "Art",
                    Semester = "Fall 2024",
                    Lecturer = "Dr. Artist"
                };

                var student = context.Students.First();
                viewModel.AddStudent(student);

                viewModel.Save.Execute(null);

                var savedSubject = context.Subjects.Include(s => s.Students).FirstOrDefault(s => s.Name == "Art");
                Assert.IsNotNull(savedSubject);
                Assert.IsTrue(savedSubject.Students.Any(s => s.Name == student.Name));
            }
        }

        [TestMethod]
        public void Add_ValidBookToSubject()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                var viewModel = new AddSubjectViewModel(context, _dialogService)
                {
                    Name = "Literature",
                    Semester = "Spring 2024",
                    Lecturer = "Prof. Literature"
                };

                var book = context.Books.First();
                viewModel.AddBooks(book);

                viewModel.Save.Execute(null);

                var savedSubject = context.Subjects.Include(s => s.Books).FirstOrDefault(s => s.Name == "Literature");
                Assert.IsNotNull(savedSubject);
                Assert.IsTrue(savedSubject.Books.Any(b => b.Title == book.Title));
            }
        }
    }
}
