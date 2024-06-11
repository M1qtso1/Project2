using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class ClassroomViewModel : ViewModelBase
    {
        private readonly UniversityContext _context;
        private readonly IDialogService _dialogService;

        private bool? _dialogResult = null;
        public bool? DialogResult
        {
            get
            {
                return _dialogResult;
            }
            set
            {
                _dialogResult = value;
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
                    return _classrooms;
                }
                return _classrooms;
            }
            set
            {
                _classrooms = value;
                OnPropertyChanged(nameof(Classrooms));
            }
        }

        private ICommand? _add = null;
        public ICommand? Add
        {
            get
            {
                if (_add is null)
                {
                    _add = new RelayCommand<object>(AddNewClassroom);
                }
                return _add;
            }
        }

        private void AddNewClassroom(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.ClassroomsSubView = new AddClassroomViewModel(_context, _dialogService);
            }
        }

        private ICommand? _edit = null;
        public ICommand? Edit
        {
            get
            {
                if (_edit is null)
                {
                    _edit = new RelayCommand<object>(EditClassroom);
                }
                return _edit;
            }
        }

        private void EditClassroom(object? obj)
        {
            if (obj is not null)
            {
                long classroomId = (long)obj;
                EditClassroomViewModel editClassroomViewModel = new EditClassroomViewModel(_context, _dialogService)
                {
                    ClassroomId = classroomId
                };
                var instance = MainWindowViewModel.Instance();
                if (instance is not null)
                {
                    instance.ClassroomsSubView = editClassroomViewModel;
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
                    _remove = new RelayCommand<object>(RemoveClassroom);
                }
                return _remove;
            }
        }

        private void RemoveClassroom(object? obj)
        {
            if (obj is not null)
            {
                long classroomId = (long)obj;
                Classroom? classroom = _context.Classrooms.Find(classroomId);
                if (classroom is not null)
                {
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

        public ClassroomViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            _context.Database.EnsureCreated();
            _context.Classrooms.Load();
            Classrooms = _context.Classrooms.Local.ToObservableCollection();
        }
    }
}
