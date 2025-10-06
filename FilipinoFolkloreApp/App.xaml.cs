using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp
{
    public partial class App : Application
    {
        static DatabaseService _database;

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
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}