using Microsoft.Maui.Controls;
using System;
using System.Linq;
using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views;

public partial class NarratorManagementPage : ContentPage
{
    class NarratorCard
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Avatar { get; set; } = "";
        public bool IsLocked { get; set; }
        public bool ShowBattery => !IsLocked;
        public int Battery { get; set; }
        public string BatteryImage { get; set; } = "";
        public int Price { get; set; }
        public double Opacity => IsLocked ? 0.5 : 1.0;
        public Color NameColor => IsLocked ? Color.FromArgb("#757575") : Color.FromArgb("#333333");
    }

    public NarratorManagementPage()
    {
        InitializeComponent();
        LoadHUD();
        LoadNarrators();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadHUD();
        LoadNarrators();
    }

    private void LoadHUD()
    {
        HudAvatar.Source = CharacterHelper.CurrentAvatar;
        PlayerNameLabel.Text = CharacterHelper.CurrentName;
        StarsLabel.Text = CharacterHelper.CurrentStars.ToString();
        RefreshHearts();
    }

    private void RefreshHearts()
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

    private void LoadNarrators()
    {
        var narratorCards = AlamatContent.Narrators.Select(narrator =>
        {
            bool isUnlocked = narrator.Id == "tarsier" || AlamatContent.UnlockedNarrators.Contains(narrator.Id);

            return new NarratorCard
            {
                Id = narrator.Id,
                Name = narrator.Name,
                Avatar = narrator.Avatar,
                IsLocked = !isUnlocked,
                Battery = isUnlocked ? AlamatContent.NarratorBattery : 0,
                BatteryImage = isUnlocked ? AlamatContent.GetNarratorBatteryImage() : "batteryempty.png",
                Price = narrator.PriceStars
            };
        }).ToList();

        NarratorsView.ItemsSource = narratorCards;
    }

    private async void OnNarratorTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Grid grid && grid.BindingContext is NarratorCard card)
        {
            if (!card.IsLocked)
            {
                await Navigation.PushAsync(new NarratorDetailPage(card.Id));
            }
            else
            {
                await DisplayAlert("Locked", 
                    $"{card.Name} ay naka-lock pa. Kailangan mo ng {card.Price} stars para i-unlock.", 
                    "OK");
            }
        }
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }

    
}