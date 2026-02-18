using FilipinoFolkloreApp.Models;
using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views.Home;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Layouts;
using System;

namespace FilipinoFolkloreApp.Views
{
    public partial class AvatarSelectionPage : ContentPage
    {
        // Tutorial state
        private int _tutorialStep = 0;
        private const string TUTORIAL_COMPLETED_KEY = "AvatarSelectionTutorialCompleted";

        // Tutorial steps configuration
        private readonly TutorialStep[] _tutorialSteps = new[]
        {
            new TutorialStep
            {
                Title = "Pumili ng Iyong Avatar!",
                Message = "Maligayang pagdating! Pumili ng avatar na magiging iyong kaibigan sa adventure.",
                TargetElementName = null,
            },
            new TutorialStep
            {
                Title = "Tingnan ang mga Pagpipilian",
                Message = "May apat na magagandang avatar na pwede mong piliin. Bawat isa ay may sariling istilo!",
                TargetElementName = "AvatarGrid",
            },
            new TutorialStep
            {
                Title = "I-tap para Piliin",
                Message = "I-click ang avatar na gusto mo. Makakakuha ka ng 50 Pilon Stars para magsimula!",
                TargetElementName = "Avatar3",
            }
        };

        public ObservableCollection<Avatar> Avatars { get; } = new()
        {
            new Avatar("avatarcustomization/avatar1/avatar1.png"),
            new Avatar("avatarcustomization/avatar2/avatar2.png"),
            new Avatar("avatarcustomization/avatar3/avatar3.png"),
            new Avatar("avatarcustomization/avatar4/avatar4.png"),
        };

        public AvatarSelectionPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            AvatarGrid.ItemsSource = Avatars;
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
            VisualElement? targetElement = null;

            if (elementName == "AvatarGrid")
            {
                targetElement = AvatarGrid;
            }
            else if (elementName == "Avatar3")
            {
                // Get the 3rd avatar item (index 2)
                targetElement = await GetAvatarItemAtIndex(2);
                if (targetElement == null)
                {
                    // Fallback to the whole grid if we can't get the specific item
                    targetElement = AvatarGrid;
                }
            }

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
            PositionSpeechBubble(!arrowIsAtTopHalf);

            await AnimateArrowPointer();
        }

        private async Task<VisualElement?> GetAvatarItemAtIndex(int index)
        {
            // Try to get the visual element for a specific item in the CollectionView
            try
            {
                // Give the CollectionView time to render items
                await Task.Delay(200);

                // Search through the visual tree to find the avatar image at the specified index
                var items = FindVisualChildren<Image>(AvatarGrid);
                var itemList = items.ToList();

                if (itemList.Count > index)
                {
                    return itemList[index];
                }
            }
            catch
            {
                // If we can't get the item, return null
            }

            return null;
        }

        private IEnumerable<T> FindVisualChildren<T>(Element element) where T : Element
        {
            if (element == null)
                yield break;

            // Check if current element is of type T
            if (element is T t)
                yield return t;

            // Recursively search children
            foreach (var child in GetLogicalChildren(element))
            {
                foreach (var descendant in FindVisualChildren<T>(child))
                {
                    yield return descendant;
                }
            }
        }

        private IEnumerable<Element> GetLogicalChildren(Element element)
        {
            if (element is Layout layout)
            {
                foreach (var child in layout.Children.OfType<Element>())
                    yield return child;
            }
            else if (element is ContentView contentView && contentView.Content != null)
            {
                yield return contentView.Content;
            }
            else if (element is ScrollView scrollView && scrollView.Content != null)
            {
                yield return scrollView.Content;
            }
            else if (element is Frame frame && frame.Content != null)
            {
                yield return frame.Content;
            }
            else if (element is Border border && border.Content != null)
            {
                yield return border.Content;
            }
        }

        private void PositionSpeechBubble(bool positionAtTop)
        {
            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            double screenHeight = displayInfo.Height / displayInfo.Density;

            // Use a fixed X position instead of centering
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
            AvatarGrid.Opacity = 1;

            if (string.IsNullOrEmpty(targetName))
                return;

            // Currently only one target element, but structure allows for expansion
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
            AvatarGrid.Opacity = 1;
        }

        async void AvatarGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is not Avatar selected)
            {
                AvatarGrid.SelectedItem = null;
                return;
            }

            // Animation
            PointsOverlay.Scale = 0.9;
            PointsOverlay.Opacity = 0;
            PointsOverlay.IsVisible = true;

            await Task.WhenAll(
                PointsOverlay.FadeTo(1, 350),
                PointsOverlay.ScaleTo(1.05, 350, Easing.CubicOut)
            );

            await Task.Delay(2500);

            // Update character points
            var existingChar = await App.Database.GetCharAsync();
            if (existingChar != null)
            {
                existingChar.points = 50;
                await App.Database.UpdateCharAsync(existingChar);
            }

            // Save avatar set
            var fileName = Path.GetFileNameWithoutExtension(selected.ImageSource ?? "");
            var avatarId = string.IsNullOrWhiteSpace(fileName) ? "avatar1" : fileName;

            var set = new AvatarCostumeSet
            {
                avatarid = avatarId,
                avatarblueunlocked = false,
                avatarblueredunlocked = false,
                avatargreenunlocked = false,
                avatarpinkunlocked = false,
                avatarredunlocked = false,
            };

            await App.Database.SaveAvatarSetAsync(set);
            AvatarCustomizationHelper.SelectedAvatarSetId = set.avatarid;
            CharacterHelper.CurrentAvatar = AvatarCustomizationHelper.GetFirstCostumePathOrDefault(set.avatarid);
            await App.Database.UpdateCurrentAvatarAsync(CharacterHelper.CurrentAvatar);

            // Navigate to IndexPage
            await Navigation.PushAsync(new IndexPage());
            Navigation.RemovePage(this);

            AvatarGrid.SelectedItem = null;
        }

        private class TutorialStep
        {
            public string Title { get; set; } = "";
            public string Message { get; set; } = "";
            public string? TargetElementName { get; set; }
        }
    }

    public class Avatar
    {
        public string ImageSource { get; }
        public Avatar(string imageSource) => ImageSource = imageSource;
    }
}