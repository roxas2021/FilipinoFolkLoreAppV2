using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views;
using FilipinoFolkloreApp.Views.Home;
using Plugin.Maui.Audio;

namespace FilipinoFolkloreApp
{
    public partial class App : Application
    {
        static DatabaseService _database;

        private IAudioPlayer? _backgroundMusicPlayer;
        private Stream? _backgroundMusicStream;

        public static DatabaseService Database
        {
            get
            {
                if (_database == null)
                {
                    var path = Path.Combine(FileSystem.AppDataDirectory, "GameData.db3");
                    _database = new DatabaseService(path);
                }
                return _database;
            }
        }

        public App()
        {
            InitializeComponent();
            AlamatContent.MusicIsEnabled = Preferences.Get("MusicEnabled", true);

            Task.Run(async () => await Database.LoadStoriesAsync());
            Task.Run(async () => await Database.LoadMedalsAsync());
            Task.Run(async () => await Database.LoadNarratorDataAsync());
            _ = InitializeBackgroundMusicAsync();
        }

        private async Task InitializeBackgroundMusicAsync()
        {
            try
            {
                _backgroundMusicStream = await FileSystem.OpenAppPackageFileAsync("bgmusic/Homepage.mp3");
                _backgroundMusicPlayer = AudioManager.Current.CreatePlayer(_backgroundMusicStream);

                _backgroundMusicPlayer.Loop = true;
                double savedVolume = Preferences.Get("BackgroundMusicVolume", 0.3);
                _backgroundMusicPlayer.Volume = savedVolume;

                if (AlamatContent.MusicIsEnabled)
                {
                    _backgroundMusicPlayer.Play();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load background music: {ex.Message}");
            }
        }

        public void PauseBackgroundMusic()
        {
            _backgroundMusicPlayer?.Pause();
        }

        public void ResumeBackgroundMusic()
        {
            if (AlamatContent.MusicIsEnabled && _backgroundMusicPlayer != null)
            {
                _backgroundMusicPlayer.Play();
            }
        }

        public void UpdateBackgroundMusic(bool isEnabled)
        {
            AlamatContent.MusicIsEnabled = isEnabled;
            Preferences.Set("MusicEnabled", isEnabled);

            if (isEnabled)
            {
                if (_backgroundMusicPlayer == null)
                {
                    _ = InitializeBackgroundMusicAsync();
                }
                else
                {
                    _backgroundMusicPlayer.Play();
                }
            }
            else
            {
                _backgroundMusicPlayer?.Pause();
            }
        }

        public void SetBackgroundMusicVolume(double volume)
        {
            if (_backgroundMusicPlayer != null)
            {
                _backgroundMusicPlayer.Volume = volume;
            }
        }

        public void StopBackgroundMusic()
        {
            _backgroundMusicPlayer?.Stop();
            _backgroundMusicPlayer?.Dispose();
            _backgroundMusicPlayer = null;

            _backgroundMusicStream?.Dispose();
            _backgroundMusicStream = null;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            ContentPage initialPage = DetermineInitialPage();

            var navigationPage = new NavigationPage(initialPage);

            NavigationPage.SetHasNavigationBar(initialPage, false);
            navigationPage.BarBackgroundColor = Colors.Transparent;
            navigationPage.BarTextColor = Colors.Transparent;

            MainPage = navigationPage;

            var window = new Window(MainPage);

            window.Activated += OnAppActivated;
            window.Deactivated += OnAppDeactivated;
            window.Stopped += OnAppStopped;
            window.Resumed += OnAppResumed;

            return window;
        }

        private void OnAppActivated(object? sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("App Activated");
            ResumeBackgroundMusic();
        }

        private void OnAppDeactivated(object? sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("App Deactivated");
            PauseBackgroundMusic();
        }

        private void OnAppStopped(object? sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("App Stopped");
            PauseBackgroundMusic();
        }

        private void OnAppResumed(object? sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("App Resumed");
            ResumeBackgroundMusic();
        }

        private ContentPage DetermineInitialPage()
        {
            try
            {
                var existingChar = Database.GetCharAsync().GetAwaiter().GetResult();

                if (existingChar != null && !string.IsNullOrWhiteSpace(existingChar.name))
                {
                    var existingAvatarSets = Database.GetAllAvatarSetsAsync().GetAwaiter().GetResult();

                    if (existingAvatarSets != null && existingAvatarSets.Count > 0)
                    {
                        CharacterHelper.CurrentName = existingChar.name;
                        CharacterHelper.CurrentStars = existingChar.stars;
                        CharacterHelper.CurrentAvatar = existingChar.currentavatar;
                        AvatarCustomizationHelper.SelectedAvatarSetId = existingAvatarSets[0].avatarid;

                        return new IndexPage();
                    }
                    else
                    {
                        CharacterHelper.CurrentName = existingChar.name;
                        CharacterHelper.CurrentStars = existingChar.stars;

                        return new AvatarSelectionPage();
                    }
                }

                return new MainPage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DetermineInitialPage failed: {ex}");
                return new MainPage();
            }
        }
    }
}