using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views.Home;

namespace FilipinoFolkloreApp.Views;

public partial class NarratorPage : ContentPage
{
    private readonly string _storyId;
    private TaskCompletionSource<bool>? _alertTcs;
    private HeartService HeartService =>
    Application.Current!.Handler!.MauiContext!.Services.GetService<HeartService>()!;

    class Card
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Avatar { get; set; } = "";
        public bool IsLocked { get; set; }
        public int Price { get; set; }
        public string PriceText => $"{Price}⭐";
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AlamatContent.Hearts = HeartService.GetHearts();

        // Check and refresh narrator battery
        AlamatContent.CheckAndRefreshNarratorBattery();

        try
        {
            // Sync in-memory story monitored fields from DB.
            await App.Database.LoadStoriesAsync();
            await App.Database.LoadNarratorDataAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error syncing stories on appear: {ex}");
        }

        // Refresh UI now that data is consistent
        LoadHud();
        RefreshNarratorList();
    }

    public NarratorPage(string storyId)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _storyId = storyId;
    }

    void RefreshNarratorList()
    {
        // Get the current story (safe lookup)
        var story = AlamatContent.Stories.FirstOrDefault(s => s.Id == _storyId);

        NarratorsView.ItemsSource = AlamatContent.Narrators.Select(n =>
        {
            // per-story unlock check: tarsier always unlocked, or global unlocked set,
            // or the current story has the narrator unlocked flag set.
            bool unlocked = n.Id == "tarsier"
                            || AlamatContent.UnlockedNarrators.Contains(n.Id) // global unlocks if used
                            || (story != null && n.Id == "eagle" && story.NarratorEagleUnlocked)
                            || (story != null && n.Id == "monkey" && story.NarratorMonkeyUnlocked);

            return new Card
            {
                Id = n.Id,
                Name = n.Name,
                Avatar = n.Avatar,
                IsLocked = !unlocked,
                Price = n.PriceStars
            };
        }).ToList();
    }

    async void OnNarratorTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Grid g || g.BindingContext is not Card c) return;

        // Check narrator battery before proceeding
        if (!AlamatContent.CanUseNarrator())
        {
            // Calculate time until next battery refresh
            var timeSinceLastUse = DateTime.Now - AlamatContent.LastNarratorUseTime;
            var minutesUntilRefresh = 10 - ((int)timeSinceLastUse.TotalMinutes % 10);

            await ShowGameAlertAsync(
                $"Maghintay ng {minutesUntilRefresh} minuto para sa susunod na narrator battery.",
                false
            );
            return;
        }

        // If already unlocked for this story or globally, just select and continue
        var story = AlamatContent.GetStory(_storyId);

        bool alreadyUnlockedForThisStory =
            c.Id == "tarsier" ||
            AlamatContent.UnlockedNarrators.Contains(c.Id) || // global
            (c.Id == "eagle" && story.NarratorEagleUnlocked) ||
            (c.Id == "monkey" && story.NarratorMonkeyUnlocked);

        if (!alreadyUnlockedForThisStory)
        {
            if (!AlamatContent.TrySpendStars(c.Price))
            {
                await ShowGameAlertAsync($"Kailangan: {c.Price}", false);
                return;
            }

            // Save previous flags to rollback on DB failure
            bool previousEagle = story.NarratorEagleUnlocked;
            bool previousMonkey = story.NarratorMonkeyUnlocked;

            // set the per-story flag
            switch (c.Id)
            {
                case "eagle":
                    story.NarratorEagleUnlocked = true;
                    break;
                case "monkey":
                    story.NarratorMonkeyUnlocked = true;
                    break;
            }

            bool saved = false;
            try
            {
                await App.Database.UpdateStoryAsync(story);
                await App.Database.SetStarsAsync(CharacterHelper.CurrentStars - c.Price);
                CharacterHelper.CurrentStars -= c.Price; // keep in sync
                saved = true;
            }
            catch (Exception ex)
            {
                // rollback if DB save fails: restore story flags and refund stars
                story.NarratorEagleUnlocked = previousEagle;
                story.NarratorMonkeyUnlocked = previousMonkey;
                AlamatContent.Stars += c.Price; // refund
                System.Diagnostics.Debug.WriteLine($"UpdateStoryAsync failed while unlocking narrator: {ex}");
                await ShowGameAlertAsync("Hindi naisave ang narrator — subukang muli.", false);
            }

            if (!saved)
            {
                LoadHud();
                RefreshNarratorList();
                return;
            }

            // saved ok -> refresh HUD and list
            LoadHud();
            RefreshNarratorList();
        }

        // Use narrator battery (deduct 1)
        await AlamatContent.UseNarratorAsync();

        // Save selected narrator to database
        AlamatContent.SelectedNarratorId = c.Id;
        AlamatContent.CurrentNarratorImage = c.Avatar;
        await App.Database.UpdateSelectedNarratorAsync(c.Id);

        await Navigation.PushAsync(new StoryPage(_storyId));
    }

    void LoadHud()
    {
        HudAvatar.Source = CharacterHelper.CurrentAvatar;
        StarsLabel.Text = CharacterHelper.CurrentStars.ToString();
        PlayerNameLabel.Text = CharacterHelper.CurrentName;
        RefreshHearts();
    }

    void RefreshHearts()
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

    // Custom Game Alert with Yes/No or OK buttons
    private Task<bool> ShowGameAlertAsync(string message, bool showYesNo = false)
    {
        if (GameAlertOverlay.IsVisible && _alertTcs != null)
            return _alertTcs.Task;

        _alertTcs = new TaskCompletionSource<bool>();

        // Set message
        AlertMessageLabel.Text = message;

        // Clear existing buttons
        AlertButtonsPanel.Children.Clear();

        if (showYesNo)
        {
            // Add Yes button
            var yesButton = new Button
            {
                Text = "Oo",
                FontAttributes = FontAttributes.Bold,
                CornerRadius = 18,
                HeightRequest = 44,
                WidthRequest = 100,
                BackgroundColor = Color.FromArgb("#00A6FF"),
                TextColor = Colors.White
            };
            yesButton.Clicked += (s, e) => OnAlertYesClicked(s, e);
            AlertButtonsPanel.Children.Add(yesButton);

            // Add No button
            var noButton = new Button
            {
                Text = "Hindi",
                FontAttributes = FontAttributes.Bold,
                CornerRadius = 18,
                HeightRequest = 44,
                WidthRequest = 100,
                BackgroundColor = Color.FromArgb("#FF6B6B"),
                TextColor = Colors.White
            };
            noButton.Clicked += (s, e) => OnAlertNoClicked(s, e);
            AlertButtonsPanel.Children.Add(noButton);
        }
        else
        {
            // Add OK button
            var okButton = new Button
            {
                Text = "OK",
                FontAttributes = FontAttributes.Bold,
                CornerRadius = 18,
                HeightRequest = 44,
                WidthRequest = 120,
                BackgroundColor = Color.FromArgb("#00A6FF"),
                TextColor = Colors.White
            };
            okButton.Clicked += (s, e) => OnAlertOkClicked(s, e);
            AlertButtonsPanel.Children.Add(okButton);
        }

        GameAlertOverlay.IsVisible = true;
        GameAlertOverlay.Opacity = 0;
        GameAlertCard.Scale = 0.96;

        _ = AnimateShowOverlayAsync();

        return _alertTcs.Task;
    }

    private async Task AnimateShowOverlayAsync()
    {
        try
        {
            await GameAlertOverlay.FadeTo(1, 180, Easing.CubicIn);
            await GameAlertCard.ScaleTo(1.06, 220, Easing.CubicOut);
            await GameAlertCard.ScaleTo(1.0, 120, Easing.CubicIn);
        }
        catch { }
    }

    private async Task HideGameAlertAsync(bool result)
    {
        if (!GameAlertOverlay.IsVisible) return;

        try
        {
            await GameAlertCard.ScaleTo(0.96, 120, Easing.CubicIn);
            await GameAlertOverlay.FadeTo(0, 140, Easing.CubicOut);
        }
        catch { }

        GameAlertOverlay.IsVisible = false;

        _alertTcs?.TrySetResult(result);
        _alertTcs = null;
    }

    private async void OnAlertOkClicked(object? sender, EventArgs e)
    {
        await HideGameAlertAsync(true);
    }

    private async void OnAlertYesClicked(object? sender, EventArgs e)
    {
        await HideGameAlertAsync(true);
    }

    private async void OnAlertNoClicked(object? sender, EventArgs e)
    {
        await HideGameAlertAsync(false);
    }

    private async void OnAlertBackgroundTapped(object? sender, EventArgs e)
    {
        await HideGameAlertAsync(false);
    }

    async void OnBackTapped(object? s, TappedEventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
            await Navigation.PopAsync();
    }

    async void OnHomeTapped(object? s, TappedEventArgs e)
    {
        await NavigationHelper.NavigateToIndexPage(Navigation);
    }
}