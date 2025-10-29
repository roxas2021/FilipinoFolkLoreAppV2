
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
        //LoadStories();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Load/sync story monitored data from DB into AlamatContent.Stories
        try
        {
            await App.Database.LoadStoriesAsync();
        }
        catch (Exception ex)
        {
            // optional: log or show an error - don't block UI
            System.Diagnostics.Debug.WriteLine($"LoadStoriesAsync failed: {ex}");
        }

        // refresh UI after DB sync
        LoadHud();
        LoadStories();
    }

    void LoadHud()
    {
        HudAvatar.Source = "avatar/avatar1.png";
        PlayerNameLabel.Text = PlayerNameLabel.Text is null ? "NICHOL" : PlayerNameLabel.Text;
        StarsLabel.Text = AlamatContent.Stars.ToString();
        HeartsPanel.Children.Clear();
        for (int i = 0; i < AlamatContent.Hearts; i++)
        {
            HeartsPanel.Children.Add(new Image
            {
                Source = "heart_full.png",
                WidthRequest = 24,
                HeightRequest = 24
            });
        }
    }
    async void OnBackTapped(object? s, TappedEventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
            await Navigation.PushAsync(new IndexPage());
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
            // Persisted flags
            IsPurchased = s.IsPurchased,
            IsRewardClaimed = s.IsRewardClaimed,

            // Price and display text
            Price = s.PriceStars,

            // Locked = NOT (free OR purchased OR globally unlocked via set)
            IsLocked = !(s.PriceStars == 0 || s.IsPurchased || AlamatContent.UnlockedStories.Contains(s.Id))
        }).ToList();
    }
    async void OnStoryTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Grid g || g.BindingContext is not StoryCard card) return;

        // Get the actual Story object (static)
        var story = AlamatContent.GetStory(card.Id);

        // If story is not unlocked/purchased yet, attempt purchase
        if (!AlamatContent.IsStoryUnlocked(card.Id))
        {
            // Try to spend stars (updates in-memory AlamatContent.Stars)
            if (!AlamatContent.TrySpendStars(card.Price))
            {
                await DisplayAlert("Kulang ang ⭐", $"Kailangan: {card.Price}", "OK");
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

                savedToDb = true;
            }
            catch (Exception ex)
            {
                // Rollback in-memory changes on failure
                story.IsPurchased = false;
                AlamatContent.Stars += card.Price; // refund
                System.Diagnostics.Debug.WriteLine($"UpdateStoryAsync failed: {ex}");

                await DisplayAlert("Error", "Hindi naisave ang binili — subukang muli.", "OK");
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

    //async void OnStoryTapped(object? sender, TappedEventArgs e)
    //{
    //    if (sender is not Grid g || g.BindingContext is not StoryCard card) return;

    //    // Get the actual Story object (static)
    //    var story = AlamatContent.GetStory(card.Id);

    //    // If story is not unlocked/purchased yet, attempt purchase
    //    if (!AlamatContent.IsStoryUnlocked(card.Id))
    //    {
    //        // Try to spend stars
    //        if (!AlamatContent.TrySpendStars(card.Price))
    //        {
    //            await DisplayAlert("Kulang ang ⭐", $"Kailangan: {card.Price}", "OK");
    //            return;
    //        }

    //        // Mark unlocked/purchased in memory
    //        AlamatContent.UnlockedStories.Add(card.Id);
    //        story.IsPurchased = true;

    //        // Persist monitored story data
    //        try
    //        {
    //            await App.Database.UpdateStoryAsync(story);
    //        }
    //        catch (Exception ex)
    //        {
    //            // If DB update fails, roll back in-memory changes (optional)
    //            System.Diagnostics.Debug.WriteLine($"UpdateStoryAsync failed: {ex}");
    //        }

    //        // Refresh HUD and list
    //        LoadHud();
    //        LoadStories();
    //    }

    //    // Navigate to narrator page (story view)
    //    await Navigation.PushAsync(new NarratorPage(card.Id));
    //}


    //async void OnStoryTapped(object? sender, TappedEventArgs e)
    //{
    //    if (sender is not Grid g || g.BindingContext is not StoryCard card) return;

    //    if (!AlamatContent.IsStoryUnlocked(card.Id))
    //    {
    //        if (!AlamatContent.TrySpendStars(card.Price))
    //        {
    //            await DisplayAlert("Kulang ang ⭐", $"Kailangan: {card.Price}", "OK");
    //            return;
    //        }
    //        AlamatContent.UnlockedStories.Add(card.Id);
    //        LoadStories();
    //    }

    //    await Navigation.PushAsync(new NarratorPage(card.Id));
    //}
}
