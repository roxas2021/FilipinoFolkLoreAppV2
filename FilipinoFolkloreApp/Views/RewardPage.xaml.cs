using Microsoft.Maui.Controls;
using System;
using System.Linq;
using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views;

public partial class RewardPage : ContentPage
{
    private readonly int _stars;
    private readonly string? _storyId;

    public RewardPage(int stars, string? storyId = null)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _stars = stars;
        _storyId = storyId;
        RewardText.Text = $"+{_stars} ⭐";
        // Default text (will be updated in OnAppearing after DB sync)
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Sync DB state to ensure we show the correct "already claimed" text
        try
        {
            await App.Database.LoadStoriesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadStoriesAsync failed in RewardPage.OnAppearing: {ex}");
        }

        // If this reward is tied to a story, check whether it was already claimed
        if (!string.IsNullOrEmpty(_storyId))
        {
            var story = AlamatContent.Stories.FirstOrDefault(st => st.Id == _storyId);
            if (story != null && story.IsRewardClaimed)
            {
                RewardText.Text = "Reward already claimed";
                OkButton.Text = "Close";
                // keep the button enabled so user can dismiss, but prevent awarding again
                OkButton.IsEnabled = true;
                return;
            }
        }

        // otherwise show the normal reward text
        RewardText.Text = $"+{_stars} ⭐";
        OkButton.Text = "OK";
        OkButton.IsEnabled = true;
    }

    async void OnRewardOk(object? s, EventArgs e)
    {
        // Ensure latest DB-state (again) to avoid race conditions
        try
        {
            await App.Database.LoadStoriesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadStoriesAsync failed in RewardPage.OnRewardOk: {ex}");
        }

        // If story is provided and the reward has already been claimed, just close
        if (!string.IsNullOrEmpty(_storyId))
        {
            var story = AlamatContent.Stories.FirstOrDefault(st => st.Id == _storyId);
            if (story != null && story.IsRewardClaimed)
            {
                // Remove the reward/quiz/story pages and go back to AlamatPage (same behavior as before)
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
                AlamatContent.Stars += _stars;
                rewardGiven = true;
            }
            else
            {
                if (!story.IsRewardClaimed)
                {
                    story.IsRewardClaimed = true;
                    AlamatContent.Stars += _stars;

                    try
                    {
                        await App.Database.UpdateStoryAsync(story);
                        rewardGiven = true;
                    }
                    catch (Exception ex)
                    {
                        // Rollback on failure
                        story.IsRewardClaimed = false;
                        AlamatContent.Stars -= _stars;
                        System.Diagnostics.Debug.WriteLine($"Failed to update story reward flag: {ex}");
                        await DisplayAlert("Error", "Hindi naisave ang reward — subukang muli.", "OK");
                    }
                }
                else
                {
                    rewardGiven = false; // already claimed
                }
            }
        }
        else
        {
            AlamatContent.Stars += _stars;
            rewardGiven = true;
        }

        // Re-sync to ensure UI consistency
        try
        {
            await App.Database.LoadStoriesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Post-reward LoadStoriesAsync failed: {ex}");
        }

        // Remove reward/quiz/story/narrator pages from nav stack like before
        var pages2 = Navigation.NavigationStack.ToList();
        foreach (var page in pages2)
        {
            if (page is RewardPage) Navigation.RemovePage(page);
            if (page is QuizPage) Navigation.RemovePage(page);
            if (page is StoryPage) Navigation.RemovePage(page);
            if (page is NarratorPage) Navigation.RemovePage(page);
        }

        // Navigate back to AlamatPage (HUD will show updated stars)
        await Navigation.PushAsync(new AlamatPage(AlamatContent.category));
    }
}
