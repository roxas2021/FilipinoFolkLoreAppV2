using FilipinoFolkloreApp.Services;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilipinoFolkloreApp.Views.Home;

public partial class IndexPage : ContentPage
{
    private HeartService HeartService =>
        Application.Current!.Handler!.MauiContext!.Services.GetService<HeartService>()!;
    
    private SoundService SoundService =>
        Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;

    // Tutorial state
    private int _tutorialStep = 0;
    private const string TUTORIAL_COMPLETED_KEY = "IndexPageTutorialCompleted";

    // Tutorial steps configuration
    private readonly TutorialStep[] _tutorialSteps = new[]
    {
        new TutorialStep
        {
            Title = "Maligayang Pagdating sa Home!",
            Message = "Ito ang iyong Home Page! Dito mo makikita ang lahat ng mahalagang features.",
            TargetElementName = null,
        },
        new TutorialStep
        {
            Title = "Iyong Avatar",
            Message = "I-click ang iyong avatar para mag-customize ng costume at hitsura!",
            TargetElementName = "HudAvatar",
        },
        new TutorialStep
        {
            Title = "Pilon Stars",
            Message = "Dito mo makikita ang iyong stars na kikitain sa bawat adventure!",
            TargetElementName = "StarsLabel",
        },
        new TutorialStep
        {
            Title = "Mga Puso (Lives)",
            Message = "Ito ang iyong mga lives. Kailangan mo nito para maglaro ng mga quiz!",
            TargetElementName = "HeartsPanel",
        },
        new TutorialStep
        {
            Title = "Medalya",
            Message = "I-click ito para tingnan ang mga medalyang nakuha mo!",
            TargetElementName = "MedalButton",
        },
        new TutorialStep
        {
            Title = "Settings",
            Message = "I-click ito para i-adjust ang volume ng music at narrator!",
            TargetElementName = "SettingsButton",
        },
        new TutorialStep
        {
            Title = "Alamat",
            Message = "I-click ito para magbasa ng mga Alamat - mga kwentong nagpapaliwanag kung paano nabuo ang mga bagay!",
            TargetElementName = "AlamatButton",
            OffsetX = 150 // Shift speech bubble to the right for better visibility
        },
        new TutorialStep
        {
            Title = "Epiko",
            Message = "I-click ito para magbasa ng mga Epiko - mga kwento ng mga bayani at kanilang mga adventure!",
            TargetElementName = "EpikoButton",
            OffsetX = 300
        },
        new TutorialStep
        {
            Title = "Pabula",
            Message = "I-click ito para magbasa ng mga Pabula - mga kwentong may aral na may mga hayop bilang tauhan!",
            TargetElementName = "PabulaButton",
            OffsetX = -100
        },
        new TutorialStep
        {
            Title = "Mga Laro",
            Message = "I-click ito para maglaro ng Bugtong at Coloring activities!",
            TargetElementName = "MgaLaroButton",
        }
    };
    
    public IndexPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        loadhud();
        var data = App.Database.GetCharAsync();
        LoadSettings();
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        AlamatContent.Hearts = HeartService.GetHearts();
        loadhud();
        LoadSettings();
        
        // Resume background music based on settings
        if (Application.Current is App app)
        {
            app.ResumeBackgroundMusic();
        }

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
            await PositionArrowToElement(step.TargetElementName,step.OffsetX);
        }
        else
        {
            ArrowPointer.Opacity = 0;
            // Position speech bubble at default location (upper right of tarsier)
            PositionSpeechBubble(true,0);
        }

        // Highlight target element
        HighlightTargetElement(step.TargetElementName);
    }

    private async Task PositionArrowToElement(string elementName, double offsetX = 0)
    {
        // 1. Find the target element
        VisualElement? targetElement = elementName switch
        {
            "HudAvatar" => HudAvatar,
            "StarsLabel" => StarsLabel.Parent as VisualElement,
            "HeartsPanel" => HeartsPanel,
            "MedalButton" => MedalButton,
            "SettingsButton" => SettingsButton,
            "AlamatButton" => AlamatButton,
            "EpikoButton" => EpikoButton,
            "PabulaButton" => PabulaButton,
            "MgaLaroButton" => MgaLaroButton,
            _ => null
        };

        if (targetElement == null)
        {
            ArrowPointer.Opacity = 0;
            return;
        }

        // Wait briefly for layout to settle
        await Task.Delay(150);

        // 2. Get screen info and target bounds
        var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
        double screenHeight = displayInfo.Height / displayInfo.Density;
        double safeZone = 60; // Padding from edges (Safe Zone)

        Rect targetBounds = GetAbsolutePosition(targetElement);

        if (targetBounds == Rect.Zero) return;

        // Arrow dimensions
        double arrowWidth = 50;
        double arrowHeight = 50;
        double padding = 10; // Space between arrow and element

        // 3. Calculate Potential Positions

        // Position A: Above the element (Pointing Down)
        double yAbove = targetBounds.Top - arrowHeight - padding;

        // Position B: Below the element (Pointing Up)
        double yBelow = targetBounds.Bottom + padding;

        // 4. Determine Best Position
        // Default preference based on screen half
        bool preferAbove = targetBounds.Center.Y > (screenHeight / 2);

        double finalArrowY;
        bool isArrowAbove;

        if (preferAbove)
        {
            // We want to be Above. Check if we fit in the Top Safe Zone.
            if (yAbove >= safeZone)
            {
                finalArrowY = yAbove;
                isArrowAbove = true;
            }
            else
            {
                // Overflowed Top! Flip to Below.
                finalArrowY = yBelow;
                isArrowAbove = false;
            }
        }
        else
        {
            // We want to be Below. Check if we fit in the Bottom Safe Zone.
            if (yBelow + arrowHeight <= screenHeight - safeZone)
            {
                finalArrowY = yBelow;
                isArrowAbove = false;
            }
            else
            {
                // Overflowed Bottom! Flip to Above.
                finalArrowY = yAbove;
                isArrowAbove = true;
            }
        }

        // 5. Apply Position and Rotation
        // Center horizontally
        double arrowX = targetBounds.Center.X - (arrowWidth / 2);

        AbsoluteLayout.SetLayoutBounds(ArrowPointer, new Rect(arrowX, finalArrowY, arrowWidth, arrowHeight));
        AbsoluteLayout.SetLayoutFlags(ArrowPointer, AbsoluteLayoutFlags.None);

        // Rotation Logic for Right-Pointing Arrow Image:
        // If Arrow is Above -> Needs to point DOWN -> Rotate 90 deg
        // If Arrow is Below -> Needs to point UP   -> Rotate -90 deg
        ArrowPointer.Rotation = isArrowAbove ? 90 : -90;

        // 6. Sync Speech Bubble
        // If Arrow is in top half (y < screenHeight/2), put Bubble at Bottom
        // If Arrow is in bottom half, put Bubble at Top
        bool arrowIsAtTopHalf = finalArrowY < (screenHeight / 2);
        PositionSpeechBubble(!arrowIsAtTopHalf, offsetX);

        await AnimateArrowPointer();
    }

    private void PositionSpeechBubble(bool positionAtTop, double offsetX)
    {
        var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
        double screenHeight = displayInfo.Height / displayInfo.Density;

        // Use a fixed X position instead of centering
        // Tarsier ends at 180 (X=30 + Width=150). 
        // We set Bubble X to 160 to slightly overlap the tail with the Tarsier.
        double bubbleX = 160;
        bubbleX += offsetX; // Apply any additional offset from the tutorial step
        double bubbleWidth = 350;
        double safePadding = 60; // Padding from top/bottom screen edges

        if (positionAtTop)
        {
            // Position at Top
            AbsoluteLayout.SetLayoutBounds(SpeechBubbleContainer, new Rect(bubbleX, safePadding, bubbleWidth, AbsoluteLayout.AutoSize));
        }
        else
        {
            // Position at Bottom
            // Using a fixed offset from bottom (e.g., 200) to ensure it doesn't cover keyboard/nav bar
            AbsoluteLayout.SetLayoutBounds(SpeechBubbleContainer, new Rect(bubbleX, screenHeight - 200, bubbleWidth, AbsoluteLayout.AutoSize));
        }

        AbsoluteLayout.SetLayoutFlags(SpeechBubbleContainer, AbsoluteLayoutFlags.None);
    }

    private Rect GetAbsolutePosition(VisualElement element)
    {
        if (element == null) return Rect.Zero;

        double x = element.X;
        double y = element.Y;
        double width = element.Width;
        double height = element.Height;

        var current = element.Parent as VisualElement;

        while (current != null && !(current is Page))
        {
            x += current.X;
            y += current.Y;

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

        // Bounce animation loop
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
            catch
            {
                // Animation cancelled - ignore
            }
        });
    }

    private void HighlightTargetElement(string? targetName)
    {
        // Reset all highlights
        HudAvatar.Opacity = 1;
        StarsLabel.Opacity = 1;
        HeartsPanel.Opacity = 1;
        AlamatButton.Opacity = 1;
        EpikoButton.Opacity = 1;
        PabulaButton.Opacity = 1;
        MgaLaroButton.Opacity = 1;

        if (string.IsNullOrEmpty(targetName))
            return;

        // Dim everything except target
        switch (targetName)
        {
            case "HudAvatar":
                StarsLabel.Opacity = 0.3;
                HeartsPanel.Opacity = 0.3;
                AlamatButton.Opacity = 0.3;
                EpikoButton.Opacity = 0.3;
                PabulaButton.Opacity = 0.3;
                MgaLaroButton.Opacity = 0.3;
                break;
            case "StarsLabel":
                HudAvatar.Opacity = 0.5;
                HeartsPanel.Opacity = 0.3;
                AlamatButton.Opacity = 0.3;
                EpikoButton.Opacity = 0.3;
                PabulaButton.Opacity = 0.3;
                MgaLaroButton.Opacity = 0.3;
                break;
            case "HeartsPanel":
                HudAvatar.Opacity = 0.5;
                StarsLabel.Opacity = 0.3;
                AlamatButton.Opacity = 0.3;
                EpikoButton.Opacity = 0.3;
                PabulaButton.Opacity = 0.3;
                MgaLaroButton.Opacity = 0.3;
                break;
            case "AlamatButton":
                HudAvatar.Opacity = 0.5;
                StarsLabel.Opacity = 0.3;
                HeartsPanel.Opacity = 0.3;
                EpikoButton.Opacity = 0.3;
                PabulaButton.Opacity = 0.3;
                MgaLaroButton.Opacity = 0.3;
                break;
            case "EpikoButton":
                HudAvatar.Opacity = 0.5;
                StarsLabel.Opacity = 0.3;
                HeartsPanel.Opacity = 0.3;
                AlamatButton.Opacity = 0.3;
                PabulaButton.Opacity = 0.3;
                MgaLaroButton.Opacity = 0.3;
                break;
            case "PabulaButton":
                HudAvatar.Opacity = 0.5;
                StarsLabel.Opacity = 0.3;
                HeartsPanel.Opacity = 0.3;
                AlamatButton.Opacity = 0.3;
                EpikoButton.Opacity = 0.3;
                MgaLaroButton.Opacity = 0.3;
                break;
            case "MgaLaroButton":
                HudAvatar.Opacity = 0.5;
                StarsLabel.Opacity = 0.3;
                HeartsPanel.Opacity = 0.3;
                AlamatButton.Opacity = 0.3;
                EpikoButton.Opacity = 0.3;
                PabulaButton.Opacity = 0.3;
                break;
        }
    }

    private async Task CompleteTutorial()
    {
        // Save that tutorial is completed
        Preferences.Set(TUTORIAL_COMPLETED_KEY, true);

        // Animate out
        await Task.WhenAll(
            ArrowPointer.FadeTo(0, 200),
            SpeechBubbleContainer.FadeTo(0, 300),
            TarsierImage.FadeTo(0, 300),
            TutorialOverlay.FadeTo(0, 400)
        );

        TutorialOverlay.IsVisible = false;

        // Reset opacities
        HudAvatar.Opacity = 1;
        StarsLabel.Opacity = 1;
        HeartsPanel.Opacity = 1;
        AlamatButton.Opacity = 1;
        EpikoButton.Opacity = 1;
        PabulaButton.Opacity = 1;
        MgaLaroButton.Opacity = 1;
    }
    
    void loadhud()
    {
        HudAvatar.Source = CharacterHelper.CurrentAvatar;
        PlayerNameLabel.Text = CharacterHelper.CurrentName;
        StarsLabel.Text = CharacterHelper.CurrentStars.ToString();

        RefreshHearts();
    }
    
    void RefreshHearts()
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
    
    private async void OnAvatarTapped(object sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await Navigation.PushAsync(new CharacterCostume());
    }
    
    private async void OnAlamatClicked(object sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await Navigation.PushAsync(new AlamatPage("alamat"));
    }
    
    private async void OnEpikoClicked(object sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await Navigation.PushAsync(new AlamatPage("epiko"));
    }
    
    private async void OnPabulaClicked(object sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await Navigation.PushAsync(new AlamatPage("pabula"));
    }
    
    private async void OnMedalyaClicked(object sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await Navigation.PushAsync(new MedalPage());
    }
    
    private async void OnMgaLaroClicked(object sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await Navigation.PushAsync(new MgaLaroPage());
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        SettingsModalOverlay.IsVisible = true;
    }
    
    private async void OnCloseSettingsModal(object sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        SettingsModalOverlay.IsVisible = false;
    }
    
    private void OnMusicToggled(object sender, ToggledEventArgs e)
    {
        if (Application.Current is App app)
        {
            app.UpdateBackgroundMusic(e.Value);
        }
    }
    
    private void OnBackgroundVolumeChanged(object sender, ValueChangedEventArgs e)
    {
        int volumePercent = (int)e.NewValue;
        BackgroundVolumeLabel.Text = $"{volumePercent}%";

        double volumeValue = e.NewValue / 100.0;
        Preferences.Set("BackgroundMusicVolume", volumeValue);

        if (Application.Current is App app)
        {
            app.SetBackgroundMusicVolume(volumeValue);
        }
    }
    
    private async void OnKreditsClicked(object sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        
        // Close the settings modal first
        SettingsModalOverlay.IsVisible = false;

        // Navigate to Credits page
        await Navigation.PushAsync(new CreditsPage());
    }
    
    private void OnVolumeChanged(object sender, ValueChangedEventArgs e)
    {
        // Update the volume label
        int volumePercent = (int)e.NewValue;
        BackgroundVolumeLabel.Text = $"{volumePercent}%";
        
        // Save to preferences (0.0 to 1.0 range)
        double volumeValue = e.NewValue / 100.0;
        Preferences.Set("NarratorVolume", volumeValue);
        
        // Store in AlamatContent for easy access
        AlamatContent.NarratorVolume = volumeValue;
    }
    
    private void OnNarratorVolumeChanged(object sender, ValueChangedEventArgs e)
    {
        // Update the volume label
        int volumePercent = (int)e.NewValue;
        NarratorVolumeLabel.Text = $"{volumePercent}%";

        // Save to preferences (0.0 to 1.0 range)
        double volumeValue = e.NewValue / 100.0;
        Preferences.Set("NarratorVolume", volumeValue);

        // Store in AlamatContent for easy access
        AlamatContent.NarratorVolume = volumeValue;
    }
    
    private void LoadSettings()
    {
        double savedBackgroundVolume = Preferences.Get("BackgroundMusicVolume", 0.3);
        BackgroundVolumeSlider.ValueChanged -= OnBackgroundVolumeChanged;
        BackgroundVolumeSlider.Value = savedBackgroundVolume * 100;
        BackgroundVolumeLabel.Text = $"{(int)(savedBackgroundVolume * 100)}%";
        BackgroundVolumeSlider.ValueChanged += OnBackgroundVolumeChanged;

        double savedNarratorVolume = Preferences.Get("NarratorVolume", 1.0);
        AlamatContent.NarratorVolume = savedNarratorVolume;

        NarratorVolumeSlider.ValueChanged -= OnNarratorVolumeChanged;
        NarratorVolumeSlider.Value = savedNarratorVolume * 100;
        NarratorVolumeLabel.Text = $"{(int)(savedNarratorVolume * 100)}%";
        NarratorVolumeSlider.ValueChanged += OnNarratorVolumeChanged;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }

    private class TutorialStep
    {
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string? TargetElementName { get; set; }
        // Add this line:
        public double OffsetX { get; set; } = 0;
    }
}