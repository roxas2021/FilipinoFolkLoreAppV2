using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilipinoFolkloreApp.Services;
using Microsoft.Maui.Layouts;

namespace FilipinoFolkloreApp.Views
{
    public partial class CharacterCostume : ContentPage
    {
        private int PilonStarNicholAmountValue = 300;
        private List<bool> purchasedCostumes;
        private List<int> costumePrices;
        private int selectedCostumeId = 0;

        TaskCompletionSource<bool> _alertTcs;
        private AvatarCustomizationHelper.AvatarSet currentAvatarSet = AvatarCustomizationHelper.CurrentAvatarSet(AvatarCustomizationHelper.SelectedAvatarSetId);

        public string CurrentUserName { get; set; } = "Nichol";

        public List<TapisItem> TapisItems { get; set; }

        private SoundService SoundService =>
            Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;

        // Tutorial state
        private int _tutorialStep = 0;
        private const string TUTORIAL_COMPLETED_KEY = "CharacterCostumeTutorialCompleted2";

        // Tutorial steps configuration
        private readonly TutorialStep[] _tutorialSteps = new[]
        {
            new TutorialStep
            {
                Title = "Pag-customize ng Avatar!",
                Message = "Maligayang pagdating sa Avatar Customization! Dito mo pwedeng palitan ang damit ng iyong character.",
                TargetElementName = null,
                OffsetX = 0
            },
            new TutorialStep
            {
                Title = "Iyong Character",
                Message = "Dito mo makikita ang iyong current avatar. Mag-preview dito ng mga costumes na pipiliin mo!",
                TargetElementName = "CharacterImage",
                OffsetX = +200
            },
            new TutorialStep
            {
                Title = "Iyong Pilon Stars",
                Message = "Dito mo makikita kung gaano karaming stars mayroon ka para bumili ng mga bagong tapis!",
                TargetElementName = "PilonStarNicholLabel",
                OffsetX = 50
            },
            new TutorialStep
            {
                Title = "Mga Tapis (Costumes)",
                Message = "Pumili ng tapis mula dito! May presyo bawat isa. Pag nabili mo na, pwede mo nang gamitin!",
                TargetElementName = "TapisCollectionView",
                OffsetX = -100
            },
            new TutorialStep
            {
                Title = "Bilhin o Piliin",
                Message = "I-click ang button na ito para bilhin ang bagong tapis o piliin ang nabili mo na!",
                TargetElementName = "BuyButton",
                OffsetX = +200
            },
            new TutorialStep
            {
                Title = "Bumalik sa Home",
                Message = "I-click ito para bumalik sa Home page kapag tapos ka na mag-customize!",
                TargetElementName = "HomeButton",
                OffsetX = -50
            }
        };

        public CharacterCostume()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            CurrentUserName = CharacterHelper.CurrentName;
            PilonStarNicholAmountValue = CharacterHelper.CurrentStars;
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            CharacterImage.Source = CharacterHelper.CurrentAvatar;
            costumePrices = new List<int> { 100, 150, 180, 200, 250 };

            await LoadTapisItemsAsync();
            PilonStarNicholLabel.Text = CharacterHelper.CurrentStars.ToString();

            AlertMessageLabel.Text = $"You don't have enough {CurrentUserName}!";

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
                // Position speech bubble at default location (upper right of tarsier)
                PositionSpeechBubble(true, 0);
            }

            // Highlight target element
            HighlightTargetElement(step.TargetElementName);
        }

        private async Task PositionArrowToElement(string elementName, double offsetX = 0)
        {
            // 1. Find the target element
            VisualElement? targetElement = elementName switch
            {
                "CharacterImage" => CharacterImage,
                "PilonStarNicholLabel" => PilonStarNicholLabel.Parent as VisualElement,
                "TapisCollectionView" => TapisCollectionView,
                "BuyButton" => BuyButton,
                "HomeButton" => HomeButton,
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
            CharacterImage.Opacity = 1;
            TapisCollectionView.Opacity = 1;
            BuyButton.Opacity = 1;

            if (string.IsNullOrEmpty(targetName))
                return;

            // Dim everything except target
            switch (targetName)
            {
                case "CharacterImage":
                    TapisCollectionView.Opacity = 0.3;
                    BuyButton.Opacity = 0.3;
                    break;
                case "PilonStarNicholLabel":
                    CharacterImage.Opacity = 0.5;
                    TapisCollectionView.Opacity = 0.3;
                    BuyButton.Opacity = 0.3;
                    break;
                case "TapisCollectionView":
                    CharacterImage.Opacity = 0.5;
                    BuyButton.Opacity = 0.3;
                    break;
                case "BuyButton":
                    CharacterImage.Opacity = 0.5;
                    TapisCollectionView.Opacity = 0.3;
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
            CharacterImage.Opacity = 1;
            TapisCollectionView.Opacity = 1;
            BuyButton.Opacity = 1;
        }

        private async Task LoadTapisItemsAsync()
        {
            await AvatarCustomizationHelper.LoadPurchasedCostume();

            purchasedCostumes = AvatarCustomizationHelper.purchasedCostumes;
            var items = new List<TapisItem>
                {
                    new() { TapisImageSource = currentAvatarSet.TapisPaths[0], AvatarImageSource = currentAvatarSet.CostumePaths[1], IsPurchased = purchasedCostumes[0], Price = 100, TapisId = 1 },
                    new() { TapisImageSource = currentAvatarSet.TapisPaths[1], AvatarImageSource = currentAvatarSet.CostumePaths[2], IsPurchased = purchasedCostumes[1], Price = 150, TapisId = 2 },
                    new() { TapisImageSource = currentAvatarSet.TapisPaths[2], AvatarImageSource = currentAvatarSet.CostumePaths[3], IsPurchased = purchasedCostumes[2], Price = 180, TapisId = 3 },
                    new() { TapisImageSource = currentAvatarSet.TapisPaths[3], AvatarImageSource = currentAvatarSet.CostumePaths[4], IsPurchased = purchasedCostumes[3], Price = 200, TapisId = 4 },
                    new() { TapisImageSource = currentAvatarSet.TapisPaths[4], AvatarImageSource = currentAvatarSet.CostumePaths[5], IsPurchased = purchasedCostumes[4], Price = 250, TapisId = 5 }
                };

            TapisItems = items;
            TapisCollectionView.ItemsSource = TapisItems;

            if (selectedCostumeId > 0 && selectedCostumeId <= TapisItems.Count)
                TapisCollectionView.SelectedItem = TapisItems[selectedCostumeId - 1];
        }

        private bool IsEquipped(TapisItem item)
        {
            return NormalizeAvatarKey(CharacterHelper.CurrentAvatar)
                == NormalizeAvatarKey(item.AvatarImageSource);
        }

        private static string NormalizeAvatarKey(string src)
        {
            if (string.IsNullOrWhiteSpace(src)) return string.Empty;

            src = src.Replace("\\", "/").Trim().ToLowerInvariant();

            var lastSlash = src.LastIndexOf('/');
            return lastSlash >= 0 ? src[(lastSlash + 1)..] : src;
        }

        private async void OnTapisSelected(object sender, SelectionChangedEventArgs e)
        {
            await SoundService.PlayButtonClickAsync();

            var selected = e.CurrentSelection.FirstOrDefault() as TapisItem;
            if (selected == null) return;

            foreach (var item in TapisItems)
                item.IsSelected = false;

            selected.IsSelected = true;
            selectedCostumeId = selected.TapisId;

            UpdateBuyButtonState(selected);
            await AnimateAvatarChangeAsync(selected.AvatarImageSource);
        }

        private void UpdateBuyButtonState(TapisItem selected)
        {
            if (selected.IsPurchased && IsEquipped(selected))
            {
                BuyButton.Text = "NAKA-SUOT";
                BuyButton.BackgroundColor = Colors.SeaGreen;
                BuyButton.IsEnabled = false;
                return;
            }

            if (selected.IsPurchased)
            {
                BuyButton.Text = "PILIIN";
                BuyButton.BackgroundColor = Colors.SeaGreen;
                BuyButton.IsEnabled = true;
                return;
            }

            BuyButton.Text = $"BILHIN {selected.Price}";
            BuyButton.BackgroundColor = Colors.DeepPink;
            BuyButton.IsEnabled = PilonStarNicholAmountValue >= selected.Price;
        }

        private async Task AnimateAvatarChangeAsync(string newAvatarSource)
        {
            try
            {
                await CharacterImage.FadeTo(0.25, 120, Easing.CubicIn);

                CharacterImage.Source = $"{newAvatarSource}";

                CharacterImage.Rotation = 0;
                CharacterImage.TranslationY = 0;

                var pop = CharacterImage.ScaleTo(1.18, 180, Easing.CubicOut);
                var rot = CharacterImage.RotateTo(6, 140, Easing.CubicOut);
                await Task.WhenAll(pop, rot);

                var settleScale = CharacterImage.ScaleTo(1.0, 220, Easing.BounceOut);
                var settleRot = CharacterImage.RotateTo(0, 140, Easing.SpringOut);
                var translateUp = CharacterImage.TranslateTo(0, -8, 100, Easing.CubicOut);
                await Task.WhenAll(settleScale, settleRot, translateUp);

                await CharacterImage.TranslateTo(0, 0, 160, Easing.BounceOut);

                await CharacterImage.FadeTo(1.0, 150, Easing.CubicIn);

                await BuyButton.ScaleTo(1.06, 110, Easing.CubicOut);
                await BuyButton.ScaleTo(1.0, 120, Easing.BounceOut);
            }
            catch
            {

            }
        }

        private async void OnBuyButtonClicked(object sender, EventArgs e)
        {
            await SoundService.PlayButtonClickAsync();

            if (selectedCostumeId == 0) return;

            var selected = TapisItems[selectedCostumeId - 1];
            int itemCost = selected.Price;

            if (selected.IsPurchased)
            {
                CharacterHelper.CurrentAvatar = selected.AvatarImageSource;
                await App.Database.UpdateCurrentAvatarAsync(CharacterHelper.CurrentAvatar);

                await ShowGameAlertAsync(
                    PilonStarNicholAmountValue,
                    "Naka-suot na ang tapis!",
                    "emoji_happy.png"
                );

                BuyButton.Text = "NAKA-SUOT";
                BuyButton.IsEnabled = false;
                return;
            }

            if (PilonStarNicholAmountValue < itemCost)
            {
                await ShowGameAlertAsync(PilonStarNicholAmountValue, null, "emoji_sad.png");
                return;
            }

            PilonStarNicholAmountValue -= itemCost;
            CharacterHelper.CurrentStars = PilonStarNicholAmountValue;
            PilonStarNicholLabel.Text = PilonStarNicholAmountValue.ToString();

            selected.IsPurchased = true;
            purchasedCostumes[selectedCostumeId - 1] = true;

            await App.Database.SetStarsAsync(PilonStarNicholAmountValue);

            string avatarId = AvatarCustomizationHelper.SelectedAvatarSetId;
            string costumeKey = (selectedCostumeId - 1) switch
            {
                0 => "avatarblue",
                1 => "avatarbluered",
                2 => "avatargreen",
                3 => "avatarpink",
                4 => "avatarred",
                _ => throw new ArgumentOutOfRangeException()
            };

            await App.Database.UnlockCostumeAsync(avatarId, costumeKey);

            CharacterHelper.CurrentAvatar = selected.AvatarImageSource;
            await App.Database.UpdateCurrentAvatarAsync(CharacterHelper.CurrentAvatar);

            await ShowGameAlertAsync(
                PilonStarNicholAmountValue,
                "Nabili!",
                "emoji_happy.png"
            );

            UpdateBuyButtonState(selected);
            TapisCollectionView.ItemsSource = null;
            TapisCollectionView.ItemsSource = TapisItems;
        }

        private async void OnHomeButtonClicked(object sender, EventArgs e)
        {
            await SoundService.PlayButtonClickAsync();
            await Navigation.PopAsync();
        }

        public Task ShowGameAlertAsync(int amount, string message = null, string emojiSource = "emoji_sad.png")
        {
            if (GameAlertOverlay.IsVisible && _alertTcs != null)
                return _alertTcs.Task;

            _alertTcs = new TaskCompletionSource<bool>();

            AlertEmoji.Source = emojiSource;

            AlertAmountLabel.Text = PilonStarNicholAmountValue.ToString();
            if (string.IsNullOrWhiteSpace(message))
                message = $"Kulang ang iyong Pilon Star!";

            AlertMessageLabel.Text = message;

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

        private async Task HideGameAlertAsync(bool completedByUser = true)
        {
            if (!GameAlertOverlay.IsVisible) return;

            try
            {
                await GameAlertCard.ScaleTo(0.96, 120, Easing.CubicIn);
                await GameAlertOverlay.FadeTo(0, 140, Easing.CubicOut);
            }
            catch { }

            GameAlertOverlay.IsVisible = false;

            _alertTcs?.TrySetResult(completedByUser);
            _alertTcs = null;
        }

        private async void OnAlertOkClicked(object sender, EventArgs e)
        {
            await SoundService.PlayButtonClickAsync();
            await HideGameAlertAsync(true);
        }

        private async void OnAlertBackgroundTapped(object sender, EventArgs e)
        {
            await HideGameAlertAsync(false);
        }

        private class TutorialStep
        {
            public string Title { get; set; } = "";
            public string Message { get; set; } = "";
            public string? TargetElementName { get; set; }
            public double OffsetX { get; set; } = 0;
        }
    }

    public class TapisItem
    {
        public string TapisImageSource { get; set; }
        public string AvatarImageSource { get; set; }
        public bool IsPurchased { get; set; }
        public int Price { get; set; }
        public int TapisId { get; set; }
        public bool IsSelected { get; set; }
    }
}