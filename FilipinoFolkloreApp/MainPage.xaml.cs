using System;
using Microsoft.Maui.Controls;
using FilipinoFolkloreApp.Views;
using FilipinoFolkloreApp.Views.Home;
using FilipinoFolkloreApp.Models;
using System.Threading.Tasks;
using FilipinoFolkloreApp.Services;
using Microsoft.Maui.Layouts;

namespace FilipinoFolkloreApp
{
    public partial class MainPage : ContentPage
    {
        // Name length constraints
        private const int MIN_NAME_LENGTH = 2;
        private const int MAX_NAME_LENGTH = 20;

        // guard to avoid navigating twice
        private bool _navigated = false;

        // Tutorial state
        private int _tutorialStep = 0;
        private const string TUTORIAL_COMPLETED_KEY = "MainPageTutorialCompleted";

        // Tutorial steps configuration
        private readonly TutorialStep[] _tutorialSteps = new[]
        {
            new TutorialStep
            {
                Title = "Maligayang pagdating!",
                Message = "Ako si Tarsy, ang iyong gabay! Tulungan kitang magsimula sa paglalakbay sa mundo ng Filipino Folklore.",
                TargetElementName = null,
            },
            new TutorialStep
            {
                Title = "Ilagay ang Iyong Pangalan",
                Message = "I-click ang mga titik sa ibaba para magsimula ng paglagay ng iyong pangalan. Kailangan ng 2-20 titik.",
                TargetElementName = "OutputEntry",
            },
            new TutorialStep
            {
                Title = "Tanggalin ang Mali",
                Message = "Kung may mali, i-click ang erase button para burahin ang lahat ng titik.",
                TargetElementName = "EraseButton",
            },
            new TutorialStep
            {
                Title = "Ipagpatuloy",
                Message = "Kapag tapos ka na, i-click ang OK button para magpatuloy sa adventure!",
                TargetElementName = "EnterButton",
            }
        };

        public MainPage()
        {
            InitializeComponent();
            UpdateCharacterCounter();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

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
                await PositionArrowToElement(step.TargetElementName);
            }
            else
            {
                ArrowPointer.Opacity = 0;
                // Position speech bubble at default location (upper right of tarsier)
                PositionSpeechBubble(true);
            }

            // Highlight target element
            HighlightTargetElement(step.TargetElementName);
        }

        private async Task PositionArrowToElement(string elementName)
        {
            // 1. Find the target element
            VisualElement? targetElement = elementName switch
            {
                "OutputEntry" => OutputEntry,
                "EraseButton" => EraseButton,
                "EnterButton" => EnterButton,
                "KeyboardGrid" => KeyboardGrid,
                _ => null
            };

            if (targetElement == null)
            {
                ArrowPointer.Opacity = 0;
                return;
            }

            // Wait briefly for layout to settle
            await Task.Delay(100);

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
            PositionSpeechBubble(!arrowIsAtTopHalf);

            await AnimateArrowPointer();
        }

        private void PositionSpeechBubble(bool positionAtTop)
        {
            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            double screenHeight = displayInfo.Height / displayInfo.Density;

            // CHANGE 1: Use a fixed X position instead of centering
            // Tarsier ends at 180 (X=30 + Width=150). 
            // We set Bubble X to 160 to slightly overlap the tail with the Tarsier.
            double bubbleX = 160;

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
            OutputEntry.Opacity = 1;
            KeyboardGrid.Opacity = 1;

            if (string.IsNullOrEmpty(targetName))
                return;

            // Dim everything except target
            switch (targetName)
            {
                case "OutputEntry":
                    KeyboardGrid.Opacity = 0.3;
                    break;
                case "EraseButton":
                case "EnterButton":
                    OutputEntry.Opacity = 0.5;
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
            OutputEntry.Opacity = 1;
            KeyboardGrid.Opacity = 1;
        }

        

        private async void Letter_Clicked(object sender, EventArgs e)
        {
            if (sender is ImageButton btn && btn.CommandParameter is string letter)
            {
                if (letter == "enter")
                {
                    var name = (OutputEntry.Text ?? string.Empty).Trim();

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        await DisplayAlert("Kailangan ang Pangalan", "Pakilagay ang iyong pangalan.", "OK");
                        return;
                    }

                    if (name.Length < MIN_NAME_LENGTH)
                    {
                        await DisplayAlert("Masyadong Maikli", $"Ang pangalan ay dapat hindi bababa sa {MIN_NAME_LENGTH} titik.", "OK");
                        return;
                    }

                    if (name.Length > MAX_NAME_LENGTH)
                    {
                        await DisplayAlert("Masyadong Mahaba", $"Ang pangalan ay hindi dapat lumampas sa {MAX_NAME_LENGTH} titik.", "OK");
                        return;
                    }

                    try
                    {
                        await App.Database.SaveCharAsync(new Character { Id = 1, name = name, stars = 50 });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Save name failed: {ex}");
                        await DisplayAlert("Hindi Na-save", "Hindi naisave ang pangalan. Subukang muli.", "OK");
                        return;
                    }

                    if (!_navigated)
                    {
                        _navigated = true;
                        CharacterHelper.CurrentName = name;
                        CharacterHelper.CurrentStars = 50;

                        await Navigation.PushAsync(new AvatarSelectionPage(), true);
                        Navigation.RemovePage(this);
                    }
                }
                else if (letter == "erase")
                {
                    OutputEntry.Text = string.Empty;
                    UpdateCharacterCounter();
                }
                else
                {
                    string currentText = OutputEntry.Text ?? string.Empty;
                    if (currentText.Length < MAX_NAME_LENGTH)
                    {
                        OutputEntry.Text += letter.ToUpper();
                        UpdateCharacterCounter();
                    }
                    else
                    {
                        AnimateMaxLengthReached();
                    }
                }
            }
        }

        private void UpdateCharacterCounter()
        {
            int currentLength = OutputEntry.Text?.Length ?? 0;
            CharCounterLabel.Text = $"{currentLength}/{MAX_NAME_LENGTH}";

            if (currentLength >= MAX_NAME_LENGTH)
            {
                CharCounterLabel.TextColor = Color.FromArgb("#FF1493");
            }
            else if (currentLength >= MAX_NAME_LENGTH - 5)
            {
                CharCounterLabel.TextColor = Color.FromArgb("#FF69B4");
            }
            else
            {
                CharCounterLabel.TextColor = Color.FromArgb("#FFB6C1");
            }
        }

        private async void AnimateMaxLengthReached()
        {
            try
            {
                await CharCounterLabel.TranslateTo(-10, 0, 50);
                await CharCounterLabel.TranslateTo(10, 0, 50);
                await CharCounterLabel.TranslateTo(-10, 0, 50);
                await CharCounterLabel.TranslateTo(10, 0, 50);
                await CharCounterLabel.TranslateTo(0, 0, 50);
            }
            catch
            {
                // Animation cancelled or failed - ignore
            }
        }

        private class TutorialStep
        {
            public string Title { get; set; } = "";
            public string Message { get; set; } = "";
            public string? TargetElementName { get; set; }
        }
    }
}