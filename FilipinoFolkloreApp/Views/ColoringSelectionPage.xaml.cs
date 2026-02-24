using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views.Home;

namespace FilipinoFolkloreApp.Views;

public partial class ColoringSelectionPage : ContentPage
{
    private SoundService SoundService =>
        Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;

    private bool _isNavigating = false;

    private readonly List<ColoringTemplate> _coloringTemplates = new()
    {
        new ColoringTemplate
        {
            Id = "ski",
            Title = "Magkulay 1",
            ThumbnailPath = "drawingicon1.png",
            TemplatePath = "coloring/templates/drawing1.png"
        },
        new ColoringTemplate
        {
            Id = "magkulay2",
            Title = "Magkulay 2",
            ThumbnailPath = "drawingicon2.png",
            TemplatePath = "coloring/templates/drawing2.png"
        },
        new ColoringTemplate
        {
            Id = "magkulay3",
            Title = "Magkulay 3",
            ThumbnailPath = "drawingicon3.png",
            TemplatePath = "coloring/templates/drawing3.png"
        },
        new ColoringTemplate
        {
            Id = "magkulay4",
            Title = "Magkulay 4",
            ThumbnailPath = "drawingicon4.png",
            TemplatePath = "coloring/templates/drawing4.png"
        },
        new ColoringTemplate
        {
            Id = "magkulay5",
            Title = "Magkulay 5",
            ThumbnailPath = "drawingicon5.png",
            TemplatePath = "coloring/templates/drawing5.png"
        },
        new ColoringTemplate
        {
            Id = "magkulay6",
            Title = "Magkulay 6",
            ThumbnailPath = "drawingicon6.png",
            TemplatePath = "coloring/templates/drawing6.png"
        },
        new ColoringTemplate
        {
            Id = "magkulay7",
            Title = "Magkulay 7",
            ThumbnailPath = "drawingicon7.png",
            TemplatePath = "coloring/templates/drawing7.png"
        },
        new ColoringTemplate
        {
            Id = "magkulay8",
            Title = "Magkulay 8",
            ThumbnailPath = "drawingicon8.png",
            TemplatePath = "coloring/templates/drawing8.png"
        },
        new ColoringTemplate
        {
            Id = "magkulay9",
            Title = "Magkulay 9",
            ThumbnailPath = "drawingicon9.png",
            TemplatePath = "coloring/templates/drawing9.png"
        },
        new ColoringTemplate
        {
            Id = "magkulay10",
            Title = "Magkulay 10",
            ThumbnailPath = "drawingicon10.png",
            TemplatePath = "coloring/templates/drawing10.png"
        },
        new ColoringTemplate
        {
            Id = "magkulay11",
            Title = "Magkulay 11",
            ThumbnailPath = "drawingicon11.png",
            TemplatePath = "coloring/templates/drawing11.png"
        },
        new ColoringTemplate
        {
            Id = "magkulay12",
            Title = "Magkulay 12",
            ThumbnailPath = "drawingicon12.png",
            TemplatePath = "coloring/templates/drawing12.png"
        },
        new ColoringTemplate
        {
            Id = "magkulay13",
            Title = "Magkulay 13",
            ThumbnailPath = "drawingicon13.png",
            TemplatePath = "coloring/templates/drawing13.png"
        }
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

        _isNavigating = false;

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
        int columns = 5;
        int currentColumn = 0;
        Grid? currentRow = null;

        foreach (var template in _coloringTemplates)
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

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => OnColoringTemplateTapped(template);
        frame.GestureRecognizers.Add(tapGesture);

        return frame;
    }

    private async void OnColoringTemplateTapped(ColoringTemplate template)
    {
        if (_isNavigating)
        {
            System.Diagnostics.Debug.WriteLine("Double-tap prevented: Already navigating to ColoringPage.");
            return;
        }

        await SoundService.PlayButtonClickAsync();

        _isNavigating = true;

        try
        {
            await Navigation.PushAsync(new ColoringPage(template.TemplatePath));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnColoringTemplateTapped: {ex}");
            _isNavigating = false;
        }
    }

    private async void OnCollectionTapped(object? sender, EventArgs e)
    {
        if (_isNavigating)
        {
            System.Diagnostics.Debug.WriteLine("Double-tap prevented: Already navigating to ColoringCollectionPage.");
            return;
        }

        await SoundService.PlayButtonClickAsync();

        _isNavigating = true;

        try
        {
            await Navigation.PushAsync(new ColoringCollectionPage());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnCollectionTapped: {ex}");
            _isNavigating = false;
        }
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

    private class ColoringTemplate
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string ThumbnailPath { get; set; } = "";
        public string TemplatePath { get; set; } = "";
    }
}