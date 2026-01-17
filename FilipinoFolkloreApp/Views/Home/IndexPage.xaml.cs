using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views.Home;

public partial class IndexPage : ContentPage
{
    private HeartService HeartService =>
    Application.Current!.Handler!.MauiContext!.Services.GetService<HeartService>()!;
    public IndexPage()
    {
        InitializeComponent();

        loadhud();
        var data = App.Database.GetCharAsync();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        AlamatContent.Hearts = HeartService.GetHearts();
        loadhud();
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
}