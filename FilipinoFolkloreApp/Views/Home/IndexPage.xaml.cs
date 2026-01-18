using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views.Home;

public partial class IndexPage : ContentPage
{
    private HeartService HeartService =>
    Application.Current!.Handler!.MauiContext!.Services.GetService<HeartService>()!;
    private static bool _hasSetAsRoot = false;
    public IndexPage()
    {
        InitializeComponent();

        loadhud();
        var data = App.Database.GetCharAsync();
        LoadSettings();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        AlamatContent.Hearts = HeartService.GetHearts();
        loadhud();
        LoadSettings();
        if (!_hasSetAsRoot && Application.Current?.MainPage is NavigationPage navPage)
        {
            // Check if we're not already the root
            if (navPage.Navigation.NavigationStack.Count > 1 ||
                navPage.Navigation.NavigationStack.FirstOrDefault() is not IndexPage)
            {
                // Set IndexPage as the new root
                Application.Current.MainPage = new NavigationPage(this);
                _hasSetAsRoot = true;
            }
        }
        if (Application.Current is App app)
        {
            app.ResumeBackgroundMusic();
        }
    }
    void loadhud()
    {
        HudAvatar.Source = CharacterHelper.CurrentAvatar;
        PlayerNameLabel.Text = CharacterHelper.CurrentName;
        StarsLabel.Text = CharacterHelper.CurrentStars.ToString();

        RefreshHearts();

    }
    void RefreshHearts()
    {
        HeartsPanel.Children.Clear();
        for (int i = 0; i < AlamatContent.Hearts; i++)
        {
            HeartsPanel.Children.Add(new Image
            {
                Source = "heart_full.png",
                WidthRequest = 24,
                HeightRequest = 24,
                Aspect = Aspect.AspectFit
            });
        }
    }
    private async void OnAvatarTapped(object sender, EventArgs e)
    {
        // Navigate to the CharacterCostume page
        await Navigation.PushAsync(new CharacterCostume());
    }
    private async void OnAlamatClicked(object sender, EventArgs e)
    {
        // Navigate to the AlamatList page
        await Navigation.PushAsync(new AlamatPage("alamat"));
    }
    private async void OnEpikoClicked(object sender, EventArgs e)
    {
        // Navigate to the AlamatList page
        await Navigation.PushAsync(new AlamatPage("epiko"));
    }
    private async void OnPabulaClicked(object sender, EventArgs e)
    {
        // Navigate to the AlamatList page
        await Navigation.PushAsync(new AlamatPage("pabula"));
    }
    private async void OnMedalyaClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MedalPage());
    }
    private void OnSettingsClicked(object sender, EventArgs e)
    {
        SettingsModalOverlay.IsVisible = true;
    }
    private void OnCloseSettingsModal(object sender, EventArgs e)
    {
        SettingsModalOverlay.IsVisible = false;

    }
    private void OnMusicToggled(object sender, ToggledEventArgs e)
    {
        // Control background music
        if (Application.Current is App app)
        {
            app.UpdateBackgroundMusic(e.Value);
            
        }
    }
    private void LoadSettings()
    {

        // Load saved settings from the global static property
        bool musicEnabled = AlamatContent.MusicIsEnabled;
        // Load saved settings from Preferences
        MusicSwitch.Toggled -= OnMusicToggled;
        MusicSwitch.IsToggled = musicEnabled;
        MusicSwitch.Toggled += OnMusicToggled;
    }
}