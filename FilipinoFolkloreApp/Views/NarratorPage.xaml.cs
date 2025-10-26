using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FilipinoFolkloreApp.Services;

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

    public NarratorPage(string storyId)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _storyId = storyId;
        LoadHud();
        NarratorsView.ItemsSource = AlamatContent.Narrators.Select(n => new Card
        {
            Id = n.Id,
            Name = n.Name,
            Avatar = n.Avatar,
            IsLocked = !AlamatContent.IsNarratorUnlocked(n.Id),
            Price = n.PriceStars
        }).ToList();
    }

    async void OnNarratorTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Grid g || g.BindingContext is not Card c) return;

        if (!AlamatContent.IsNarratorUnlocked(c.Id))
        {
            if (!AlamatContent.TrySpendStars(c.Price))
            {
                await DisplayAlert("Kulang ang ⭐", $"Kailangan: {c.Price}", "OK");
                return;
            }
            AlamatContent.UnlockedNarrators.Add(c.Id);
        }

        AlamatContent.SelectedNarratorId = c.Id;
        await Navigation.PushAsync(new StoryPage(_storyId));
    }
    void LoadHud()
    {
        HudAvatar.Source = "avatar/avatar1.png";
        StarsLabel.Text = AlamatContent.Stars.ToString();

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
        await Navigation.PopToRootAsync();
    }

}
