using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LabyrinthGame.ViewModels
{
    public class ChangeProfileViewModel : INotifyPropertyChanged
    //Deklaration der Klasse ChangeProfileViewModel, die das Interface INotifyPropertyChanged implementiert,das es ermöglicht, Änderungen an Eigenschaften zu melden
    {
        //Eigenschaft SettingsCommand vom Typ ICommand mit einem Getter
        //diese Eigenschaft von außerhalb der Klasse gelesen, aber nicht geändert werden kann.
        public ICommand SettingsCommand { get; }
        public ICommand TrophyCommand { get; }
        public ICommand HomeCommand { get; }
        public ICommand CreateProfileCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteProfile1Command { get; }
        public ICommand DeleteProfile2Command { get; }
        public ICommand DeleteProfile3Command { get; }
        private ProgramManager ProgramManager;
        private string _newNickname;
        private bool _isProfile1Checked;
        private bool _isProfile2Checked;
        private bool _isProfile3Checked;
        private BitmapImage profileImage;

        public ChangeProfileViewModel()
        {
            SettingsCommand = new RelayCommand(param => OpenSettings(param));
            TrophyCommand = new RelayCommand(param => OpenTrophy(param));
            HomeCommand = new RelayCommand(param => OpenHome(param));
            ExitCommand = new RelayCommand(param => OpenExit(param));
            SaveCommand = new RelayCommand(param => SaveProfile(param));
            CreateProfileCommand = new RelayCommand(CreateProfile, CanCreateProfile);
            DeleteProfile1Command = new RelayCommand(param => DeleteProfile1());
            DeleteProfile2Command = new RelayCommand(param => DeleteProfile2());
            DeleteProfile3Command = new RelayCommand(param => DeleteProfile3());

            ProgramManager = ProgramManager.Instance;

            //this.ProfileImage = ProgramManager.ProfileManager.GlobalProfileList[0].LoadImage();
        }

        public string Nickname1
        {
            get
            {
                if (ProgramManager.ProfileManager.GlobalProfileList.Count != 0)
                {
                    return ProgramManager.ProfileManager.GlobalProfileList[0].Nickname;
                }
                else
                {
                    return "EMPTY";
                }
            }
        }

        //public BitmapImage ProfileImage
        //{
        //    get => profileImage;
        //    set
        //    {
        //        profileImage = value;
        //        OnPropertyChanged(nameof(ProfileImage));
        //    }
        //}

        public string Nickname2
        {
            get
            {
                if (ProgramManager.ProfileManager.GlobalProfileList.Count > 1)
                {
                    return ProgramManager.ProfileManager.GlobalProfileList[1].Nickname;
                }
                else
                {
                    return "EMPTY";
                }
            }
        }

        public string Nickname3
        {
            get
            {
                if (ProgramManager.ProfileManager.GlobalProfileList.Count > 2)
                {
                    return ProgramManager.ProfileManager.GlobalProfileList[2].Nickname;
                }
                else
                {
                    return "EMPTY";
                }
            }
        }

        public string NewNickname
        {
            get => _newNickname;
            set
            {
                _newNickname = value;
                OnPropertyChanged(nameof(NewNickname));
                ((RelayCommand)CreateProfileCommand).RaiseCanExecuteChanged();
            }
        }

        public bool IsProfile1Checked
        {
            get => _isProfile1Checked;
            set
            {
                _isProfile1Checked = value;
                OnPropertyChanged(nameof(IsProfile1Checked));
                if (value) SelectedProfile = 0;
            }
        }

        public bool IsProfile2Checked
        {
            get => _isProfile2Checked;
            set
            {
                _isProfile2Checked = value;
                OnPropertyChanged(nameof(IsProfile2Checked));
                if (value) SelectedProfile = 1;
            }
        }

        public bool IsProfile3Checked
        {
            get => _isProfile3Checked;
            set
            {
                _isProfile3Checked = value;
                OnPropertyChanged(nameof(IsProfile3Checked));
                if (value) SelectedProfile = 2;
            }
        }

        private int _selectedProfile;
        public int SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                _selectedProfile = value;
                OnPropertyChanged(nameof(SelectedProfile));
            }
        }

        private bool CanCreateProfile(object? parameter)
        {
            return !string.IsNullOrEmpty(NewNickname);
        }

        private void OpenSettings(object? parameter)
        //Methode OpenSettings, die einen Parameter vom Typ object? annimmt
        {
            //Erstellen und Anzeigen des Einstellungsfensters, Schließen des aktuellen Fensters
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenTrophy(object? parameter)
        // Methode OpenTrophy, die einen Parameter vom Typ object? annimmt
        {
            //Erstellen und Anzeigen des Trophäenfensters, Schließen des aktuellen Fensters
            var trophyWindow = new TrophyWindow();
            trophyWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenHome(object? parameter)
        // Methode OpenTrophy, die einen Parameter vom Typ object? annimmt
        {
            //Erstellen und Anzeigen des Hauptfensters, Schließen des aktuellen Fensters
            var homeWindow = new MainWindow();
            homeWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenExit(object? parameter)
        // Methode OpenTrophy, die einen Parameter vom Typ object? annimmt
        {
            //Erstellen und Anzeigen des Ausgangsfensters, Schließen des aktuellen Fensters
            var exitWindow = new ExitWindow();
            exitWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private async void SaveProfile(object? parameter)
        {
            //Überprüfen, ob mindestens ein Profil erstellt wurde
            if (ProgramManager.ProfileManager.GlobalProfileList.Count == 0)
            {
                string previousNewNickname = NewNickname;
                NewNickname = "Please create Profile";
                OnPropertyChanged(nameof(NewNickname));
                await Task.Delay(1750);
                NewNickname = previousNewNickname;
                return;
            }

            // Überprüfen, ob ein Profil ausgewählt ist
            if (!IsProfile1Checked && !IsProfile2Checked && !IsProfile3Checked || ProgramManager.ProfileManager.GlobalProfileList.Count() < SelectedProfile + 1)
            {
                string previousNewNickname = NewNickname;
                NewNickname = "Please select Profile";
                OnPropertyChanged(nameof(NewNickname));
                await Task.Delay(1750);
                NewNickname = previousNewNickname;
                return;
            }

            // Speichern Sie Ihr Profil, falls ausgewählt
            ProgramManager.Profile1 = ProgramManager.ProfileManager.GlobalProfileList[SelectedProfile];

            //Erstellen und Anzeigen eines Profilspeicherfensters, Schließen des aktuellen Fensters
            var saveProfileWindow = new GameModeWindow();
            saveProfileWindow.Show();
            CloseCurrentWindow(parameter);
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
                    // this.ProfileImage = ProgramManager.ProfileManager.GlobalProfileList.FirstOrDefault(profile => profile.Nickname.Equals(NewNickname)).LoadImage();
                    OnPropertyChanged(nameof(ProgramManager.ProfileManager.GlobalProfileList));
                    OnPropertyChanged(nameof(Nickname1));
                    OnPropertyChanged(nameof(Nickname2));
                    OnPropertyChanged(nameof(Nickname3));
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

        private void DeleteProfile1()
        {
            ProgramManager.ProfileManager.DeleteProfile(Nickname1);
            OnPropertyChanged(nameof(Nickname1));
            OnPropertyChanged(nameof(Nickname2));
            OnPropertyChanged(nameof(Nickname3));
        }

        private void DeleteProfile2()
        {
            ProgramManager.ProfileManager.DeleteProfile(Nickname2);
            OnPropertyChanged(nameof(Nickname1));
            OnPropertyChanged(nameof(Nickname2));
            OnPropertyChanged(nameof(Nickname3));
        }

        private void DeleteProfile3()
        {
            ProgramManager.ProfileManager.DeleteProfile(Nickname3);
            OnPropertyChanged(nameof(Nickname1));
            OnPropertyChanged(nameof(Nickname2));
            OnPropertyChanged(nameof(Nickname3));
        }

        private static void CloseCurrentWindow(object? parameter)
        //Deklaration einer privaten statischen Methode CloseCurrentWindow, die einen Parameter vom Typ object? annimmt
        {
            //Überprüfung, ob der Parameter ein Objekt vom Typ Window ist
            if (parameter is Window window)
            {
                //Wenn der Parameter ein Fenster ist, dieses Fenster schließen
                window.Close();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        //Deklaration eines öffentlichen Ereignisses PropertyChanged vom Typ PropertyChangedEventHandler?
        protected virtual void OnPropertyChanged(string propertyName)
        //Deklaration einer geschützten virtuellen Methode OnPropertyChanged, die einen Parameter vom Typ string annimmt
        {
            //Aufrufen des Ereignisses PropertyChanged, wenn es nicht null ist, mit Übergabe des aktuellen Objekts und eines neuen PropertyChangedEventArgs mit dem Eigenschaftsnamen
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}