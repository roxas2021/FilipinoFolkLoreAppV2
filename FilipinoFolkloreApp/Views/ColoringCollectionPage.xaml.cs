using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views.Home;

namespace FilipinoFolkloreApp.Views;

public partial class ColoringCollectionPage : ContentPage
{
    private const int StarsPerImage = 15;
    private List<ColoredImageInfo> _coloredImages = new();
    private TaskCompletionSource<bool>? _alertTcs;

    private SoundService SoundService =>
        Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;

    public ColoringCollectionPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        LoadHUD();
        _ = LoadColoredImagesAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadHUD();
        _ = LoadColoredImagesAsync();
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

    private async Task LoadColoredImagesAsync()
    {
        try
        {
            var folderPath = Path.Combine(FileSystem.AppDataDirectory, "ColoredImages");

            if (!Directory.Exists(folderPath))
            {
                ShowEmptyState();
                return;
            }

            var imageFiles = Directory.GetFiles(folderPath, "*.png");

            if (imageFiles.Length == 0)
            {
                ShowEmptyState();
                return;
            }

            _coloredImages.Clear();
            foreach (var filePath in imageFiles.OrderByDescending(f => File.GetCreationTime(f)))
            {
                _coloredImages.Add(new ColoredImageInfo
                {
                    FilePath = filePath,
                    FileName = Path.GetFileName(filePath),
                    CreatedDate = File.GetCreationTime(filePath)
                });
            }

            LoadColoredImageGrid();
            EmptyStateContainer.IsVisible = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading colored images: {ex}");
            await ShowGameAlertAsync("Hindi ma-load ang mga larawan", false);
        }
    }

    private void LoadColoredImageGrid()
    {
        ColoredImagesContainer.Children.Clear();

        int columns = 5;
        int currentColumn = 0;
        Grid? currentRow = null;

        foreach (var imageInfo in _coloredImages)
        {
            if (currentColumn == 0)
            {
                currentRow = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    },
                    ColumnSpacing = 10,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                ColoredImagesContainer.Children.Add(currentRow);
            }

            var cardFrame = CreateImageCard(imageInfo);
            Grid.SetColumn(cardFrame, currentColumn);
            currentRow?.Children.Add(cardFrame);

            currentColumn++;
            if (currentColumn >= columns)
            {
                currentColumn = 0;
            }
        }
    }

    private Frame CreateImageCard(ColoredImageInfo imageInfo)
    {
        var frame = new Frame
        {
            BackgroundColor = Color.FromArgb("#F5DEB3"),
            BorderColor = Color.FromArgb("#8B4513"),
            CornerRadius = 12,
            Padding = 8,
            HasShadow = true
        };

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(120) },
                new RowDefinition { Height = GridLength.Auto }
            }
        };

        var thumbnailFrame = new Frame
        {
            BackgroundColor = Colors.White,
            CornerRadius = 8,
            Padding = 4,
            HasShadow = false,
            BorderColor = Color.FromArgb("#D2691E")
        };

        var image = new Image
        {
            Source = ImageSource.FromFile(imageInfo.FilePath),
            Aspect = Aspect.AspectFit,
            HeightRequest = 100
        };

        var imageTapGesture = new TapGestureRecognizer();
        imageTapGesture.Tapped += (s, e) => OnImageTapped(imageInfo);
        thumbnailFrame.GestureRecognizers.Add(imageTapGesture);

        thumbnailFrame.Content = image;
        Grid.SetRow(thumbnailFrame, 0);
        grid.Children.Add(thumbnailFrame);

        var sellButton = new Button
        {
            Text = $"+ {StarsPerImage}",
            BackgroundColor = Color.FromArgb("#FFD700"),
            TextColor = Color.FromArgb("#8B4513"),
            FontAttributes = FontAttributes.Bold,
            FontSize = 14,
            CornerRadius = 8,
            Padding = new Thickness(10, 5),
            Margin = new Thickness(0, 6, 0, 0)
        };

        sellButton.Clicked += async (s, e) => await OnSellImageClicked(imageInfo, frame);

        Grid.SetRow(sellButton, 1);
        grid.Children.Add(sellButton);

        frame.Content = grid;

        return frame;
    }

    private void OnImageTapped(ColoredImageInfo imageInfo)
    {
        PreviewImage.Source = ImageSource.FromFile(imageInfo.FilePath);
        ImagePreviewOverlay.IsVisible = true;
    }

    private async void OnClosePreview(object? sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        ImagePreviewOverlay.IsVisible = false;
    }

    private async Task OnSellImageClicked(ColoredImageInfo imageInfo, Frame cardFrame)
    {
        bool confirm = await ShowGameAlertAsync(
            $"Ibenta ang larawan para sa {StarsPerImage} stars?",
            true
        );

        if (!confirm) return;

        try
        {
            if (File.Exists(imageInfo.FilePath))
            {
                File.Delete(imageInfo.FilePath);
            }

            var character = await App.Database.AddStarsAsync(StarsPerImage);
            CharacterHelper.CurrentStars = character.stars;

            StarsLabel.Text = character.stars.ToString();

            _coloredImages.Remove(imageInfo);

            LoadColoredImageGrid();

            if (_coloredImages.Count == 0)
            {
                ShowEmptyState();
            }

            await ShowGameAlertAsync($"Nakakuha ka ng {StarsPerImage} stars!", false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error selling image: {ex}");
            await ShowGameAlertAsync("Hindi ma-ibenta ang larawan", false);
        }
    }

    private void ShowEmptyState()
    {
        ColoredImagesContainer.Children.Clear();
        EmptyStateContainer.IsVisible = true;
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
            if (page is ColoringSelectionPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is ColoringCollectionPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is ColoringPage)
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

    private class ColoredImageInfo
    {
        public string FilePath { get; set; } = "";
        public string FileName { get; set; } = "";
        public DateTime CreatedDate { get; set; }
    }
}