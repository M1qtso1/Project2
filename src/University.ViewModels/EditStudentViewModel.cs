using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using University.Data;
using University.Extensions;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class EditStudentViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly UniversityContext _context;
        private readonly IDialogService _dialogService;
        private Student? _student = new Student();

        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Name":
                        if (string.IsNullOrEmpty(Name)) return "Name is required";
                        break;
                    case "LastName":
                        if (string.IsNullOrEmpty(LastName)) return "Last Name is required";
                        break;
                    case "PESEL":
                        if (string.IsNullOrEmpty(PESEL)) return "PESEL is required";
                        if (!PESEL.IsValidPESEL()) return "PESEL is invalid";
                        break;
                    case "BirthDate":
                        if (BirthDate is null) return "Birth Date is required";
                        break;
                    case "Gender":
                        if (string.IsNullOrEmpty(Gender)) return "Gender is required";
                        if (Gender != "Male" && Gender != "Female") return "Gender must be 'Male' or 'Female'";
                        break;
                    case "PlaceOfBirth":
                        if (string.IsNullOrEmpty(PlaceOfBirth)) return "Place of Birth is required";
                        break;
                    case "PlaceOfResidence":
                        if (string.IsNullOrEmpty(PlaceOfResidence)) return "Place of Residence is required";
                        break;
                    case "AddressLine1":
                        if (string.IsNullOrEmpty(AddressLine1)) return "Address Line 1 is required";
                        break;
                    case "AddressLine2":
                        if (string.IsNullOrEmpty(AddressLine1)) return "Address Line 2 is required";
                        break;
                    case "PostalCode":
                        if (string.IsNullOrEmpty(PostalCode)) return "Postal Code is required";
                        if (!Regex.IsMatch(PostalCode, @"^\d{2}-\d{3}$")) return "Postal Code is invalid";
                        break;
                }
                return string.Empty;
            }
        }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        private string _pesel = string.Empty;
        public string PESEL
        {
            get => _pesel;
            set
            {
                _pesel = value;
                OnPropertyChanged(nameof(PESEL));
            }
        }

        private DateTime? _birthDate = null;
        public DateTime? BirthDate
        {
            get => _birthDate;
            set
            {
                _birthDate = value;
                OnPropertyChanged(nameof(BirthDate));
            }
        }

        private string _gender = string.Empty;
        public string Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }

        private string _placeOfBirth = string.Empty;
        public string PlaceOfBirth
        {
            get => _placeOfBirth;
            set
            {
                _placeOfBirth = value;
                OnPropertyChanged(nameof(PlaceOfBirth));
            }
        }

        private string _placeOfResidence = string.Empty;
        public string PlaceOfResidence
        {
            get => _placeOfResidence;
            set
            {
                _placeOfResidence = value;
                OnPropertyChanged(nameof(PlaceOfResidence));
            }
        }

        private string _addressLine1 = string.Empty;
        public string AddressLine1
        {
            get => _addressLine1;
            set
            {
                _addressLine1 = value;
                OnPropertyChanged(nameof(AddressLine1));
            }
        }

        private string _addressLine2 = string.Empty;
        public string AddressLine2
        {
            get => _addressLine2;
            set
            {
                _addressLine2 = value;
                OnPropertyChanged(nameof(AddressLine2));
            }
        }

        private string _postalCode = string.Empty;
        public string PostalCode
        {
            get => _postalCode;
            set
            {
                _postalCode = value;
                OnPropertyChanged(nameof(PostalCode));
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

        private long _studentId = 0;
        public long StudentId
        {
            get => _studentId;
            set
            {
                _studentId = value;
                OnPropertyChanged(nameof(StudentId));
                LoadStudentData();
            }
        }

        private ObservableCollection<Subject>? _assignedSubjects = null;
        public ObservableCollection<Subject> AssignedSubjects
        {
            get
            {
                if (_assignedSubjects is null)
                {
                    _assignedSubjects = LoadSubjects();
                    return _assignedSubjects;
                }
                return _assignedSubjects;
            }
            set
            {
                _assignedSubjects = value;
                OnPropertyChanged(nameof(AssignedSubjects));
            }
        }

        private ICommand? _back = null;
        public ICommand Back
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
                instance.StudentsSubView = new StudentsViewModel(_context, _dialogService);
            }
        }

        private ICommand? _save = null;
        public ICommand Save
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

            if (_student is null)
            {
                return;
            }

            _student.Name = Name;
            _student.LastName = LastName;
            _student.PESEL = PESEL;
            _student.BirthDate = BirthDate;
            _student.Gender = Gender;
            _student.PlaceOfBirth = PlaceOfBirth;
            _student.PlaceOfResidence = PlaceOfResidence;
            _student.AddressLine1 = AddressLine1;
            _student.AddressLine2 = AddressLine2;
            _student.PostalCode = PostalCode;
            _student.Subjects = AssignedSubjects.Where(s => s.IsSelected).ToList();

            _context.Entry(_student).State = EntityState.Modified;
            _context.SaveChanges();

            Response = "Data Updated";
        }

        public EditStudentViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
        }

        private ObservableCollection<Subject> LoadSubjects()
        {
            _context.Database.EnsureCreated();
            _context.Subjects.Load();
            return _context.Subjects.Local.ToObservableCollection();
        }

        private bool IsValid()
        {
            string[] properties = { "Name", "LastName", "PESEL", "BirthDate", "Gender", "PlaceOfBirth", "PlaceOfResidence", "AddressLine1", "AddressLine2", "PostalCode" };
            foreach (string property in properties)
            {
                if (!string.IsNullOrEmpty(this[property]))
                {
                    return false;
                }
            }
            return true;
        }

        private void LoadStudentData()
        {
            if (_context?.Students is null)
            {
                return;
            }
            _student = _context.Students.Find(StudentId);
            if (_student is null)
            {
                return;
            }

            Name = _student.Name;
            LastName = _student.LastName;
            PESEL = _student.PESEL;
            BirthDate = _student.BirthDate;
            Gender = _student.Gender;
            PlaceOfBirth = _student.PlaceOfBirth;
            PlaceOfResidence = _student.PlaceOfResidence;
            AddressLine1 = _student.AddressLine1;
            AddressLine2 = _student.AddressLine2;
            PostalCode = _student.PostalCode;

            if (_student.Subjects is null)
            {
                return;
            }

            foreach (var subject in _student.Subjects)
            {
                if (subject != null && AssignedSubjects != null)
                {
                    var assignedSubject = AssignedSubjects.FirstOrDefault(s => s.SubjectId == subject.SubjectId);
                    if (assignedSubject != null)
                    {
                        assignedSubject.IsSelected = true;
                    }
                }
            }
        }
    }
}
