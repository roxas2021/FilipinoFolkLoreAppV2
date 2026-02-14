using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views;

public partial class CreditsPage : ContentPage
{
    private SoundService SoundService =>
    Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;

    public CreditsPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void OnBackClicked(object? sender, TappedEventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await Navigation.PopAsync();
    }
}