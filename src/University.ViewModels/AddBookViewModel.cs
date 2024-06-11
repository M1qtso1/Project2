using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class AddBookViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly UniversityContext _context;
        private readonly IDialogService _dialogService;

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                    switch (columnName)
                {
                    case nameof(Title):
                        if (string.IsNullOrEmpty(Title)) return "Title is required";
                        break;
                    case nameof(Author):
                        if (string.IsNullOrEmpty(Author)) return "Author is required";
                        break;
                    case nameof(Publisher):
                        if (string.IsNullOrEmpty(Publisher)) return "Publisher is required";
                        break;
                    case nameof(PublicationDate):
                        if (PublicationDate == null) return "Publication Date is required";
                        break;
                    case nameof(ISBN):
                        if (string.IsNullOrEmpty(ISBN)) return "ISBN is required";
                        break;
                    case nameof(Genre):
                        if (string.IsNullOrEmpty(Genre)) return "Genre is required";
                        break;
                    case nameof(Description):
                        if (string.IsNullOrEmpty(Description)) return "Description is required";
                        break;
                    case nameof(Language):
                        if (string.IsNullOrEmpty(Language)) return "Language is required";
                        break;
                    case nameof(PageCount):
                        if (PageCount <= 0) return "Page Count must be greater than 0";
                        break;
                }
                return string.Empty;
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
            {
                Response = "Please complete all required fields";
                return;
            }

            Book book = new Book
            {
                Title = this.Title,
                Author = this.Author,
                Publisher = this.Publisher,
                PublicationDate = this.PublicationDate,
                ISBN = this.ISBN,
                Genre = this.Genre,
                Description = this.Description,
                Language = this.Language,
                PageCount = this.PageCount,
            };

            _context.Books.Add(book);
            _context.SaveChanges();

            Response = "Data Saved";
        }

        public AddBookViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
        }

        private bool IsValid()
        {
            string[] properties = { nameof(Title), nameof(Author), nameof(Publisher), nameof(PublicationDate), nameof(ISBN), nameof(Genre), nameof(Description), nameof(Language), nameof(PageCount) };
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
