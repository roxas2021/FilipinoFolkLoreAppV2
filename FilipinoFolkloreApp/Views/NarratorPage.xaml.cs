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

        try
        {
            // Sync in-memory story monitored fields from DB.
            await App.Database.LoadStoriesAsync();
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

        // Initial HUD load (will be refreshed in OnAppearing after DB sync)
        
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

        // If already unlocked for this story or globally, just select and continue
        // We'll rely on RefreshNarratorList() which uses the per-story flags.
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
                await DisplayAlert("Kulang ang ⭐", $"Kailangan: {c.Price}", "OK");
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

                // DO NOT add to AlamatContent.UnlockedNarrators here if you want per-story behavior.
                // AlamatContent.UnlockedNarrators.Add(c.Id); // <-- remove this line
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
                await DisplayAlert("Error", "Hindi naisave ang narrator — subukang muli.", "OK");
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

        AlamatContent.SelectedNarratorId = c.Id;
        await Navigation.PushAsync(new StoryPage(_storyId));
    }


    void LoadHud()
    {
        HudAvatar.Source = CharacterHelper.CurrentAvatar;
        StarsLabel.Text = CharacterHelper.CurrentStars.ToString();
        PlayerNameLabel.Text = CharacterHelper.CurrentName;
        HeartsPanel.Children.Clear();
        for (int i = 0; i < AlamatContent.Hearts; i++)
            HeartsPanel.Children.Add(new Image { Source = "heart_full.png", WidthRequest = 24, HeightRequest = 24, Aspect = Aspect.AspectFit });
    }

    async void OnBackTapped(object? s, TappedEventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
            await Navigation.PopAsync();
    }

    async void OnHomeTapped(object? s, TappedEventArgs e)
    {
        var pages = Navigation.NavigationStack.ToList();
        foreach (var page in pages)
        {
            if (page is RewardPage) Navigation.RemovePage(page);
            if (page is QuizPage) Navigation.RemovePage(page);
            if (page is StoryPage) Navigation.RemovePage(page);
            if (page is NarratorPage) Navigation.RemovePage(page);
            if (page is AlamatPage) Navigation.RemovePage(page);
        }

        await Navigation.PushAsync(new IndexPage());
    }
}
