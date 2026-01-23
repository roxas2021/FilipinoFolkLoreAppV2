using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views.Home;

namespace FilipinoFolkloreApp.Views;

public partial class ColoringSelectionPage : ContentPage
{
    // Hardcoded list of coloring templates
    private readonly List<ColoringTemplate> _coloringTemplates = new()
    {
        new ColoringTemplate
        {
            Id = "ski",
            Title = "Ski Template",
            ThumbnailPath = "coloringthumbnail1.png",
            TemplatePath = "coloring/templates/coloringtemplate1.png"
        },
        new ColoringTemplate
        {
            Id = "ski2",
            Title = "Ski Template2",
            ThumbnailPath = "coloringthumbnail1.png",
            TemplatePath = "coloring/templates/coloringtemplate1.png"
        },
        new ColoringTemplate
        {
            Id = "ski3",
            Title = "Ski Template3",
            ThumbnailPath = "coloringthumbnail1.png",
            TemplatePath = "coloring/templates/coloringtemplate1.png"
        },
    };

    public ColoringSelectionPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        LoadHUD();
        LoadColoringTemplates();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadHUD();
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

    private void LoadColoringTemplates()
    {
        // Create grid layout with 5 columns
        int columns = 5;
        int currentColumn = 0;
        Grid? currentRow = null;

        foreach (var template in _coloringTemplates)
        {
            if (currentColumn == 0)
            {
                // Create new row with 5 equal columns
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
                ColoringImagesContainer.Children.Add(currentRow);
            }

            var cardFrame = CreateColoringCard(template);
            Grid.SetColumn(cardFrame, currentColumn);
            currentRow?.Children.Add(cardFrame);

            currentColumn++;
            if (currentColumn >= columns)
            {
                currentColumn = 0;
            }
        }
    }

    private Frame CreateColoringCard(ColoringTemplate template)
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
            Source = template.ThumbnailPath,
            Aspect = Aspect.AspectFit,
            HeightRequest = 100
        };

        thumbnailFrame.Content = image;
        Grid.SetRow(thumbnailFrame, 0);
        grid.Children.Add(thumbnailFrame);

        // Title
        var titleLabel = new Label
        {
            Text = template.Title,
            TextColor = Color.FromArgb("#8B4513"),
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 6, 0, 0),
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 2
        };

        Grid.SetRow(titleLabel, 1);
        grid.Children.Add(titleLabel);

        frame.Content = grid;

        // Add tap gesture
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => OnColoringTemplateTapped(template);
        frame.GestureRecognizers.Add(tapGesture);

        return frame;
    }

    private async void OnColoringTemplateTapped(ColoringTemplate template)
    {
        await Navigation.PushAsync(new ColoringPage(template.TemplatePath));
    }

    private async void OnCollectionTapped(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new ColoringCollectionPage());
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnHomeTapped(object? sender, TappedEventArgs e)
    {
        var pages = Navigation.NavigationStack.ToList();
        foreach (var page in pages)
        {
            if (page is ColoringSelectionPage)
            {
                // Remove RewardPage from the stack
                Navigation.RemovePage(page);
            }
            if (page is ColoringCollectionPage)
            {
                // Remove RewardPage from the stack
                Navigation.RemovePage(page);
            }
            if (page is ColoringPage)
            {
                // Remove RewardPage from the stack
                Navigation.RemovePage(page);
            }
            if (page is MgaLaroPage)
            {
                // Remove RewardPage from the stack
                Navigation.RemovePage(page);
            }
        }

        await Navigation.PushAsync(new IndexPage());
    }

    private class ColoringTemplate
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string ThumbnailPath { get; set; } = "";
        public string TemplatePath { get; set; } = "";
    }
}