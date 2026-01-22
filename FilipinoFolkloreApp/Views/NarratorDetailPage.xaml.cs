using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Collections.Generic;
using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views;

public partial class NarratorDetailPage : ContentPage
{
    private readonly string _narratorId;

    class FoodItem
    {
        public string Name { get; set; } = "";
        public string Image { get; set; } = "";
        public int Price { get; set; }
        public int BatteryRestore { get; set; }
    }

    private static readonly List<FoodItem> Foods = new()
    {
        new FoodItem { Name = "Grass", Image = "grass.png", Price = 10, BatteryRestore = 1 },
        new FoodItem { Name = "Banana", Image = "banana.png", Price = 15, BatteryRestore = 1 },
        new FoodItem { Name = "Apple", Image = "apple.png", Price = 25, BatteryRestore = 2 },
        new FoodItem { Name = "Guava", Image = "guava.png", Price = 40, BatteryRestore = 3 },
        new FoodItem { Name = "Sugarcane", Image = "sugarcane.png", Price = 50, BatteryRestore = 3 }
    };

    public NarratorDetailPage(string narratorId)
    {
        InitializeComponent();
        _narratorId = narratorId;
        LoadPage();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadHUD();
        UpdateBatteryDisplay();
        ShowNarratorOverlay();
    }

    private void LoadPage()
    {
        LoadHUD();
        LoadNarratorInfo();
        LoadFoodItems();
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

    private void LoadNarratorInfo()
    {
        var narrator = AlamatContent.Narrators.FirstOrDefault(n => n.Id == _narratorId);
        if (narrator != null)
        {
            NarratorImage.Source = narrator.Avatar;
        }
        UpdateBatteryDisplay();
    }

    private void UpdateBatteryDisplay()
    {
        BatteryImage.Source = AlamatContent.GetNarratorBatteryImage();
    }

    private void LoadFoodItems()
    {
        FoodItemsView.ItemsSource = Foods;
    }

    private void ShowNarratorOverlay()
    {
        var narrator = AlamatContent.Narrators.FirstOrDefault(n => n.Id == _narratorId);
        if (narrator != null)
        {
            OverlayNarratorImage.Source = narrator.Avatar;
            NarratorBackgroundLabel.Text = narrator.NarratorBackground;
            NarratorOverlay.IsVisible = true;
        }
    }

    private void OnCloseOverlayClicked(object? sender, EventArgs e)
    {
        NarratorOverlay.IsVisible = false;
    }

    private void OnOverlayBackgroundTapped(object? sender, TappedEventArgs e)
    {
        // Optional: Close overlay when tapping outside the content
        // NarratorOverlay.IsVisible = false;
    }

    private async void OnFoodTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Grid grid || grid.BindingContext is not FoodItem food)
            return;

        // Check if battery is already full
        if (AlamatContent.NarratorBattery >= 3)
        {
            await DisplayAlert("Battery Full", "Ang narrator battery ay puno na!", "OK");
            return;
        }

        // Check if player has enough stars
        if (CharacterHelper.CurrentStars < food.Price)
        {
            await DisplayAlert("Kulang ang Stars", $"Kailangan mo ng {food.Price} stars para bumili ng {food.Name}.", "OK");
            return;
        }

        // Confirm purchase
        bool confirm = await DisplayAlert(
            "Confirm Purchase",
            $"Bumili ng {food.Name} para sa {food.Price} stars?\nMagdadagdag ng {food.BatteryRestore} battery.",
            "Yes",
            "No"
        );

        if (!confirm)
            return;

        try
        {
            // Track previous battery level
            int previousBattery = AlamatContent.NarratorBattery;
            
            // Deduct stars
            CharacterHelper.CurrentStars -= food.Price;
            await App.Database.SetStarsAsync(CharacterHelper.CurrentStars);

            // Restore battery
            AlamatContent.NarratorBattery = Math.Min(3, AlamatContent.NarratorBattery + food.BatteryRestore);
            AlamatContent.LastNarratorUseTime = DateTime.Now;
            await App.Database.UpdateNarratorBatteryAsync(AlamatContent.NarratorBattery, AlamatContent.LastNarratorUseTime);

            // Update UI
            LoadHUD();
            UpdateBatteryDisplay();

            await DisplayAlert("Success", $"Nabili ang {food.Name}! Battery restored by {food.BatteryRestore}.", "OK");

            // Check for achievements
            await CheckAndAwardAchievements(previousBattery);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Hindi nai-save ang pagbili: {ex.Message}", "OK");
        }
    }

    private async Task CheckAndAwardAchievements(int previousBattery)
    {
        // Check if battery just became full
        if (AlamatContent.NarratorBattery == 3 && previousBattery < 3)
        {
            // Check if this is the first time reaching full battery
            string firstFullBatteryKey = $"Narrator_{_narratorId}_FirstFullBattery";
            bool firstFullBattery = Preferences.Get(firstFullBatteryKey, false);
            
            if (!firstFullBattery)
            {
                // Show reward page using ColoringRewardPage
                // Medal ID 17 for narrator first full battery achievement
                await Navigation.PushAsync(new ColoringRewardPage(
                    stars: 50,
                    medalId: 17,
                    rewardKey: firstFullBatteryKey,
                    returnPageType: "NarratorDetail",
                    returnPageParameter: _narratorId
                ));
                return;
            }
        }
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnHomeTapped(object? sender, TappedEventArgs e)
    {
        await NavigationHelper.NavigateToIndexPage(Navigation);
    }
}