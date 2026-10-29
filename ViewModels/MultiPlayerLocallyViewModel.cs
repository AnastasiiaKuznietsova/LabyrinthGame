using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LabyrinthGame.ViewModels
{
    public class MultiPlayerLocallyViewModel : INotifyPropertyChanged
    {
        public ICommand HomeCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand TrophyCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CreateProfileCommand { get; }

        private ProgramManager ProgramManager;
        private string _newNickname;
        private bool _isProfile1Checked;
        private bool _isProfile2Checked;
        private int _selectedProfile;


        public string NewNickname
        {
            get { return _newNickname; }
            set { _newNickname = value; OnPropertyChanged(nameof(NewNickname)); }
        }
        public bool IsProfile1Checked
        {
            get => _isProfile1Checked;
            set
            {
                _isProfile1Checked = value;
                OnPropertyChanged(nameof(IsProfile1Checked));
                if (value) SelectedProfile = 1;
            }
        }

        public bool IsProfile2Checked
        {
            get => _isProfile2Checked;
            set
            {
                _isProfile2Checked = value;
                OnPropertyChanged(nameof(IsProfile2Checked));
                if (value) SelectedProfile = 2;
            }
        }

        public int SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                _selectedProfile = value;
                OnPropertyChanged(nameof(SelectedProfile));
            }
        }

        public string Nickname1
        {
            get
            {
                if (ProgramManager.ProfileManager.GlobalProfileList.Count >= 2)
                {
                    return ProgramManager.ProfileManager.GlobalProfileList.FirstOrDefault(profile => profile != ProgramManager.Profile1).Nickname;
                }
                else
                {
                    return "EMPTY";
                }
            }
        }

        public string Nickname2
        {
            get
            {
                if (ProgramManager.ProfileManager.GlobalProfileList.Count >= 3)
                {
                    return ProgramManager.ProfileManager.GlobalProfileList.FirstOrDefault(profile => profile != ProgramManager.Profile1 && profile.Nickname != Nickname1).Nickname;
                }
                else
                {
                    return "EMPTY";
                }
            }
        }

        public MultiPlayerLocallyViewModel()
        {
            HomeCommand = new RelayCommand(param => OpenHomeWindow(param));
            ExitCommand = new RelayCommand(param => OpenExitWindow(param));
            TrophyCommand = new RelayCommand(param => OpenTrophyWindow(param));
            SettingsCommand = new RelayCommand(param => OpenSettingsWindow(param));
            SaveCommand = new RelayCommand(param => OpenSaveWindow(param));
            CreateProfileCommand = new RelayCommand(param => CreateProfile(param));

            ProgramManager = ProgramManager.Instance;
        }

        private async void CreateProfile(object? parameter)
        {
            if (!string.IsNullOrEmpty(NewNickname))
            {
                //Prüfen, ob bereits ein Profil mit demselben Namen vorhanden ist
                if (ProgramManager.ProfileManager.GlobalProfileList.Any(p => p.Nickname == NewNickname))
                {
                    string previousNewNickname = NewNickname;
                    NewNickname = "Profile already exists";
                    OnPropertyChanged(nameof(NewNickname));
                    await Task.Delay(1000);
                    NewNickname = previousNewNickname;
                    OnPropertyChanged(nameof(NewNickname));
                    return;
                }
                if (ProgramManager.ProfileManager.CanCreateNewProfile())
                {
                    ProgramManager.ProfileManager.CreateNewProfile(NewNickname);
                    ProgramManager.ProfileManager.SaveProfiles();
                    OnPropertyChanged(nameof(ProgramManager.ProfileManager.GlobalProfileList));
                    OnPropertyChanged(nameof(Nickname1));
                    OnPropertyChanged(nameof(Nickname2));
                    // Clear the TextBox
                    NewNickname = string.Empty;
                    OnPropertyChanged(nameof(NewNickname));
                }
                else
                {
                    string PreviousNewNickname = NewNickname;
                    NewNickname = "Profile list full";
                    OnPropertyChanged(nameof(NewNickname));
                    await Task.Delay(2000);
                    NewNickname = PreviousNewNickname;
                }
            }
        }

        private async void OpenSaveWindow(object? parameter)
        {
            // Prüfung der Profilauswahl
            if (!IsProfile1Checked && !IsProfile2Checked)
            {
                string previousNewNickname = NewNickname;
                NewNickname = "Please select opponent Profile";
                OnPropertyChanged(nameof(NewNickname));
                await Task.Delay(1000);
                NewNickname = previousNewNickname;
            }
            else
            {
                // Speichern eines Profils
                ProgramManager.Profile2 = ProgramManager.ProfileManager.GlobalProfileList[SelectedProfile];
                ProgramManager.InitializeNewLocalMultiplayerGame();

                var saveWindow = new FirstPlayingFieldWindow();
                saveWindow.Show();
                CloseCurrentWindow(parameter);
            }
        }

        private void OpenHomeWindow(object? parameter)
        {
            var homeWindow = new MainWindow();
            homeWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenExitWindow(object? parameter)
        {
            var exitWindow = new ExitWindow();
            exitWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenTrophyWindow(object? parameter)
        {
            var trophyWindow = new TrophyWindow();
            trophyWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenSettingsWindow(object? parameter)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void CloseCurrentWindow(object? parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}