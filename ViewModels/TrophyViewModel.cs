using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using LabyrinthGame;

namespace LabyrinthGame.ViewModels
{
    public class TrophyViewModel : INotifyPropertyChanged
    {
        public ICommand HomeCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand CloseCommand { get; }
        private ProgramManager ProgramManager;

        public ObservableCollection<Profile> Profiles { get; set; }

        public TrophyViewModel()
        {
            HomeCommand = new RelayCommand(param => OpenHomeWindow(param));
            ExitCommand = new RelayCommand(param => OpenExitWindow(param));
            SettingsCommand = new RelayCommand(param => OpenSettingsWindow(param));
            CloseCommand = new RelayCommand(param => OpenCloseWindow(param));
            ProgramManager = ProgramManager.Instance;

            Profiles = new ObservableCollection<Profile>();
            LoadProfiles();

            if (Profiles.Count == 0)
            {
                Profiles.Add(new Profile("Allpha") { Highscore = 32 });
                Profiles.Add(new Profile("Command") { Highscore = 18 });
                Profiles.Add(new Profile("Wicked") { Highscore = 15 });
                Profiles.Add(new Profile("Banshee") { Highscore = 4 });
                Profiles.Add(new Profile("Beast") { Highscore = 1 });


                SaveProfiles(); //Speichern von Originalprofilen
            }
        }

        public void UpdateOverallResults(string playerName, int playerScore)
        {
            var existingProfile = Profiles.FirstOrDefault(p => p.Nickname == playerName);
            if (existingProfile != null)
            {
                if (playerScore > existingProfile.Highscore)
                {
                    existingProfile.Highscore = playerScore;
                }
            }
            else
            {
                Profiles.Add(new Profile(playerName) { Highscore = playerScore });
            }

            // Sortieren von Profilen nach Punktzahl von der höchsten zur niedrigsten
            var sortedProfiles = Profiles.OrderByDescending(p => p.Highscore).ToList();

            // Keep only the top 10 profiles
            if (sortedProfiles.Count > 10)
            {
                sortedProfiles = sortedProfiles.Take(10).ToList();
            }

            // Update the Profiles collection
            Profiles.Clear();
            foreach (var sortedProfile in sortedProfiles)
            {
                Profiles.Add(sortedProfile);
            }

            SaveProfiles();
        }

        public void SaveProfiles()
        {
            File.WriteAllText("overall_profiles.json", JsonConvert.SerializeObject(Profiles, Formatting.Indented));
        }

        public void LoadProfiles()
        {
            if (File.Exists("overall_profiles.json"))
            {
                Profiles = JsonConvert.DeserializeObject<ObservableCollection<Profile>>(File.ReadAllText("overall_profiles.json"));
            }
            else
            {
                Profiles = new ObservableCollection<Profile>();
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

        private void OpenSettingsWindow(object? parameter)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenCloseWindow(object? parameter)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
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
