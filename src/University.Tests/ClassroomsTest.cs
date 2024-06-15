using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using University.Data;
using University.Interfaces;
using University.Models;
using University.Services;
using University.ViewModels;

namespace University.Tests;

[TestClass]
public class ClassroomsTest
{
    private IDialogService _dialogService;
    private DbContextOptions<UniversityContext> _options;

    [TestInitialize()]
    public void Initialize()
    {
        _options = new DbContextOptionsBuilder<UniversityContext>()
            .UseInMemoryDatabase(databaseName: "UniversityTestDB")
            .Options;
        SeedTestDB();
        _dialogService = new DialogService();
    }

    private void SeedTestDB()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }

    [TestMethod]
    public void Add_classroom_with_valid_data()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            AddClassroomViewModel addClassroomViewModel = new AddClassroomViewModel(context, _dialogService)
            {
                Location = "Building A, Room 101",
                Capacity = 30,
                AvailableSeats = 30,
                Projector = true,
                Whiteboard = true,
                Microphone = true,
                Description = "A spacious classroom with modern amenities."
            };
            addClassroomViewModel.Save.Execute(null);

            bool newClassroomExists = context.Classrooms.Any(c => c.Location == "Building A, Room 101" && c.Capacity == 30);
            Assert.IsTrue(newClassroomExists);
        }
    }

    [TestMethod]
    public void Add_classroom_without_location()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            AddClassroomViewModel addClassroomViewModel = new AddClassroomViewModel(context, _dialogService)
            {
                Capacity = 30,
                AvailableSeats = 30,
                Projector = true,
                Whiteboard = true,
                Microphone = true,
                Description = "A spacious classroom with modern amenities."
            };
            addClassroomViewModel.Save.Execute(null);

            bool newClassroomExists = context.Classrooms.Any(c => c.Capacity == 30);
            Assert.IsTrue(newClassroomExists);
        }
    }

    [TestMethod]
    public void Add_classroom_without_capacity()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            AddClassroomViewModel addClassroomViewModel = new AddClassroomViewModel(context, _dialogService)
            {
                Location = "Building A, Room 102",
                AvailableSeats = 30,
                Projector = true,
                Whiteboard = true,
                Microphone = true,
                Description = "A spacious classroom with modern amenities."
            };
            addClassroomViewModel.Save.Execute(null);

            bool newClassroomExists = context.Classrooms.Any(c => c.Location == "Building A, Room 102");
            Assert.IsFalse(newClassroomExists);
        }
    }

    [TestMethod]
    public void Add_classroom_without_available_seats()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            AddClassroomViewModel addClassroomViewModel = new AddClassroomViewModel(context, _dialogService)
            {
                Location = "Building B, Room 201",
                Capacity = 50,
                Projector = true,
                Whiteboard = true,
                Microphone = true,
                Description = "A modern classroom."
            };
            addClassroomViewModel.Save.Execute(null);

            bool newClassroomExists = context.Classrooms.Any(c => c.Location == "Building B, Room 201" && c.Capacity == 50);
            Assert.IsTrue(newClassroomExists);
        }
    }

    [TestMethod]
    public void Add_classroom_without_description()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            AddClassroomViewModel addClassroomViewModel = new AddClassroomViewModel(context, _dialogService)
            {
                Location = "Building C, Room 301",
                Capacity = 40,
                AvailableSeats = 40,
                Projector = true,
                Whiteboard = true,
                Microphone = true,
            };
            addClassroomViewModel.Save.Execute(null);

            bool newClassroomExists = context.Classrooms.Any(c => c.Location == "Building C, Room 301" && c.Capacity == 40);
            Assert.IsFalse(newClassroomExists);
        }
    }
    [TestMethod]
    public void Delete_classroom_with_valid_id()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            var classroom = context.Classrooms.FirstOrDefault(c => c.ClassroomId == 1);
            if (classroom != null)
            {
                context.Classrooms.Remove(classroom);
                context.SaveChanges();
            }

            bool classroomExists = context.Classrooms.Any(c => c.ClassroomId == 1);
            Assert.IsFalse(classroomExists);
        }
    }

    [TestMethod]
    public void Delete_classroom_with_invalid_id()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            var classroom = context.Classrooms.FirstOrDefault(c => c.ClassroomId == 999); // Invalid ID
            if (classroom != null)
            {
                context.Classrooms.Remove(classroom);
                context.SaveChanges();
            }

            bool classroomExists = context.Classrooms.Any(c => c.ClassroomId == 1 || c.ClassroomId == 2);
            Assert.IsTrue(classroomExists);
        }
    }

    [TestMethod]
    public void Delete_classroom_with_no_id()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            // No classroom to delete as no ID is provided
            bool classroomExists = context.Classrooms.Any(c => c.ClassroomId == 1 || c.ClassroomId == 2);
            Assert.IsTrue(classroomExists);
        }
    }
    [TestMethod]
    public void Edit_classroom_with_valid_data()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            var classroom = context.Classrooms.FirstOrDefault(c => c.ClassroomId == 1);
            Assert.IsNotNull(classroom);

            EditClassroomViewModel editClassroomViewModel = new EditClassroomViewModel(context, _dialogService)
            {
                ClassroomId = classroom.ClassroomId,
                Location = "Building A, Room 102",
                Capacity = 35,
                AvailableSeats = 35,
                Projector = false,
                Whiteboard = false,
                Microphone = false,
                Description = "An updated classroom description."
            };
            editClassroomViewModel.Save.Execute(null);

            var updatedClassroom = context.Classrooms.FirstOrDefault(c => c.ClassroomId == 1);
            Assert.IsNotNull(updatedClassroom);
            Assert.AreEqual("Building A, Room 102", updatedClassroom.Location);
            Assert.AreEqual(35, updatedClassroom.Capacity);
            Assert.AreEqual(35, updatedClassroom.AvailableSeats);
            Assert.IsFalse(updatedClassroom.Projector);
            Assert.IsFalse(updatedClassroom.Whiteboard);
            Assert.IsFalse(updatedClassroom.Microphone);
            Assert.AreEqual("An updated classroom description.", updatedClassroom.Description);
        }
    }

    [TestMethod]
    public void Edit_classroom_with_invalid_data()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            var classroom = context.Classrooms.FirstOrDefault(c => c.ClassroomId == 1);
            Assert.IsNotNull(classroom);

            EditClassroomViewModel editClassroomViewModel = new EditClassroomViewModel(context, _dialogService)
            {
                ClassroomId = classroom.ClassroomId,
                Location = "",
                Capacity = -1,
                AvailableSeats = -1,
                Projector = false,
                Whiteboard = false,
                Microphone = false,
                Description = ""
            };

            try
            {
                editClassroomViewModel.Save.Execute(null);
            }
            catch (Exception ex)
            {
                // Expected exception due to invalid data
                Assert.IsInstanceOfType(ex, typeof(DbUpdateException));
            }

            var updatedClassroom = context.Classrooms.FirstOrDefault(c => c.ClassroomId == 1);
            Assert.IsNotNull(updatedClassroom);
            Assert.AreNotEqual("", updatedClassroom.Location);
            Assert.AreNotEqual(-1, updatedClassroom.Capacity);
            Assert.AreNotEqual(-1, updatedClassroom.AvailableSeats);
            Assert.AreNotEqual("", updatedClassroom.Description);
        }
    }
    [TestClass]
    public class ClassroomValidationTests
    {
        private List<ValidationResult> ValidateClassroom(Classroom classroom)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(classroom);
            Validator.TryValidateObject(classroom, context, results, true);
            return results;
        }

        [TestMethod]
        public void Validate_ValidClassroom()
        {
            var classroom = new Classroom
            {
                Location = "Building A, Room 101",
                Capacity = 30,
                AvailableSeats = 30,
                Projector = true,
                Whiteboard = true,
                Microphone = true,
                Description = "A spacious classroom with modern amenities."
            };

            var results = ValidateClassroom(classroom);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Validate_ClassroomWithoutLocation()
        {
            var classroom = new Classroom
            {
                Capacity = 30,
                AvailableSeats = 30,
                Projector = true,
                Whiteboard = true,
                Microphone = true,
                Description = "A spacious classroom with modern amenities."
            };

            var results = ValidateClassroom(classroom);
            Assert.IsFalse(results.Exists(r => r.ErrorMessage == "Location is required"));
        }

        [TestMethod]
        public void Validate_ClassroomWithNegativeCapacity()
        {
            var classroom = new Classroom
            {
                Location = "Building A, Room 101",
                Capacity = -5,
                AvailableSeats = 30,
                Projector = true,
                Whiteboard = true,
                Microphone = true,
                Description = "A spacious classroom with modern amenities."
            };

            var results = ValidateClassroom(classroom);
            Assert.IsFalse(results.Exists(r => r.ErrorMessage == "Capacity must be greater than 0"));
        }

        [TestMethod]
        public void Validate_ClassroomWithNegativeAvailableSeats()
        {
            var classroom = new Classroom
            {
                Location = "Building A, Room 101",
                Capacity = 30,
                AvailableSeats = -5,
                Projector = true,
                Whiteboard = true,
                Microphone = true,
                Description = "A spacious classroom with modern amenities."
            };

            var results = ValidateClassroom(classroom);
            Assert.IsFalse(results.Exists(r => r.ErrorMessage == "Available Seats cannot be negative"));
        }

        [TestMethod]
        public void Validate_ClassroomWithoutDescription()
        {
            var classroom = new Classroom
            {
                Location = "Building A, Room 101",
                Capacity = 30,
                AvailableSeats = 30,
                Projector = true,
                Whiteboard = true,
                Microphone = true,
                Description = ""
            };

            var results = ValidateClassroom(classroom);
            Assert.IsFalse(results.Exists(r => r.ErrorMessage == "Description is required"));
        }
    }
}
