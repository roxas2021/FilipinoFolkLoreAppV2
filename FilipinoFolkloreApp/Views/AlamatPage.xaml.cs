
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
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
        public int Price { get; set; }
        public string PriceText => $"{Price}⭐";
    }

    public AlamatPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        LoadHud();
        LoadStories();
    }

    void LoadHud()
    {
        HudAvatar.Source = AlamatContent.CurrentNarrator.Avatar;
        HudHearts.Text = $"{AlamatContent.Hearts}";
    }

    void LoadStories()
    {
        StoriesView.ItemsSource = AlamatContent.Stories.Select(s => new StoryCard
        {
            Id = s.Id,
            Title = s.Title,
            Thumb = s.Thumb,
            IsLocked = !AlamatContent.IsStoryUnlocked(s.Id),
            Price = s.PriceStars
        }).ToList();
    }

    async void OnStoryTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Grid g || g.BindingContext is not StoryCard card) return;

        if (!AlamatContent.IsStoryUnlocked(card.Id))
        {
            if (!AlamatContent.TrySpendStars(card.Price))
            {
                await DisplayAlert("Kulang ang ⭐", $"Kailangan: {card.Price}", "OK");
                return;
            }
            AlamatContent.UnlockedStories.Add(card.Id);
            LoadStories();
        }

        await Navigation.PushAsync(new NarratorPage(card.Id));
    }
}
