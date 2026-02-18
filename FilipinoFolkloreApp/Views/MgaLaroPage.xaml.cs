using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using FilipinoFolkloreApp.Services;
using Microsoft.Maui.Layouts;

namespace FilipinoFolkloreApp.Views;

public partial class MgaLaroPage : ContentPage
{
    private SoundService SoundService =>
        Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;

    // Tutorial state
    private int _tutorialStep = 0;
    private const string TUTORIAL_COMPLETED_KEY = "MgaLaroPageTutorialCompleted7";

    // Tutorial steps configuration - FOCUSED ON GAMES
    private readonly TutorialStep[] _tutorialSteps = new[]
    {
        new TutorialStep
        {
            Title = "Maligayang Pagdating sa Mga Laro!",
            Message = "Dito makikita mo ang mga laro na pwede mong laruin para kumita ng stars!",
            
        },
        new TutorialStep
        {
            Title = "Bugtong",
            Message = "I-click ito para maglaro ng Bugtong! Sagutan ang mga riddles at manalo ng stars!",
            TargetElementName = "BugtongButton",
            OffsetX = +330
        },
        new TutorialStep
        {
            Title = "Magkulay",
            Message = "I-click ito para mag-coloring! Kulayan ang mga larawan at mag-enjoy!",
            TargetElementName = "MagkulayButton",
            OffsetX = +330
        },
        new TutorialStep
        {
            Title = "Narrator Management",
            Message = "I-click ang puno para tingnan ang iyong narrator collection at battery status!",
            TargetElementName = "NarratorTreeImage",
            OffsetX = +150
        }
    };

    public MgaLaroPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        LoadHUD();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LoadHUD();

        // Check if tutorial should be shown
        bool tutorialCompleted = Preferences.Get(TUTORIAL_COMPLETED_KEY, false);
        if (!tutorialCompleted)
        {
            await Task.Delay(500);
            await ShowTutorial();
        }
    }

    private async Task ShowTutorial()
    {
        _tutorialStep = 0;

        UpdateTutorialStep();
        TutorialOverlay.IsVisible = true;

        // Animate tarsier entrance
        await Task.WhenAll(
            TarsierImage.FadeTo(1, 400, Easing.CubicOut),
            TarsierImage.ScaleTo(1, 400, Easing.BounceOut)
        );

        // Animate speech bubble
        await Task.Delay(200);
        await Task.WhenAll(
            SpeechBubbleContainer.FadeTo(1, 300, Easing.CubicOut),
            SpeechBubbleContainer.ScaleTo(1, 300, Easing.BounceOut)
        );
    }

    private async void OnTutorialNextStep(object? sender, EventArgs e)
    {
        _tutorialStep++;

        if (_tutorialStep >= _tutorialSteps.Length)
        {
            await CompleteTutorial();
            return;
        }

        UpdateTutorialStep();
    }

    private async void UpdateTutorialStep()
    {
        var step = _tutorialSteps[_tutorialStep];

        TutorialTitleLabel.Text = step.Title;
        TutorialMessageLabel.Text = step.Message;
        TutorialProgressLabel.Text = $"{_tutorialStep + 1}/{_tutorialSteps.Length}";

        // Update arrow pointer to point at target element dynamically
        if (!string.IsNullOrEmpty(step.TargetElementName))
        {
            await PositionArrowToElement(step.TargetElementName, step.OffsetX);
        }
        else
        {
            ArrowPointer.Opacity = 0;
            PositionSpeechBubble(true, 0);
        }

        // Highlight target element
        HighlightTargetElement(step.TargetElementName);
    }

    private async Task PositionArrowToElement(string elementName, double offsetX = 0)
    {
        VisualElement? targetElement = elementName switch
        {
            "GamesContentArea" => GamesContentArea,
            "BugtongButton" => BugtongButton,
            "MagkulayButton" => MagkulayButton,
            "NarratorTreeImage" => NarratorTreeImage,
            _ => null
        };

        if (targetElement == null)
        {
            ArrowPointer.Opacity = 0;
            return;
        }

        await Task.Delay(150);

        var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
        double screenHeight = displayInfo.Height / displayInfo.Density;
        double safeZone = 60;

        Rect targetBounds = GetAbsolutePosition(targetElement);

        if (targetBounds == Rect.Zero) return;

        double arrowWidth = 50;
        double arrowHeight = 50;
        double padding = 10;

        double yAbove = targetBounds.Top - arrowHeight - padding;
        double yBelow = targetBounds.Bottom + padding;

        bool preferAbove = targetBounds.Center.Y > (screenHeight / 2);

        double finalArrowY;
        bool isArrowAbove;

        if (preferAbove)
        {
            if (yAbove >= safeZone)
            {
                finalArrowY = yAbove;
                isArrowAbove = true;
            }
            else
            {
                finalArrowY = yBelow;
                isArrowAbove = false;
            }
        }
        else
        {
            if (yBelow + arrowHeight <= screenHeight - safeZone)
            {
                finalArrowY = yBelow;
                isArrowAbove = false;
            }
            else
            {
                finalArrowY = yAbove;
                isArrowAbove = true;
            }
        }

        double arrowX = targetBounds.Center.X - (arrowWidth / 2);

        AbsoluteLayout.SetLayoutBounds(ArrowPointer, new Rect(arrowX, finalArrowY, arrowWidth, arrowHeight));
        AbsoluteLayout.SetLayoutFlags(ArrowPointer, AbsoluteLayoutFlags.None);

        ArrowPointer.Rotation = isArrowAbove ? 90 : -90;

        bool arrowIsAtTopHalf = finalArrowY < (screenHeight / 2);
        PositionSpeechBubble(!arrowIsAtTopHalf, offsetX);

        await AnimateArrowPointer();
    }

    private void PositionSpeechBubble(bool positionAtTop, double offsetX)
    {
        var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
        double screenHeight = displayInfo.Height / displayInfo.Density;

        double bubbleX = 160 + offsetX;
        double bubbleWidth = 350;
        double safePadding = 60;

        if (positionAtTop)
        {
            AbsoluteLayout.SetLayoutBounds(SpeechBubbleContainer, new Rect(bubbleX, safePadding, bubbleWidth, AbsoluteLayout.AutoSize));
        }
        else
        {
            AbsoluteLayout.SetLayoutBounds(SpeechBubbleContainer, new Rect(bubbleX, screenHeight - 200, bubbleWidth, AbsoluteLayout.AutoSize));
        }

        AbsoluteLayout.SetLayoutFlags(SpeechBubbleContainer, AbsoluteLayoutFlags.None);
    }

    private Rect GetAbsolutePosition(VisualElement element)
    {
        if (element == null) return Rect.Zero;

        // Add TranslationX and TranslationY to grab the true visual position
        double x = element.X + element.TranslationX;
        double y = element.Y + element.TranslationY;
        double width = element.Width;
        double height = element.Height;

        var current = element.Parent as VisualElement;

        while (current != null && !(current is Page))
        {
            // Add parent layouts AND parent translations
            x += current.X + current.TranslationX;
            y += current.Y + current.TranslationY;

            if (current is ScrollView scrollView)
            {
                x -= scrollView.ScrollX;
                y -= scrollView.ScrollY;
            }

            current = current.Parent as VisualElement;
        }

        return new Rect(x, y, width, height);
    }

    private async Task AnimateArrowPointer()
    {
        ArrowPointer.Opacity = 0;
        ArrowPointer.Scale = 0.8;

        await Task.Delay(100);

        await Task.WhenAll(
            ArrowPointer.FadeTo(1, 300, Easing.CubicOut),
            ArrowPointer.ScaleTo(1, 300, Easing.BounceOut)
        );

        _ = Task.Run(async () =>
        {
            try
            {
                while (ArrowPointer.Opacity > 0 && TutorialOverlay.IsVisible)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        if (ArrowPointer.Opacity > 0 && TutorialOverlay.IsVisible)
                        {
                            await ArrowPointer.ScaleTo(1.2, 500, Easing.CubicInOut);
                            if (ArrowPointer.Opacity > 0 && TutorialOverlay.IsVisible)
                            {
                                await ArrowPointer.ScaleTo(1.0, 500, Easing.CubicInOut);
                            }
                        }
                    });
                    await Task.Delay(100);
                }
            }
            catch { }
        });
    }

    private void HighlightTargetElement(string? targetName)
    {
        // Reset all highlights
        GamesContentArea.Opacity = 1;
        BugtongButton.Opacity = 1;
        MagkulayButton.Opacity = 1;
        NarratorTreeImage.Opacity = 1;

        if (string.IsNullOrEmpty(targetName))
            return;

        // Dim everything except target
        switch (targetName)
        {
            case "GamesContentArea":
                // Don't dim anything, just show the whole area
                break;
            case "BugtongButton":
                MagkulayButton.Opacity = 0.3;
                NarratorTreeImage.Opacity = 0.3;
                break;
            case "MagkulayButton":
                BugtongButton.Opacity = 0.3;
                NarratorTreeImage.Opacity = 0.3;
                break;
            case "NarratorTreeImage":
                BugtongButton.Opacity = 0.3;
                MagkulayButton.Opacity = 0.3;
                break;
        }
    }

    private async Task CompleteTutorial()
    {
        Preferences.Set(TUTORIAL_COMPLETED_KEY, true);

        await Task.WhenAll(
            ArrowPointer.FadeTo(0, 200),
            SpeechBubbleContainer.FadeTo(0, 300),
            TarsierImage.FadeTo(0, 300),
            TutorialOverlay.FadeTo(0, 400)
        );

        TutorialOverlay.IsVisible = false;

        // Reset opacities
        GamesContentArea.Opacity = 1;
        BugtongButton.Opacity = 1;
        MagkulayButton.Opacity = 1;
        NarratorTreeImage.Opacity = 1;
    }

    private void LoadHUD()
    {
        HudAvatar.Source = CharacterHelper.CurrentAvatar;
        PlayerNameLabel.Text = CharacterHelper.CurrentName;
        StarsLabel.Text = CharacterHelper.CurrentStars.ToString();
        NarratorImage.Source = AlamatContent.CurrentNarratorImage;
        NarratorBatteryImage.Source = AlamatContent.GetNarratorBatteryImage();
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

    private async void OnNarratorTreeTapped(object? sender, TappedEventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await Navigation.PushAsync(new NarratorManagementPage());
    }

    private async void OnBugtongTapped(object? sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await Navigation.PushAsync(new BugtongListPage());
    }

    private async void OnMagpintaTapped(object? sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        // Navigate to Coloring Selection page
        await Navigation.PushAsync(new ColoringSelectionPage());
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await Navigation.PopAsync();
    }

    private async void OnHomeTapped(object? sender, TappedEventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await NavigationHelper.NavigateToIndexPage(Navigation);
    }

    private class TutorialStep
    {
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string? TargetElementName { get; set; }
        public double OffsetX { get; set; } = 0;
    }
}