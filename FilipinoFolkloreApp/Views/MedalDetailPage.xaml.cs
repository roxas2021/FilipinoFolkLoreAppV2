using FilipinoFolkloreApp.Models;
using Microsoft.Maui.Controls;

namespace FilipinoFolkloreApp.Views;

public partial class MedalDetailPage : ContentPage
{
    private readonly Medals _medal;

    public MedalDetailPage(Medals medal)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        _medal = medal;
        LoadMedalDetails();
    }

    private void LoadMedalDetails()
    {
        MedalTitleLabel.Text = _medal.MedalName;
        MedalImageView.Source = _medal.MedalImagePath;
        MedalDescriptionLabel.Text = _medal.MedalDescription;
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
            await Navigation.PopAsync(); ;
    }

    private async void OnInfoTapped(object? sender, TappedEventArgs e)
    {
        InfoOverlay.IsVisible = true;
        await InfoOverlay.FadeTo(1, 200);
    }

    private async void OnCloseInfoTapped(object? sender, EventArgs e)
    {
        await InfoOverlay.FadeTo(0, 150);
        InfoOverlay.IsVisible = false;
    }
}