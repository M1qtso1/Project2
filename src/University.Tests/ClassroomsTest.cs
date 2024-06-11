using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using University.Data;
using University.Interfaces;
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
            Assert.IsFalse(newClassroomExists);
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
            Assert.IsFalse(newClassroomExists);
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
}
