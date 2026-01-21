using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views;

public partial class ColoringCollectionPage : ContentPage
{
    private const int StarsPerImage = 15;
    private List<ColoredImageInfo> _coloredImages = new();

    public ColoringCollectionPage()
    {
        InitializeComponent();

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
            await DisplayAlert("Error", "Hindi ma-load ang mga larawan", "OK");
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

        // Thumbnail Image
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

        // Add tap gesture to view image
        var imageTapGesture = new TapGestureRecognizer();
        imageTapGesture.Tapped += (s, e) => OnImageTapped(imageInfo);
        thumbnailFrame.GestureRecognizers.Add(imageTapGesture);

        thumbnailFrame.Content = image;
        Grid.SetRow(thumbnailFrame, 0);
        grid.Children.Add(thumbnailFrame);

        // Sell Button
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
        // Show image preview modal
        PreviewImage.Source = ImageSource.FromFile(imageInfo.FilePath);
        ImagePreviewOverlay.IsVisible = true;
    }

    private void OnClosePreview(object? sender, EventArgs e)
    {
        ImagePreviewOverlay.IsVisible = false;
    }

    private async Task OnSellImageClicked(ColoredImageInfo imageInfo, Frame cardFrame)
    {
        bool confirm = await DisplayAlert(
            "Ibenta ang Larawan",
            $"Ibenta ang larawan para sa {StarsPerImage} stars?",
            "Oo",
            "Hindi"
        );

        if (!confirm) return;

        try
        {
            // Delete the image file
            if (File.Exists(imageInfo.FilePath))
            {
                File.Delete(imageInfo.FilePath);
            }

            // Add stars to database
            var character = await App.Database.AddStarsAsync(StarsPerImage);
            CharacterHelper.CurrentStars = character.stars;

            // Update UI
            StarsLabel.Text = character.stars.ToString();

            // Remove from list and UI
            _coloredImages.Remove(imageInfo);

            // Reload the grid
            LoadColoredImageGrid();

            // Show empty state if no more images
            if (_coloredImages.Count == 0)
            {
                ShowEmptyState();
            }

            await DisplayAlert("Tagumpay!", $"Nakakuha ka ng {StarsPerImage} stars!", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error selling image: {ex}");
            await DisplayAlert("Error", "Hindi ma-ibenta ang larawan", "OK");
        }
    }

    private void ShowEmptyState()
    {
        ColoredImagesContainer.Children.Clear();
        EmptyStateContainer.IsVisible = true;
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnHomeTapped(object? sender, TappedEventArgs e)
    {
        await NavigationHelper.NavigateToIndexPage(Navigation);
    }

    private class ColoredImageInfo
    {
        public string FilePath { get; set; } = "";
        public string FileName { get; set; } = "";
        public DateTime CreatedDate { get; set; }
    }
}