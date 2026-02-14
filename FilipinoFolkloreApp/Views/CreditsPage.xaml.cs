namespace FilipinoFolkloreApp.Views;

public partial class CreditsPage : ContentPage
{
    public CreditsPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void OnBackClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }
}