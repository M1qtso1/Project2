using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private readonly UniversityContext _context;
        private readonly IDialogService _dialogService;

        private bool? _dialogResult = null;
        public bool? DialogResult
        {
            get => _dialogResult;
            set
            {
                _dialogResult = value;
                OnPropertyChanged(nameof(DialogResult));
            }
        }

        private string _firstCondition = string.Empty;
        public string FirstCondition
        {
            get => _firstCondition;
            set
            {
                _firstCondition = value;
                OnPropertyChanged(nameof(FirstCondition));
            }
        }

        private string _secondCondition = string.Empty;
        public string SecondCondition
        {
            get => _secondCondition;
            set
            {
                _secondCondition = value;
                OnPropertyChanged(nameof(SecondCondition));
            }
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        private bool _areStudentsVisible;
        public bool AreStudentsVisible
        {
            get => _areStudentsVisible;
            set
            {
                _areStudentsVisible = value;
                OnPropertyChanged(nameof(AreStudentsVisible));
            }
        }

        private bool _areSubjectsVisible;
        public bool AreSubjectsVisible
        {
            get => _areSubjectsVisible;
            set
            {
                _areSubjectsVisible = value;
                OnPropertyChanged(nameof(AreSubjectsVisible));
            }
        }

        private bool _areBooksVisible;
        public bool AreBooksVisible
        {
            get => _areBooksVisible;
            set
            {
                _areBooksVisible = value;
                OnPropertyChanged(nameof(AreBooksVisible));
            }
        }

        private bool _areClassroomsVisible;
        public bool AreClassroomsVisible
        {
            get => _areClassroomsVisible;
            set
            {
                _areClassroomsVisible = value;
                OnPropertyChanged(nameof(AreClassroomsVisible));
            }
        }

        private ObservableCollection<Student>? _students = null;
        public ObservableCollection<Student>? Students
        {
            get
            {
                if (_students is null)
                {
                    _students = new ObservableCollection<Student>();
                }
                return _students;
            }
            set
            {
                _students = value;
                OnPropertyChanged(nameof(Students));
            }
        }

        private ObservableCollection<Subject>? _subjects = null;
        public ObservableCollection<Subject>? Subjects
        {
            get
            {
                if (_subjects is null)
                {
                    _subjects = new ObservableCollection<Subject>();
                }
                return _subjects;
            }
            set
            {
                _subjects = value;
                OnPropertyChanged(nameof(Subjects));
            }
        }

        private ObservableCollection<Book>? _books = null;
        public ObservableCollection<Book>? Books
        {
            get
            {
                if (_books is null)
                {
                    _books = new ObservableCollection<Book>();
                }
                return _books;
            }
            set
            {
                _books = value;
                OnPropertyChanged(nameof(Books));
            }
        }

        private ObservableCollection<Classroom>? _classrooms = null;
        public ObservableCollection<Classroom>? Classrooms
        {
            get
            {
                if (_classrooms is null)
                {
                    _classrooms = new ObservableCollection<Classroom>();
                }
                return _classrooms;
            }
            set
            {
                _classrooms = value;
                OnPropertyChanged(nameof(Classrooms));
            }
        }

        private ICommand? _comboBoxSelectionChanged = null;
        public ICommand? ComboBoxSelectionChanged
        {
            get
            {
                if (_comboBoxSelectionChanged is null)
                {
                    _comboBoxSelectionChanged = new RelayCommand<object>(UpdateCondition);
                }
                return _comboBoxSelectionChanged;
            }
        }

        private void UpdateCondition(object? obj)
        {
            if (obj is string objAsString)
            {
                IsVisible = true;
                string selectedValue = objAsString;
                SecondCondition = string.Empty;
                if (selectedValue == "Students")
                {
                    FirstCondition = "who attends";
                }
                else if (selectedValue == "Subjects")
                {
                    FirstCondition = "attended by Student with PESEL";
                }
                else if (selectedValue == "Books")
                {
                    FirstCondition = "written by Author";
                }
                else if (selectedValue == "Classrooms")
                {
                    FirstCondition = "located in Building";
                }
            }
        }

        private ICommand? _search = null;
        public ICommand? Search
        {
            get
            {
                if (_search is null)
                {
                    _search = new RelayCommand<object>(SelectData);
                }
                return _search;
            }
        }

        private void SelectData(object? obj)
        {
            _context.Database.EnsureCreated();
            if (FirstCondition == "who attends")
            {
                Subject? subject = _context.Subjects.FirstOrDefault(s => s.Name == SecondCondition);
                if (subject is not null)
                {
                    var students = _context.Students
                        .Include(s => s.Subjects)
                        .Where(s => s.Subjects.Any(sub => sub.Name == subject.Name))
                        .ToList();

                    Students = new ObservableCollection<Student>(students);
                    AreSubjectsVisible = false;
                    AreBooksVisible = false;
                    AreClassroomsVisible = false;
                    AreStudentsVisible = true;
                }
            }
            else if (FirstCondition == "attended by Student with PESEL")
            {
                Student? student = _context.Students.FirstOrDefault(s => s.PESEL == SecondCondition);
                if (student is not null)
                {
                    var subjects = _context.Subjects
                        .Include(s => s.Students)
                        .Where(s => s.Students.Any(st => st.PESEL == student.PESEL))
                        .ToList();

                    Subjects = new ObservableCollection<Subject>(subjects);
                    AreStudentsVisible = false;
                    AreBooksVisible = false;
                    AreSubjectsVisible = true;
                    AreClassroomsVisible = false;
                }
            }
            else if (FirstCondition == "written by Author")
            {
                var books = _context.Books
                    .Where(b => b.Author.Contains(SecondCondition))
                    .ToList();

                Books = new ObservableCollection<Book>(books);
                AreStudentsVisible = false;
                AreSubjectsVisible = false;
                AreBooksVisible = true;
                AreClassroomsVisible = false;
            }
            else if (FirstCondition == "located in Building")
            {
                var classrooms = _context.Classrooms
                    .Where(c => c.Location.Contains(SecondCondition))
                    .ToList();

                Classrooms = new ObservableCollection<Classroom>(classrooms);
                AreStudentsVisible = false;
                AreSubjectsVisible = false;
                AreBooksVisible = false;
                AreClassroomsVisible = true;
            }
        }

        private ICommand? _edit = null;
        public ICommand? Edit
        {
            get
            {
                if (_edit is null)
                {
                    _edit = new RelayCommand<object>(EditItem);
                }
                return _edit;
            }
        }

        private void EditItem(object? obj)
        {
            if (obj is not null)
            {
                var instance = MainWindowViewModel.Instance();
                if (FirstCondition == "who attends" && obj is long studentId)
                {
                    EditStudentViewModel editStudentViewModel = new EditStudentViewModel(_context, _dialogService)
                    {
                        StudentId = studentId
                    };
                    if (instance is not null)
                    {
                        instance.StudentsSubView = editStudentViewModel;
                        instance.SelectedTab = 0;
                    }
                }
                else if (FirstCondition == "attended by Student with PESEL" && obj is long subjectId)
                {
                    EditSubjectViewModel editSubjectViewModel = new EditSubjectViewModel(_context, _dialogService)
                    {
                        SubjectId = subjectId
                    };
                    if (instance is not null)
                    {
                        instance.SubjectsSubView = editSubjectViewModel;
                        instance.SelectedTab = 1;
                    }
                }
                else if (FirstCondition == "written by Author" && obj is long bookId)
                {
                    EditBookViewModel editBookViewModel = new EditBookViewModel(_context, _dialogService)
                    {
                        BookId = bookId
                    };
                    if (instance is not null)
                    {
                        instance.BooksSubView = editBookViewModel;
                        instance.SelectedTab = 2;
                    }
                }
                else if (FirstCondition == "located in Building" && obj is long classroomId)
                {
                    EditClassroomViewModel editClassroomViewModel = new EditClassroomViewModel(_context, _dialogService)
                    {
                        ClassroomId = classroomId
                    };
                    if (instance is not null)
                    {
                        instance.ClassroomsSubView = editClassroomViewModel;
                        instance.SelectedTab = 3;
                    }
                }
            }
        }

        private ICommand? _remove = null;
        public ICommand? Remove
        {
            get
            {
                if (_remove is null)
                {
                    _remove = new RelayCommand<object>(RemoveItem);
                }
                return _remove;
            }
        }

        private void RemoveItem(object? obj)
        {
            if (obj is not null)
            {
                if (FirstCondition == "who attends" && obj is long studentId)
                {
                    Student? student = _context.Students.Find(studentId);
                    if (student is null)
                    {
                        return;
                    }

                    DialogResult = _dialogService.Show($"{student.Name} {student.LastName}");
                    if (DialogResult == false)
                    {
                        return;
                    }
                    _context.Students.Remove(student);
                    _context.SaveChanges();
                }
                else if (FirstCondition == "attended by Student with PESEL" && obj is long subjectId)
                {
                    Subject? subject = _context.Subjects.Find(subjectId);
                    if (subject is null)
                    {
                        return;
                    }

                    DialogResult = _dialogService.Show(subject.Name);
                    if (DialogResult == false)
                    {
                        return;
                    }
                    _context.Subjects.Remove(subject);
                    _context.SaveChanges();
                }
                else if (FirstCondition == "written by Author" && obj is string bookId)
                {
                    Book? book = _context.Books.Find(bookId);
                    if (book is null)
                    {
                        return;
                    }

                    DialogResult = _dialogService.Show(book.Title);
                    if (DialogResult == false)
                    {
                        return;
                    }
                    _context.Books.Remove(book);
                    _context.SaveChanges();
                }
                else if (FirstCondition == "located in Building" && obj is long classroomId)
                {
                    Classroom? classroom = _context.Classrooms.Find(classroomId);
                    if (classroom is null)
                    {
                        return;
                    }

                    DialogResult = _dialogService.Show(classroom.Location);
                    if (DialogResult == false)
                    {
                        return;
                    }
                    _context.Classrooms.Remove(classroom);
                    _context.SaveChanges();
                }
            }
        }

        public SearchViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            IsVisible = false;
            AreStudentsVisible = false;
            AreSubjectsVisible = false;
            AreBooksVisible = false;
            AreClassroomsVisible = false;
        }
    }
}
