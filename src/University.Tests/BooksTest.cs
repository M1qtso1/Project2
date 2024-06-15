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

namespace University.Tests
{
    [TestClass]
    public class BookTests
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
            _dialogService = new DialogService();
        }

        private void SeedTestDB()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                context.Database.EnsureDeleted();
                List<Book> books = new List<Book>
                {
                    new Book
                    {
                        BookId = 1, Author = "Orwell", Title = "1984", Genre = "Dystopian", ISBN = "1234567890",
                        Language = "English", PageCount = 328, PublicationDate = new DateTime(1949, 6, 8),
                        Publisher = "Secker & Warburg", Description = "A novel about a dystopian future."
                    }
                };
                context.Books.AddRange(books);
                context.SaveChanges();
            }
        }

        private List<ValidationResult> ValidateBook(Book book)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(book);
            Validator.TryValidateObject(book, context, results, true);
            return results;
        }

        [TestMethod]
        public void Validate_ValidBook()
        {
            var book = new Book
            {
                Author = "George Orwell",
                Title = "1984",
                Genre = "Dystopian",
                ISBN = "1234567890",
                Language = "English",
                PageCount = 328,
                PublicationDate = new DateTime(1949, 6, 8),
                Publisher = "Secker & Warburg",
                Description = "A novel about a dystopian future."
            };

            var results = ValidateBook(book);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Validate_BookWithoutAuthor()
        {
            var book = new Book
            {
                Title = "1984",
                Genre = "Dystopian",
                ISBN = "1234567890",
                Language = "English",
                PageCount = 328,
                PublicationDate = new DateTime(1949, 6, 8),
                Publisher = "Secker & Warburg",
                Description = "A novel about a dystopian future."
            };

            var results = ValidateBook(book);
            Assert.IsFalse(results.Exists(r => r.ErrorMessage == "Author is required"));
        }

        [TestMethod]
        public void Validate_BookWithoutTitle()
        {
            var book = new Book
            {
                Author = "George Orwell",
                Genre = "Dystopian",
                ISBN = "1234567890",
                Language = "English",
                PageCount = 328,
                PublicationDate = new DateTime(1949, 6, 8),
                Publisher = "Secker & Warburg",
                Description = "A novel about a dystopian future."
            };

            var results = ValidateBook(book);
            Assert.IsFalse(results.Exists(r => r.ErrorMessage == "Title is required"));
        }

        [TestMethod]
        public void Validate_BookWithoutISBN()
        {
            var book = new Book
            {
                Author = "George Orwell",
                Title = "1984",
                Genre = "Dystopian",
                Language = "English",
                PageCount = 328,
                PublicationDate = new DateTime(1949, 6, 8),
                Publisher = "Secker & Warburg",
                Description = "A novel about a dystopian future."
            };

            var results = ValidateBook(book);
            Assert.IsFalse(results.Exists(r => r.ErrorMessage == "ISBN is required"));
        }

        [TestMethod]
        public void Validate_BookWithNegativePageCount()
        {
            var book = new Book
            {
                Author = "George Orwell",
                Title = "1984",
                Genre = "Dystopian",
                ISBN = "1234567890",
                Language = "English",
                PageCount = -1,
                PublicationDate = new DateTime(1949, 6, 8),
                Publisher = "Secker & Warburg",
                Description = "A novel about a dystopian future."
            };

            var results = ValidateBook(book);
            Assert.IsFalse(results.Exists(r => r.ErrorMessage == "Page count must be greater than 0"));
        }

        [TestMethod]
        public void Validate_BookWithoutDescription()
        {
            var book = new Book
            {
                Author = "George Orwell",
                Title = "1984",
                Genre = "Dystopian",
                ISBN = "1234567890",
                Language = "English",
                PageCount = 328,
                PublicationDate = new DateTime(1949, 6, 8),
                Publisher = "Secker & Warburg",
                Description = ""
            };

            var results = ValidateBook(book);
            Assert.IsFalse(results.Exists(r => r.ErrorMessage == "Description is required"));
        }

        [TestMethod]
        public void Add_ValidBook()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddBookViewModel addBookViewModel = new AddBookViewModel(context, _dialogService)
                {
                    Author = "Aldous Huxley",
                    Title = "Brave New World",
                    Genre = "Dystopian",
                    ISBN = "0987654321",
                    Language = "English",
                    PageCount = 311,
                    PublicationDate = new DateTime(1932, 8, 18),
                    Publisher = "Chatto & Windus",
                    Description = "A novel about a dystopian future."
                };
                addBookViewModel.Save.Execute(null);

                bool newBookExists = context.Books.Any(b => b.Author == "Aldous Huxley" && b.Title == "Brave New World");
                Assert.IsTrue(newBookExists);
            }
        }

        [TestMethod]
        public void Add_BookWithoutAuthor()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddBookViewModel addBookViewModel = new AddBookViewModel(context, _dialogService)
                {
                    Title = "Brave New World",
                    Genre = "Dystopian",
                    ISBN = "0987654321",
                    Language = "English",
                    PageCount = 311,
                    PublicationDate = new DateTime(1932, 8, 18),
                    Publisher = "Chatto & Windus",
                    Description = "A novel about a dystopian future."
                };
                addBookViewModel.Save.Execute(null);

                bool newBookExists = context.Books.Any(b => b.Title == "Brave New World");
                Assert.IsFalse(newBookExists);
            }
        }

        [TestMethod]
        public void Delete_Book()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                var bookToDelete = context.Books.FirstOrDefault(b => b.BookId == 1);
                Assert.IsNotNull(bookToDelete);

                context.Books.Remove(bookToDelete);
                context.SaveChanges();

                bool bookExists = context.Books.Any(b => b.BookId == 1);
                Assert.IsFalse(bookExists);
            }
        }

        [TestMethod]
        public void Edit_Book()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                var bookToEdit = context.Books.FirstOrDefault(b => b.BookId == 1);
                Assert.IsNotNull(bookToEdit);

                var editBookViewModel = new EditBookViewModel(context, _dialogService)
                {
                    Title = "Nineteen Eighty-Four"
                };
                editBookViewModel.Save.Execute(null);

                bool bookEdited = context.Books.Any(b => b.Title == "Nineteen Eighty-Four");
                Assert.IsFalse(bookEdited);
            }
        }

        [TestMethod]
        public void Edit_BookWithoutTitle()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                var bookToEdit = context.Books.FirstOrDefault(b => b.BookId == 1);
                Assert.IsNotNull(bookToEdit);

                var editBookViewModel = new EditBookViewModel(context, _dialogService)
                {
                    Title = null
                };
                editBookViewModel.Save.Execute(null);

                bool bookEdited = context.Books.Any(b => b.Title == null);
                Assert.IsFalse(bookEdited);
            }
        }
    }
}
