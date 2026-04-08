using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views.Home;

namespace FilipinoFolkloreApp.Views;

public partial class NarratorManagementPage : ContentPage
{
    private TaskCompletionSource<bool>? _alertTcs;

    private SoundService SoundService =>
        Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;

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
        NavigationPage.SetHasNavigationBar(this, false);
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
        await SoundService.PlayButtonClickAsync();

        if (sender is Grid grid && grid.BindingContext is NarratorCard card)
        {
            if (!card.IsLocked)
            {
                await Navigation.PushAsync(new NarratorDetailPage(card.Id));
            }
            else
            {
                if (CharacterHelper.CurrentStars >= card.Price)
                {
                    bool confirm = await ShowGameAlertAsync(
   $"I-unlock ang {card.Name} para sa {card.Price} coins?",
   true
);

                    if (confirm)
                    {
                        await UnlockNarrator(card);
                    }
                }
                else
                {
                    await ShowGameAlertAsync(
   $"{card.Name} ay naka-lock pa. Kailangan mo ng {card.Price} coins para i-unlock.",
   false
);
                }
            }
        }
    }

    private async Task UnlockNarrator(NarratorCard card)
    {
        try
        {
            CharacterHelper.CurrentStars -= card.Price;
            await App.Database.SetStarsAsync(CharacterHelper.CurrentStars);

            var firstStory = AlamatContent.Stories.FirstOrDefault(s => s.Id == "1_juan_tamad");
            if (firstStory != null)
            {
                switch (card.Id)
                {
                    case "eagle":
                        firstStory.NarratorEagleUnlocked = true;
                        break;
                    case "monkey":
                        firstStory.NarratorMonkeyUnlocked = true;
                        break;
                }

                await App.Database.UpdateStoryAsync(firstStory);
            }

            AlamatContent.UnlockedNarrators.Add(card.Id);

            LoadHUD();
            LoadNarrators();

            await ShowGameAlertAsync(
   $"Congratulations! Na-unlock mo ang {card.Name}!",
   false
);

            await Navigation.PushAsync(new NarratorDetailPage(card.Id));
        }
        catch (Exception ex)
        {
            await ShowGameAlertAsync(
                $"Error sa pag-unlock: {ex.Message}",
                false
            );
        }
    }

    private Task<bool> ShowGameAlertAsync(string message, bool showYesNo = false)
    {
        if (GameAlertOverlay.IsVisible && _alertTcs != null)
            return _alertTcs.Task;

        _alertTcs = new TaskCompletionSource<bool>();

        AlertMessageLabel.Text = message;

        AlertButtonsPanel.Children.Clear();

        if (showYesNo)
        {
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