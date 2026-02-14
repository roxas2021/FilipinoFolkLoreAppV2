using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views.Home;

public partial class IndexPage : ContentPage
{
    private HeartService HeartService =>
        Application.Current!.Handler!.MauiContext!.Services.GetService<HeartService>()!;
    
    public IndexPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
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
        
        // Resume background music based on settings
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
        await Navigation.PushAsync(new CharacterCostume());
    }
    
    private async void OnAlamatClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AlamatPage("alamat"));
    }
    
    private async void OnEpikoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AlamatPage("epiko"));
    }
    
    private async void OnPabulaClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AlamatPage("pabula"));
    }
    
    private async void OnMedalyaClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MedalPage());
    }
    private async void OnMgaLaroClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MgaLaroPage());
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
        if (Application.Current is App app)
        {
            app.UpdateBackgroundMusic(e.Value);
        }
    }
    private void OnBackgroundVolumeChanged(object sender, ValueChangedEventArgs e)
    {
        int volumePercent = (int)e.NewValue;
        BackgroundVolumeLabel.Text = $"{volumePercent}%";

        double volumeValue = e.NewValue / 100.0;
        Preferences.Set("BackgroundMusicVolume", volumeValue);

        if (Application.Current is App app)
        {
            app.SetBackgroundMusicVolume(volumeValue);
        }
    }
    private async void OnKreditsClicked(object sender, EventArgs e)
    {
        // Close the settings modal first
        SettingsModalOverlay.IsVisible = false;

        // Navigate to Credits page
        await Navigation.PushAsync(new CreditsPage());
    }
    private void OnVolumeChanged(object sender, ValueChangedEventArgs e)
    {
        // Update the volume label
        int volumePercent = (int)e.NewValue;
        BackgroundVolumeLabel.Text = $"{volumePercent}%";
        
        // Save to preferences (0.0 to 1.0 range)
        double volumeValue = e.NewValue / 100.0;
        Preferences.Set("NarratorVolume", volumeValue);
        
        // Store in AlamatContent for easy access
        AlamatContent.NarratorVolume = volumeValue;
    }
    private void OnNarratorVolumeChanged(object sender, ValueChangedEventArgs e)
    {
        // Update the volume label
        int volumePercent = (int)e.NewValue;
        NarratorVolumeLabel.Text = $"{volumePercent}%";

        // Save to preferences (0.0 to 1.0 range)
        double volumeValue = e.NewValue / 100.0;
        Preferences.Set("NarratorVolume", volumeValue);

        // Store in AlamatContent for easy access
        AlamatContent.NarratorVolume = volumeValue;
    }
    private void LoadSettings()
    {
        
        double savedBackgroundVolume = Preferences.Get("BackgroundMusicVolume", 0.3);
        BackgroundVolumeSlider.ValueChanged -= OnBackgroundVolumeChanged;
        BackgroundVolumeSlider.Value = savedBackgroundVolume * 100;
        BackgroundVolumeLabel.Text = $"{(int)(savedBackgroundVolume * 100)}%";
        BackgroundVolumeSlider.ValueChanged += OnBackgroundVolumeChanged;

        double savedNarratorVolume = Preferences.Get("NarratorVolume", 1.0);
        AlamatContent.NarratorVolume = savedNarratorVolume;

        NarratorVolumeSlider.ValueChanged -= OnNarratorVolumeChanged;
        NarratorVolumeSlider.Value = savedNarratorVolume * 100;
        NarratorVolumeLabel.Text = $"{(int)(savedNarratorVolume * 100)}%";
        NarratorVolumeSlider.ValueChanged += OnNarratorVolumeChanged;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}