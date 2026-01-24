
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FilipinoFolkloreApp.Views.Home;
using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views;

public partial class AlamatPage : ContentPage
{
    private HeartService HeartService =>
    Application.Current!.Handler!.MauiContext!.Services.GetService<HeartService>()!;
    private TaskCompletionSource<bool>? _alertTcs;

    public class StoryCard
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Thumb { get; set; } = "";
        public bool IsLocked { get; set; }
        public string Category { get; set; } = "";
        public bool IsPurchased { get; set; }
        public bool IsRewardClaimed { get; set; }
        public int Price { get; set; }
        public string PriceText => $"{Price}⭐";
    }

    public AlamatPage(string category)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        AlamatContent.category = category;

        LoadHud();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            await App.Database.LoadStoriesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadStoriesAsync failed: {ex}");
        }

        LoadHud();
        LoadStories();
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
    void LoadHud()
    {
        HudAvatar.Source = CharacterHelper.CurrentAvatar;
        PlayerNameLabel.Text = CharacterHelper.CurrentName;
        StarsLabel.Text = CharacterHelper.CurrentStars.ToString();
        RefreshHearts();
    }
    async void OnBackTapped(object? s, TappedEventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
            await Navigation.PopAsync(); ;
    }

    async void OnHomeTapped(object? s, TappedEventArgs e)
    {


        await Navigation.PushAsync(new IndexPage());

    }

    void LoadStories()
    {
        var currentCategory = AlamatContent.category ?? "";

        StoriesView.ItemsSource = AlamatContent.Stories.Where(s => string.IsNullOrEmpty(currentCategory)
                || (!string.IsNullOrEmpty(s.Category)
                    && s.Category.Equals(currentCategory, StringComparison.OrdinalIgnoreCase))).Select(s => new StoryCard
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Thumb = s.Thumb,
                        IsPurchased = s.IsPurchased,
                        IsRewardClaimed = s.IsRewardClaimed,

                        Price = s.PriceStars,
                        IsLocked = !(s.PriceStars == 0 || s.IsPurchased || AlamatContent.UnlockedStories.Contains(s.Id))
                    }).ToList();
    }
    async void OnStoryTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Grid g || g.BindingContext is not StoryCard card) return;

        var story = AlamatContent.GetStory(card.Id);

        if (!AlamatContent.IsStoryUnlocked(card.Id))
        {
            // Try to spend stars (updates in-memory AlamatContent.Stars)
            if (!AlamatContent.TrySpendStars(card.Price))
            {
                await ShowGameAlertAsync($"Kailangan: {card.Price}", false);
                return;
            }

            // Optimistically mark as purchased in memory
            story.IsPurchased = true;

            bool savedToDb = false;
            try
            {
                // Persist monitored story data
                await App.Database.UpdateStoryAsync(story);

                // Keep the fast-check set in sync (UpdateStoryAsync also tries to sync it,
                // but we ensure it here immediately so UI checks are consistent).
                AlamatContent.UnlockedStories.Add(story.Id);
                await App.Database.SetStarsAsync(CharacterHelper.CurrentStars - card.Price);
                CharacterHelper.CurrentStars -= card.Price; // keep in sync
                savedToDb = true;
            }
            catch (Exception ex)
            {
                // Rollback in-memory changes on failure
                story.IsPurchased = false;
                AlamatContent.Stars += card.Price; // refund
                System.Diagnostics.Debug.WriteLine($"UpdateStoryAsync failed: {ex}");

                await ShowGameAlertAsync("Hindi naisave ang binili — subukang muli.", false);
            }

            // If saving failed, stop here (user was refunded). If saved, refresh UI.
            if (!savedToDb)
                return;

            // Refresh HUD and list
            LoadHud();
            LoadStories();
        }
        AlamatContent.CurrentStoryId = story.Id;

        // Navigate to narrator page (story view)
        await Navigation.PushAsync(new NarratorPage(card.Id));
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
}