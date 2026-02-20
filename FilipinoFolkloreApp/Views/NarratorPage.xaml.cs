using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views.Home;
using Microsoft.Maui.Layouts;

namespace FilipinoFolkloreApp.Views;

public partial class NarratorPage : ContentPage
{
    private readonly string _storyId;
    private TaskCompletionSource<bool>? _alertTcs;
    private HeartService HeartService =>
    Application.Current!.Handler!.MauiContext!.Services.GetService<HeartService>()!;

    private SoundService SoundService =>
        Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;

    // Double-tap prevention flag
    private bool _isNavigating = false;

    // Tutorial state
    private int _tutorialStep = 0;
    private const string TUTORIAL_COMPLETED_KEY = "NarratorPageTutorialCompleted4";

    // Tutorial steps configuration - FOCUSED ON NARRATOR MECHANICS
    private readonly TutorialStep[] _tutorialSteps = new[]
    {
        new TutorialStep
        {
            Title = "Pumili ng Narrator!",
            Message = "Dito makikita mo ang mga narrator na pwedeng magbasa ng kuwento. Piliin ang gusto mo!",
            TargetElementName = null,

        },
        new TutorialStep
        {
            Title = "Libre ang Tarsier!",
            Message = "Ang Tarsier narrator ay laging libre at pwede mong gamitin anumang oras! I-click para pumili!",
            TargetElementName = "FirstUnlockedNarrator",
            OffsetX = 100
        },
        new TutorialStep
        {
            Title = "Mga Nakandado",
            Message = "Ang may lock icon at presyo ay kailangan pang bilhin gamit ang iyong stars. I-click para bumili!",
            TargetElementName = "FirstLockedNarrator",
            OffsetX = -100
        }
    };

    class Card
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Avatar { get; set; } = "";
        public bool IsLocked { get; set; }
        public int Price { get; set; }
        public string PriceText => $"{Price}⭐";
    }

    public NarratorPage(string storyId)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _storyId = storyId;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Reset navigation flag when page appears
        _isNavigating = false;

        AlamatContent.Hearts = HeartService.GetHearts();

        // Check and refresh narrator battery
        AlamatContent.CheckAndRefreshNarratorBattery();

        try
        {
            // Sync in-memory story monitored fields from DB.
            await App.Database.LoadStoriesAsync();
            await App.Database.LoadNarratorDataAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error syncing stories on appear: {ex}");
        }

        // Refresh UI now that data is consistent
        LoadHud();
        RefreshNarratorList();

        // Check if tutorial should be shown
        bool tutorialCompleted = Preferences.Get(TUTORIAL_COMPLETED_KEY, false);
        if (!tutorialCompleted)
        {
            await Task.Delay(800); // Wait for narrators to load
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
        Rect targetBounds = Rect.Zero;

        // 1. Find the target bounds using the Virtual Anchor approach
        if (elementName == "FirstUnlockedNarrator" || elementName == "FirstLockedNarrator")
        {
            await Task.Delay(200); // Give layout time to render

            Rect cvBounds = GetAbsolutePosition(NarratorsView);

            if (cvBounds != Rect.Zero)
            {
                // Check device type to match your XAML's OnIdiom Span
                double columns = 3; // Default for Phone
                if (DeviceInfo.Idiom == DeviceIdiom.Tablet) columns = 5;
                if (DeviceInfo.Idiom == DeviceIdiom.Desktop) columns = 6;

                double spacing = 16; // Match your HorizontalItemSpacing

                // Calculate the exact width of a single narrator card
                double cardWidth = (cvBounds.Width - (spacing * (columns - 1))) / columns;

                double targetX = cvBounds.X;

                // If we want the SECOND item (FirstLockedNarrator), shift right by one card + spacing
                if (elementName == "FirstLockedNarrator")
                {
                    targetX += cardWidth + spacing;
                }

                // Create the virtual target box (Using 180 for height to roughly cover Avatar + Price)
                targetBounds = new Rect(targetX, cvBounds.Y, cardWidth, 180);
            }
        }
        else if (elementName == "NarratorsGridContainer")
        {
            // Target the whole grid for the first step
            targetBounds = GetAbsolutePosition(NarratorsGridContainer);
        }

        if (targetBounds == Rect.Zero)
        {
            ArrowPointer.Opacity = 0;
            return;
        }

        await Task.Delay(150);

        // 2. Get screen info and target bounds
        var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
        double screenHeight = displayInfo.Height / displayInfo.Density;
        double safeZone = 60;

        double arrowWidth = 50;
        double arrowHeight = 50;
        double padding = 10;

        // 3. Calculate Potential Positions
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

        // 4. Apply Position and Rotation
        double arrowX = targetBounds.Center.X - (arrowWidth / 2);

        AbsoluteLayout.SetLayoutBounds(ArrowPointer, new Rect(arrowX, finalArrowY, arrowWidth, arrowHeight));
        AbsoluteLayout.SetLayoutFlags(ArrowPointer, AbsoluteLayoutFlags.None);

        ArrowPointer.Rotation = isArrowAbove ? 90 : -90;

        bool arrowIsAtTopHalf = finalArrowY < (screenHeight / 2);
        PositionSpeechBubble(!arrowIsAtTopHalf, offsetX);

        await AnimateArrowPointer();
    }

    private VisualElement? GetNarratorCardByIndex(int targetIndex)
    {
        try
        {
            if (NarratorsView?.Handler?.PlatformView != null)
            {
                var items = GetVisualTreeDescendants(NarratorsView);
                var narratorCards = new List<Grid>();

                // Collect all narrator cards in order
                foreach (var item in items)
                {
                    if (item is Grid grid &&
                        grid.BindingContext is Card &&
                        grid.IsVisible)
                    {
                        narratorCards.Add(grid);
                    }
                }

                // Return the card at the specified index
                if (targetIndex >= 0 && targetIndex < narratorCards.Count)
                {
                    System.Diagnostics.Debug.WriteLine($"Found narrator card at index {targetIndex}: {(narratorCards[targetIndex].BindingContext as Card)?.Name}");
                    return narratorCards[targetIndex];
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Narrator card index {targetIndex} out of range. Total cards: {narratorCards.Count}");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting narrator card by index: {ex}");
        }

        return null;
    }

    private IEnumerable<Element> GetVisualTreeDescendants(Element element)
    {
        var queue = new Queue<Element>();
        queue.Enqueue(element);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            yield return current;

            if (current is Layout layout)
            {
                foreach (var child in layout.Children.OfType<Element>())
                {
                    queue.Enqueue(child);
                }
            }
            else if (current is ContentView contentView && contentView.Content != null)
            {
                queue.Enqueue(contentView.Content);
            }
            else if (current is ScrollView scrollView && scrollView.Content != null)
            {
                queue.Enqueue(scrollView.Content);
            }
        }
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
        NarratorsGridContainer.Opacity = 1;

        if (string.IsNullOrEmpty(targetName))
            return;

        // For specific narrator cards, we slightly dim the container
        switch (targetName)
        {
            case "FirstUnlockedNarrator":
            case "FirstLockedNarrator":
                NarratorsGridContainer.Opacity = 0.95;
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
        NarratorsGridContainer.Opacity = 1;
    }

    void RefreshNarratorList()
    {
        // Get the current story (safe lookup)
        var story = AlamatContent.Stories.FirstOrDefault(s => s.Id == _storyId);

        NarratorsView.ItemsSource = AlamatContent.Narrators.Select(n =>
        {
            // per-story unlock check: tarsier always unlocked, or global unlocked set,
            // or the current story has the narrator unlocked flag set.
            bool unlocked = n.Id == "tarsier"
                            || AlamatContent.UnlockedNarrators.Contains(n.Id) // global unlocks if used
                            || (story != null && n.Id == "eagle" && story.NarratorEagleUnlocked)
                            || (story != null && n.Id == "monkey" && story.NarratorMonkeyUnlocked);

            return new Card
            {
                Id = n.Id,
                Name = n.Name,
                Avatar = n.Avatar,
                IsLocked = !unlocked,
                Price = n.PriceStars
            };
        }).ToList();
    }

    async void OnNarratorTapped(object? sender, TappedEventArgs e)
    {
        // Prevent double-tap: if already navigating, ignore this tap
        if (_isNavigating)
        {
            System.Diagnostics.Debug.WriteLine("Double-tap prevented: Already navigating to story.");
            return;
        }

        await SoundService.PlayButtonClickAsync();

        if (sender is not Grid g || g.BindingContext is not Card c) return;

        // Check narrator battery before proceeding
        if (!AlamatContent.CanUseNarrator())
        {
            // Calculate time until next battery refresh
            var timeSinceLastUse = DateTime.Now - AlamatContent.LastNarratorUseTime;
            var minutesUntilRefresh = 10 - ((int)timeSinceLastUse.TotalMinutes % 10);

            await ShowGameAlertAsync(
                $"Maghintay ng {minutesUntilRefresh} minuto para sa susunod na narrator battery.",
                false
            );
            return;
        }

        // Set navigation flag to prevent double-tap
        _isNavigating = true;

        try
        {
            // If already unlocked for this story or globally, just select and continue
            var story = AlamatContent.GetStory(_storyId);

            bool alreadyUnlockedForThisStory =
                c.Id == "tarsier" ||
                AlamatContent.UnlockedNarrators.Contains(c.Id) || // global
                (c.Id == "eagle" && story.NarratorEagleUnlocked) ||
                (c.Id == "monkey" && story.NarratorMonkeyUnlocked);

            if (!alreadyUnlockedForThisStory)
            {
                if (!AlamatContent.TrySpendStars(c.Price))
                {
                    await ShowGameAlertAsync($"Kailangan: {c.Price}", false);
                    _isNavigating = false; // Reset flag before returning
                    return;
                }

                // Save previous flags to rollback on DB failure
                bool previousEagle = story.NarratorEagleUnlocked;
                bool previousMonkey = story.NarratorMonkeyUnlocked;

                // set the per-story flag
                switch (c.Id)
                {
                    case "eagle":
                        story.NarratorEagleUnlocked = true;
                        break;
                    case "monkey":
                        story.NarratorMonkeyUnlocked = true;
                        break;
                }

                bool saved = false;
                try
                {
                    await App.Database.UpdateStoryAsync(story);
                    await App.Database.SetStarsAsync(CharacterHelper.CurrentStars - c.Price);
                    CharacterHelper.CurrentStars -= c.Price; // keep in sync
                    saved = true;
                }
                catch (Exception ex)
                {
                    // rollback if DB save fails: restore story flags and refund stars
                    story.NarratorEagleUnlocked = previousEagle;
                    story.NarratorMonkeyUnlocked = previousMonkey;
                    AlamatContent.Stars += c.Price; // refund
                    System.Diagnostics.Debug.WriteLine($"UpdateStoryAsync failed while unlocking narrator: {ex}");
                    await ShowGameAlertAsync("Hindi naisave ang narrator — subukang muli.", false);
                }

                if (!saved)
                {
                    LoadHud();
                    RefreshNarratorList();
                    _isNavigating = false; // Reset flag before returning
                    return;
                }

                // saved ok -> refresh HUD and list
                LoadHud();
                RefreshNarratorList();
            }

            // Use narrator battery (deduct 1)
            await AlamatContent.UseNarratorAsync();

            // Save selected narrator to database
            AlamatContent.SelectedNarratorId = c.Id;
            AlamatContent.CurrentNarratorImage = c.Avatar;
            await App.Database.UpdateSelectedNarratorAsync(c.Id);

            await Navigation.PushAsync(new StoryPage(_storyId));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnNarratorTapped: {ex}");
            _isNavigating = false; // Reset flag on error
        }
        // Note: Don't reset _isNavigating here - it will be reset in OnAppearing when user returns
    }

    void LoadHud()
    {
        HudAvatar.Source = CharacterHelper.CurrentAvatar;
        StarsLabel.Text = CharacterHelper.CurrentStars.ToString();
        PlayerNameLabel.Text = CharacterHelper.CurrentName;
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

    // Custom Game Alert with Yes/No or OK buttons
    private Task<bool> ShowGameAlertAsync(string message, bool showYesNo = false)
    {
        if (GameAlertOverlay.IsVisible && _alertTcs != null)
            return _alertTcs.Task;

        _alertTcs = new TaskCompletionSource<bool>();

        // Set message
        AlertMessageLabel.Text = message;

        // Clear existing buttons
        AlertButtonsPanel.Children.Clear();

        if (showYesNo)
        {
            // Add Yes button
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

            // Add No button
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
            // Add OK button
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

    async void OnBackTapped(object? s, TappedEventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        if (Navigation.NavigationStack.Count > 1)
            await Navigation.PopAsync();
    }

    async void OnHomeTapped(object? s, TappedEventArgs e)
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