
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
        PlayerNameLabel.Text = PlayerNameLabel.Text is null ? "NICHOL" : PlayerNameLabel.Text;
        StarsLabel.Text = AlamatContent.Stars.ToString();
        HeartsPanel.Children.Clear();
        for (int i = 0; i < AlamatContent.Hearts; i++)
        {
            HeartsPanel.Children.Add(new Image
            {
                Source = "heart_full.png",
                WidthRequest = 20,
                HeightRequest = 20
            });
        }
    }
    async void OnBackTapped(object? s, TappedEventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
            await Navigation.PopAsync();
    }

    async void OnHomeTapped(object? s, TappedEventArgs e)
    {
        await Navigation.PopToRootAsync();
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
