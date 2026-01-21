using Microsoft.Maui.Controls;
using System;
using System.Linq;
using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views;

public partial class ColoringRewardPage : ContentPage
{
    private readonly int _stars;
    private readonly int _medalId;
    private const string FIRST_IMAGE_REWARD_KEY = "FirstColoredImageRewardClaimed";

    public ColoringRewardPage(int stars = 20, int medalId = 16)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _stars = stars;
        _medalId = medalId;
        RewardText.Text = $"+{_stars} ?";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Load medals to get the medal image
        try
        {
            await App.Database.LoadMedalsAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadMedalsAsync failed in ColoringRewardPage.OnAppearing: {ex}");
        }

        // Set the medal image
        MedalImage.Source = DatabaseService.GetMedalImagePath(_medalId);

        // Check if reward was already claimed
        bool alreadyClaimed = Preferences.Get(FIRST_IMAGE_REWARD_KEY, false);

        if (alreadyClaimed)
        {
            RewardText.Text = "Reward already claimed";
            OkButton.Text = "Close";
            OkButton.IsEnabled = true;
            return;
        }

        // Show normal reward text
        RewardText.Text = $"+{_stars} ?";
        OkButton.Text = "OK";
        OkButton.IsEnabled = true;
    }

    private async void OnRewardOk(object? sender, EventArgs e)
    {
        // Check if already claimed (double-check to prevent race conditions)
        bool alreadyClaimed = Preferences.Get(FIRST_IMAGE_REWARD_KEY, false);

        if (alreadyClaimed)
        {
            // Just close and navigate back
            await NavigateBack();
            return;
        }

        try
        {
            // Award the stars
            var character = await App.Database.AddStarsAsync(_stars);
            CharacterHelper.CurrentStars = character.stars;

            // Unlock the medal
            await App.Database.UnlockMedalAsync(_medalId);

            // Mark the reward as claimed
            Preferences.Set(FIRST_IMAGE_REWARD_KEY, true);

            System.Diagnostics.Debug.WriteLine($"First colored image reward claimed: {_stars} stars, Medal ID: {_medalId}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to award coloring reward: {ex}");
            await DisplayAlert("Error", "Hindi naisave ang reward — subukang muli.", "OK");
            return;
        }

        // Navigate back
        await NavigateBack();
    }

    private async Task NavigateBack()
    {
        // Remove this reward page and navigate to ColoringSelectionPage
        var pages = Navigation.NavigationStack.ToList();
        foreach (var page in pages)
        {
            if (page is ColoringRewardPage)
            {
                Navigation.RemovePage(page);
            }
        }

        // Navigate to ColoringSelectionPage
        await Navigation.PushAsync(new ColoringSelectionPage());
    }
}