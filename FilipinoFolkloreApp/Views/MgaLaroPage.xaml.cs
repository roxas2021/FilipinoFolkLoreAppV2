using Microsoft.Maui.Controls;
using System;
using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views;

public partial class MgaLaroPage : ContentPage
{
    public MgaLaroPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        LoadHUD();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadHUD();
    }

    private void LoadHUD()
    {
        HudAvatar.Source = CharacterHelper.CurrentAvatar;
        PlayerNameLabel.Text = CharacterHelper.CurrentName;
        StarsLabel.Text = CharacterHelper.CurrentStars.ToString();
        NarratorImage.Source = AlamatContent.CurrentNarratorImage;
        NarratorBatteryImage.Source = AlamatContent.GetNarratorBatteryImage();
        RefreshHearts();
    }

    private void RefreshHearts()
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

    private async void OnNarratorTreeTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new NarratorManagementPage());
    }

    private async void OnBugtongTapped(object? sender, EventArgs e)
    {
         await Navigation.PushAsync(new BugtongListPage());
        //await DisplayAlert("Bugtong", "Ang Bugtong page ay wala pa. Gagawin pa ito!", "OK");
    }

    private async void OnMagpintaTapped(object? sender, EventArgs e)
    {
        // Navigate to Coloring Selection page
        await Navigation.PushAsync(new ColoringSelectionPage());
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnHomeTapped(object? sender, TappedEventArgs e)
    {
        await NavigationHelper.NavigateToIndexPage(Navigation);
    }
}