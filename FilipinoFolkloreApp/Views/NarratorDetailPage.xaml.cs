using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views.Home;

namespace FilipinoFolkloreApp.Views;

public partial class NarratorDetailPage : ContentPage
{
    private readonly string _narratorId;
    private TaskCompletionSource<bool>? _alertTcs;

    private SoundService SoundService =>
        Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;

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
        NavigationPage.SetHasNavigationBar(this, false);
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

    private async void ShowNarratorOverlay()
    {
        var narrator = AlamatContent.Narrators.FirstOrDefault(n => n.Id == _narratorId);
        if (narrator != null)
        {
            OverlayNarratorImage.Source = narrator.Avatar;
            NarratorBackgroundLabel.Text = narrator.NarratorBackground;
            NarratorOverlay.IsVisible = true;
        }
    }

    private async void OnCloseOverlayClicked(object? sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        NarratorOverlay.IsVisible = false;
    }

    private void OnOverlayBackgroundTapped(object? sender, TappedEventArgs e)
    {
        // Optional: Close overlay when tapping outside the content
        // NarratorOverlay.IsVisible = false;
    }

    private async void OnFoodTapped(object? sender, TappedEventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        
        if (sender is not Grid grid || grid.BindingContext is not FoodItem food)
            return;

        // Check if battery is already full
        if (AlamatContent.NarratorBattery >= 3)
        {
            await ShowGameAlertAsync(
                "Ang narrator battery ay puno na!",
                false
            );
            return;
        }

        // Check if player has enough stars
        if (CharacterHelper.CurrentStars < food.Price)
        {
            await ShowGameAlertAsync(
                $"Kailangan mo ng {food.Price} stars para bumili ng {food.Name}.",
                false
            );
            return;
        }

        // Confirm purchase
        bool confirm = await ShowGameAlertAsync(
            $"Bumili ng {food.Name} para sa {food.Price} stars?\nMagdadagdag ng {food.BatteryRestore} battery.",
            true
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

            await ShowGameAlertAsync(
                $"Nabili ang {food.Name}! Battery restored by {food.BatteryRestore}.",
                false
            );

            // Check for achievements
            await CheckAndAwardAchievements(previousBattery);
        }
        catch (Exception ex)
        {
            await ShowGameAlertAsync(
                $"Hindi nai-save ang pagbili: {ex.Message}",
                false
            );
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
                // Medal ID for narrator first full battery achievement
                await Navigation.PushAsync(new ColoringRewardPage(
                    stars: 50,
                    medalId: 22,
                    rewardKey: firstFullBatteryKey,
                    returnPageType: "NarratorDetail",
                    returnPageParameter: _narratorId
                ));
                return;
            }
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
        await SoundService.PlayButtonClickAsync();
        await HideGameAlertAsync(true);
    }

    private async void OnAlertYesClicked(object? sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await HideGameAlertAsync(true);
    }

    private async void OnAlertNoClicked(object? sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await HideGameAlertAsync(false);
    }

    private async void OnAlertBackgroundTapped(object? sender, EventArgs e)
    {
        await HideGameAlertAsync(false);
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await Navigation.PopAsync();
    }

    private async void OnHomeTapped(object? sender, TappedEventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        
        var pages = Navigation.NavigationStack.ToList();
        foreach (var page in pages)
        {
            if (page is NarratorManagementPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is NarratorDetailPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is MgaLaroPage)
            {
                Navigation.RemovePage(page);
            }
        }

        await Navigation.PushAsync(new IndexPage());
    }
}