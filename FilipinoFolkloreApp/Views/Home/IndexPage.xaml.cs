using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views.Home;

public partial class IndexPage : ContentPage
{
	public IndexPage()
	{
		InitializeComponent();

        loadhud();
        var data = App.Database.GetCharAsync();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        loadhud();
    }
    void loadhud()
    {
        HudAvatar.Source = CharacterHelper.CurrentAvatar;
        PlayerNameLabel.Text = CharacterHelper.CurrentName;
        StarsLabel.Text = CharacterHelper.CurrentStars.ToString();

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
}       