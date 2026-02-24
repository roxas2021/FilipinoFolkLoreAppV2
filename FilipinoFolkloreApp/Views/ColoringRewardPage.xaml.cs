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
    private bool _isLoading = true;

    private SoundService SoundService =>
        Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;

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

        RewardLabel.Text = "";
        RewardStarIcon.IsVisible = false;
        OkButton.IsEnabled = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_isLoading)
            return;
        string medalImagePath = DatabaseService.GetMedalImagePath(_medalId);
        try
        {


            if (string.IsNullOrEmpty(medalImagePath))
            {
                System.Diagnostics.Debug.WriteLine($"Warning: Medal image path is empty for medal ID {_medalId}");
                medalImagePath = "medal_empty.png";
            }

            System.Diagnostics.Debug.WriteLine($"Setting medal image: {medalImagePath}");

            await MainThread.InvokeOnMainThreadAsync(() =>
{
   MedalImage.Source = ImageSource.FromFile(medalImagePath);
});
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadMedalsAsync failed in ColoringRewardPage.OnAppearing: {ex}");

            await MainThread.InvokeOnMainThreadAsync(() =>
{
   MedalImage.Source = ImageSource.FromFile("medal_empty.png");
});
        }

        bool alreadyClaimed = Preferences.Get(_rewardKey, false);

        if (alreadyClaimed)
        {
            RewardLabel.Text = "Reward already claimed";
            RewardStarIcon.IsVisible = false;
            OkButton.Text = "Close";
        }
        else
        {
            RewardLabel.Text = $"{_stars}";
            RewardStarIcon.IsVisible = true;
            OkButton.Text = "OK";
        }

        OkButton.IsEnabled = true;
        _isLoading = false;
    }

    private async void OnRewardOk(object? sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();

        bool alreadyClaimed = Preferences.Get(_rewardKey, false);

        if (alreadyClaimed)
        {
            await NavigateBack();
            return;
        }

        try
        {
            var character = await App.Database.AddStarsAsync(_stars);
            CharacterHelper.CurrentStars = character.stars;

            await App.Database.UnlockMedalAsync(_medalId);

            Preferences.Set(_rewardKey, true);

            System.Diagnostics.Debug.WriteLine($"Reward claimed: {_stars} stars, Medal ID: {_medalId}, Key: {_rewardKey}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to award reward: {ex}");
            await DisplayAlert("Error", "Hindi naisave ang reward — subukang muli.", "OK");
            return;
        }

        await NavigateBack();
    }

    private async Task NavigateBack()
    {
        var pages = Navigation.NavigationStack.ToList();
        foreach (var page in pages)
        {
            if (page is ColoringRewardPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is ColoringPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is BugtongQuizPage)
            {
                Navigation.RemovePage(page);
            }
        }

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
                await Navigation.PopAsync();
                break;
        }
    }
}