namespace FilipinoFolkloreApp.Views.Home;

public partial class IndexPage : ContentPage
{
	public IndexPage()
	{
		InitializeComponent();

        var data = App.Database.GetCharAsync();
    }
    private async void OnAvatarTapped(object sender, EventArgs e)
    {
        // Navigate to the CharacterCostume page
        await Navigation.PushAsync(new CharacterCostume());
    }
}       