using Microsoft.Maui.Controls;
using System;
using System.Linq;
using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views;

public partial class RewardPage : ContentPage
{
    private readonly int _stars;
    private readonly string? _storyId;

    private SoundService SoundService =>
        Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;

    public RewardPage(int stars, string? storyId = null)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _stars = stars;
        _storyId = storyId;
        RewardLabel.Text = $"{_stars}";
        RewardStarIcon.IsVisible = true;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var MedalId = AlamatContent.GetStory(_storyId).MedalId;
        MedalImage.Source = DatabaseService.GetMedalImagePath(MedalId);
        try
        {
            await App.Database.LoadStoriesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadStoriesAsync failed in RewardPage.OnAppearing: {ex}");
        }

        if (!string.IsNullOrEmpty(_storyId))
        {
            var story = AlamatContent.Stories.FirstOrDefault(st => st.Id == _storyId);


            if (story != null && story.IsRewardClaimed)
            {
                RewardLabel.Text = "Reward already claimed";
                RewardStarIcon.IsVisible = false;
                OkButton.Text = "Close";

                OkButton.IsEnabled = true;
                return;
            }
        }


        RewardLabel.Text = $"{_stars}";
        RewardStarIcon.IsVisible = true;
        OkButton.Text = "OK";
        OkButton.IsEnabled = true;
    }

    async void OnRewardOk(object? s, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();

        try
        {
            await App.Database.LoadStoriesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadStoriesAsync failed in RewardPage.OnRewardOk: {ex}");
        }

        if (!string.IsNullOrEmpty(_storyId))
        {
            var story = AlamatContent.Stories.FirstOrDefault(st => st.Id == _storyId);
            if (story != null && story.IsRewardClaimed)
            {
                var pages = Navigation.NavigationStack.ToList();
                foreach (var page in pages)
                {
                    if (page is RewardPage) Navigation.RemovePage(page);
                    if (page is QuizPage) Navigation.RemovePage(page);
                    if (page is StoryPage) Navigation.RemovePage(page);
                    if (page is NarratorPage) Navigation.RemovePage(page);
                }

                await Navigation.PushAsync(new AlamatPage(AlamatContent.category));
                return;
            }
        }

        bool rewardGiven = false;

        if (!string.IsNullOrEmpty(_storyId))
        {
            var story = AlamatContent.Stories.FirstOrDefault(st => st.Id == _storyId);

            if (story == null)
            {
                AlamatContent.Stars = _stars;
                rewardGiven = true;
            }
            else
            {
                if (!story.IsRewardClaimed)
                {
                    story.IsRewardClaimed = true;
                    AlamatContent.Stars += _stars;
                    await App.Database.UnlockMedalAsync(story.MedalId);
                    try
                    {
                        await App.Database.UpdateStoryAsync(story);

                        rewardGiven = true;
                    }
                    catch (Exception ex)
                    {
                        story.IsRewardClaimed = false;
                        AlamatContent.Stars -= _stars;
                        System.Diagnostics.Debug.WriteLine($"Failed to update story reward flag: {ex}");
                        await DisplayAlert("Error", "Hindi naisave ang reward — subukang muli.", "OK");
                    }
                }
                else
                {
                    rewardGiven = false;
                }
            }
        }
        else
        {
            AlamatContent.Stars += _stars;
            rewardGiven = true;
        }

        try
        {
            await App.Database.LoadStoriesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Post-reward LoadStoriesAsync failed: {ex}");
        }

        var pages2 = Navigation.NavigationStack.ToList();
        foreach (var page in pages2)
        {
            if (page is RewardPage) Navigation.RemovePage(page);
            if (page is QuizPage) Navigation.RemovePage(page);
            if (page is StoryPage) Navigation.RemovePage(page);
            if (page is NarratorPage) Navigation.RemovePage(page);
        }

        await Navigation.PushAsync(new AlamatPage(AlamatContent.category));
    }
}
