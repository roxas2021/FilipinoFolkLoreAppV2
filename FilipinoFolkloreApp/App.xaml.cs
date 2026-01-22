using FilipinoFolkloreApp.Services;
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
            Task.Run(async() => await Database.LoadMedalsAsync());
            Task.Run(async() => await Database.LoadNarratorDataAsync()); // Load narrator data
            // Start background music
            _ = InitializeBackgroundMusicAsync();
        }
        private async Task InitializeBackgroundMusicAsync()
        {
            try
            {
                

                // Load your background music file (replace with your actual music file path)
                _backgroundMusicStream = await FileSystem.OpenAppPackageFileAsync("bgmusic/Homepage.mp3");
                _backgroundMusicPlayer = AudioManager.Current.CreatePlayer(_backgroundMusicStream);

                // Set to loop
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
            // Only resume if music is enabled
            if (AlamatContent.MusicIsEnabled && _backgroundMusicPlayer != null)
            {
                _backgroundMusicPlayer.Play();
            }
        }

        public void UpdateBackgroundMusic(bool isEnabled)
        {
            // Update the global state and save to preferences
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
            return new Window(new AppShell());
        }
    }
}