using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views;

public partial class ColoringRewardPage : ContentPage
{
    private readonly int _stars;
    private readonly int _medalId;
    private readonly string _rewardKey;
    private readonly string _returnPageType;
    private readonly object? _returnPageParameter;

    public ColoringRewardPage(
        int stars = 20, 
        int medalId = 16, 
        string? rewardKey = null,
        string returnPageType = "ColoringSelection",
        object? returnPageParameter = null)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _stars = stars;
        _medalId = medalId;
        _rewardKey = rewardKey ?? "FirstColoredImageRewardClaimed";
        _returnPageType = returnPageType;
        _returnPageParameter = returnPageParameter;
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
        bool alreadyClaimed = Preferences.Get(_rewardKey, false);

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
        bool alreadyClaimed = Preferences.Get(_rewardKey, false);

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
            Preferences.Set(_rewardKey, true);

            System.Diagnostics.Debug.WriteLine($"Reward claimed: {_stars} stars, Medal ID: {_medalId}, Key: {_rewardKey}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to award reward: {ex}");
            await DisplayAlert("Error", "Hindi naisave ang reward — subukang muli.", "OK");
            return;
        }

        // Navigate back
        await NavigateBack();
    }

    private async Task NavigateBack()
    {
        // Remove this reward page from navigation stack
        var pages = Navigation.NavigationStack.ToList();
        foreach (var page in pages)
        {
            if (page is ColoringRewardPage)
            {
                Navigation.RemovePage(page);
            }
        }

        // Navigate based on return page type
        switch (_returnPageType)
        {
            case "ColoringSelection":
                await Navigation.PushAsync(new ColoringSelectionPage());
                break;
            
            case "NarratorDetail":
                if (_returnPageParameter is string narratorId)
                {
                    await Navigation.PushAsync(new NarratorDetailPage(narratorId));
                }
                break;
            
            case "BugtongList":
                Navigation.RemovePage(this);
                await Navigation.PushAsync(new BugtongListPage());
                break;
            
            default:
                // Just pop if no specific navigation needed
                await Navigation.PopAsync();
                break;
        }
    }
}