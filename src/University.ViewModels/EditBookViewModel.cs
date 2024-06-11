using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using System.Xml.Linq;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class EditBookViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly UniversityContext _context;
        private readonly IDialogService _dialogService;
        private Book? _book = new Book();
        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Title":
                        if (string.IsNullOrEmpty(Title)) return "Title is required";
                        break;
                    case "Author":
                        if (string.IsNullOrEmpty(Author)) return "Author is required";
                        break;
                    case "Publisher":
                        if (string.IsNullOrEmpty(Publisher)) return "Publisher is required";
                        break;
                    case "PublicationDate":
                        if (PublicationDate == null) return "Publication Date is required";
                        break;
                    case "ISBN":
                        if (string.IsNullOrEmpty(ISBN)) return "ISBN is required";
                        break;
                    case "Genre":
                        if (string.IsNullOrEmpty(Genre)) return "Genre is required";
                        break;
                    case "Description":
                        if (string.IsNullOrEmpty(Description)) return "Description is required";
                        break;
                    case "Language":
                        if (string.IsNullOrEmpty(Language)) return "Language is required";
                        break;
                    case "PageCount":
                        if (PageCount <= 0) return "Page Count must be greater than 0";
                        break;
                }
                return string.Empty;
            }
        }

        private long _bookId;
        public long BookId
        {
            get => _bookId;
            set
            {
                _bookId = value;
                OnPropertyChanged(nameof(BookId));
                LoadBookData();
            }
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _author = string.Empty;
        public string Author
        {
            get => _author;
            set
            {
                _author = value;
                OnPropertyChanged(nameof(Author));
            }
        }

        private string _publisher = string.Empty;
        public string Publisher
        {
            get => _publisher;
            set
            {
                _publisher = value;
                OnPropertyChanged(nameof(Publisher));
            }
        }

        private DateTime? _publicationDate = null;
        public DateTime? PublicationDate
        {
            get => _publicationDate;
            set
            {
                _publicationDate = value;
                OnPropertyChanged(nameof(PublicationDate));
            }
        }

        private string _isbn = string.Empty;
        public string ISBN
        {
            get => _isbn;
            set
            {
                _isbn = value;
                OnPropertyChanged(nameof(ISBN));
            }
        }
        private string _language = string.Empty;
        public string Language
        {
            get => _language;
            set
            {
                _language = value;
                OnPropertyChanged(nameof(Language));
            }
        }
        private string _response = string.Empty;
        public string Response
        {
            get => _response;
            set
            {
                _response = value;
                OnPropertyChanged(nameof(Response));
            }
        }

        private int _pageCount = 0;
        public int PageCount
        {
            get => _pageCount;
            set
            {
                _pageCount = value;
                OnPropertyChanged(nameof(PageCount));
            }
        }
        private string _genre = string.Empty;
        public string Genre
        {
            get => _genre;
            set
            {
                _genre = value;
                OnPropertyChanged(nameof(Genre));
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        private ICommand? _back = null;
        public ICommand? Back
        {
            get
            {
                if (_back is null)
                {
                    _back = new RelayCommand<object>(NavigateBack);
                }
                return _back;
            }
        }

        private void NavigateBack(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.BooksSubView = new BooksViewModel(_context, _dialogService);
            }
        }
        private ICommand? _save = null;
        public ICommand? Save
        {
            get
            {
                if (_save is null)
                {
                    _save = new RelayCommand<object>(SaveData);
                }
                return _save;
            }
        }

        private void SaveData(object? obj)
        {
            if (!IsValid())
            {Response = "Please complete all required fields"; return; }         if (_book is not null)
            {_book.Title = Title ;_book.Author = Author; _book.Publisher = Publisher; _book.PublicationDate = PublicationDate;
                _book.ISBN = ISBN;                  _book.Genre = Genre;
                                            _book.Description = Description; 
                                                                                                                                                                        _book.Language = Language;
                _book.PageCount = PageCount                                                                             ; _context.Entry(_book).State = EntityState.Modified;
                _context.SaveChanges();

                Response = "Data Updated";
            }
        }
        private void LoadBookData()
        {
            if (_context?.Books is null)
            {
                return;
            }
            _book = _context.Books.Find(BookId);
            if (_book is null)
            {
                return;
            }

            Title = _book.Title; Author = _book.Author; Publisher = _book.Publisher; PublicationDate = _book.PublicationDate;
            ISBN = _book.ISBN; Genre = _book.Genre;
            Description = _book.Description;
            Language = _book.Language;                                                                      PageCount = _book.PageCount;
        }
        public EditBookViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
        }

        private bool IsValid()
        {
            string[] properties = { "Title", "Author", "Publisher", "PublicationDate", "ISBN", "Genre", "Description", "Language", "PageCount"};
            foreach (string property in properties)
            {
                if (!string.IsNullOrEmpty(this[property]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
