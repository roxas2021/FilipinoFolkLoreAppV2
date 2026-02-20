using FilipinoFolkloreApp.Models;
using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views.Home;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilipinoFolkloreApp.Views
{
    public partial class BugtongQuizPage : ContentPage
    {
        private readonly string _bugtongId;
        private Bugtong? _currentBugtong;
        private int _correctIndex = 0;
        private List<string> _shuffledChoices = new();
        private BugtongMilestone? _pendingMilestone = null;

        private HeartService HeartService =>
            Application.Current!.Handler!.MauiContext!.Services.GetService<HeartService>()!;

        private SoundService SoundService =>
            Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;

        public BugtongQuizPage(string bugtongId)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            _bugtongId = bugtongId;
            LoadHud();
            LoadBugtong();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            AlamatContent.Hearts = HeartService.GetHearts();
            RefreshHearts();
        }

        private void LoadHud()
        {
            HudAvatar.Source = CharacterHelper.CurrentAvatar;
            NarratorAvatar.Source = AlamatContent.CurrentNarrator.Avatar;
            PlayerNameLabel.Text = CharacterHelper.CurrentName;
            RefreshStars();
            RefreshHearts();
        }

        private void RefreshStars()
        {
            StarsLabel.Text = CharacterHelper.CurrentStars.ToString();
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

        private void LoadBugtong()
        {
            _currentBugtong = BugtongService.GetBugtong(_bugtongId);
            if (_currentBugtong == null)
            {
                DisplayAlert("Error", "Hindi mahanap ang bugtong.", "OK");
                Navigation.PopAsync();
                return;
            }

            BugtongPrompt.Text = _currentBugtong.Prompt;
            InitializeLetterBoxes();

            // Shuffle choices and display them
            ShuffleAndDisplayChoices();
        }

        private void InitializeLetterBoxes()
        {
            if (_currentBugtong == null) return;

            AnswerLetterBoxes.Children.Clear();

            // Get the answer text to determine number of boxes needed
            string answer = _currentBugtong.Answer.ToUpper();

            foreach (char letter in answer)
            {
                // Create a border for each letter
                var letterBorder = new Border
                {
                    BackgroundColor = Colors.White,
                    Stroke = Color.FromArgb("#FF66CC"),
                    StrokeThickness = 3,
                    Padding = new Thickness(12, 8),
                    Margin = new Thickness(4, 2),
                    WidthRequest = letter == ' ' ? 20 : 45,
                    HeightRequest = 50,
                    StrokeShape = new RoundRectangle { CornerRadius = 8 }
                };

                var letterLabel = new Label
                {
                    Text = "",
                    FontSize = 24,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#FF66CC"),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };

                letterBorder.Content = letterLabel;
                AnswerLetterBoxes.Children.Add(letterBorder);
            }
        }

        private void ShuffleAndDisplayChoices()
        {
            if (_currentBugtong == null) return;

            // Shuffle the choices
            _shuffledChoices = BugtongService.ShuffleChoices(_currentBugtong.Choices);

            // Find the correct index in shuffled list
            _correctIndex = _shuffledChoices.IndexOf(_currentBugtong.Answer);

            // Display the shuffled choices
            Choice0.Text = _shuffledChoices.ElementAtOrDefault(0) ?? "";
            Choice1.Text = _shuffledChoices.ElementAtOrDefault(1) ?? "";
            Choice2.Text = _shuffledChoices.ElementAtOrDefault(2) ?? "";
        }

        private async Task HandlePickAsync(int idx)
        {
            await SoundService.PlayButtonClickAsync();

            if (_currentBugtong == null) return;

            // Check heart availability
            if (HeartService.GetHearts() <= 0)
            {
                await DisplayAlert(
                    "Wala nang ❤️",
                    "Babalik ang mga puso pagkalipas ng 5 minuto.",
                    "OK"
                );
                return;
            }

            // Get the selected word
            string selectedWord = _shuffledChoices.ElementAtOrDefault(idx) ?? "";

            // Show the selected word in answer box with animation
            await ShowAnswerAsync(selectedWord);

            // Check if correct
            if (idx == _correctIndex)
            {
                await HandleCorrectAsync();
            }
            else
            {
                await HandleWrongAsync();
            }
        }

        private async Task ShowAnswerAsync(string answer)
        {
            string upperAnswer = answer.ToUpper();

            // Set all letters first
            for (int i = 0; i < AnswerLetterBoxes.Children.Count && i < upperAnswer.Length; i++)
            {
                if (AnswerLetterBoxes.Children[i] is Border border && border.Content is Label label)
                {
                    label.Text = upperAnswer[i].ToString();
                    // Set initial state for animation
                    border.Scale = 0.5;
                    border.Opacity = 0;
                }
            }

            // Create list of animation tasks to run all together
            var animationTasks = new List<Task>();

            foreach (var child in AnswerLetterBoxes.Children)
            {
                if (child is Border border)
                {
                    animationTasks.Add(border.ScaleTo(1.0, 300, Easing.SpringOut));
                    animationTasks.Add(border.FadeTo(1, 300, Easing.CubicOut));
                }
            }

            // Execute all animations simultaneously
            await Task.WhenAll(animationTasks);

            // Keep visible for a moment
            await Task.Delay(500);
        }

        private async Task HandleCorrectAsync()
        {
            if (_currentBugtong == null) return;

            var reward = _currentBugtong.RewardStars;
            var isCompleted = await App.Database.IsBugtongCompletedAsync(_bugtongId);

            BugtongMilestone? milestone = null;
            bool hasMilestone = false;
            bool wasAlreadyCompleted = isCompleted; // Track this BEFORE marking as completed

            if (!isCompleted)
            {
                await App.Database.SetBugtongCompletedAsync(_bugtongId);
                await App.Database.SetStarsAsync(CharacterHelper.CurrentStars + reward);
                CharacterHelper.CurrentStars += reward;
                RefreshStars();

                // Check for milestone achievements
                var totalCompleted = await GetTotalCompletedBugtongsAsync();
                milestone = BugtongService.GetAchievedMilestone(totalCompleted);

                if (milestone != null)
                {
                    // Check if milestone reward was already claimed
                    string milestoneKey = $"Bugtong_Milestone_{milestone.RequiredCorrect}";
                    bool alreadyClaimed = Preferences.Get(milestoneKey, false);

                    if (!alreadyClaimed)
                    {
                        // DON'T unlock medal here - let ColoringRewardPage handle it
                        // Just store the pending milestone
                        hasMilestone = true;
                        _pendingMilestone = milestone;
                    }
                }
            }

            // Show success overlay (pass whether it was already completed)
            await ShowSuccessOverlayAsync(reward, wasAlreadyCompleted);
        }

        private async Task<int> GetTotalCompletedBugtongsAsync()
        {
            int count = 0;
            foreach (var bugtong in BugtongService.Bugtongs)
            {
                if (await App.Database.IsBugtongCompletedAsync(bugtong.Id))
                {
                    count++;
                }
            }
            return count;
        }

        private async Task ShowSuccessOverlayAsync(int stars, bool wasAlreadyCompleted)
        {
            // Reuse the existing GameAlertOverlay for success message
            AlertNarrator.Source = AlamatContent.CurrentNarrator.Avatar;

            // Hide title and retry button, show OK button
            AlertTitleLabel.IsVisible = false;
            AlertRetryButton.IsVisible = false;
            AlertOkButton.IsVisible = true;

            // Clear the hearts panel and show success message
            AlertHeartsPanel.Children.Clear();

            var successLabel = new Label
            {
                Text = wasAlreadyCompleted ? "Tama ang sagot" : $"Tama ang sagot +{stars}⭐",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#0000FF"),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            AlertHeartsPanel.Children.Add(successLabel);

            GameAlertOverlay.IsVisible = true;
            GameAlertOverlay.InputTransparent = false;
            GameAlertOverlay.Opacity = 0;
            GameAlertCard.Scale = 0.96;

            await Task.WhenAll(
                GameAlertOverlay.FadeTo(1, 180, Easing.CubicOut),
                GameAlertCard.ScaleTo(1.0, 180, Easing.CubicOut)
            );
        }

        private async Task HideSuccessOverlayAsync()
        {
            await Task.WhenAll(
                GameAlertOverlay.FadeTo(0, 80, Easing.CubicIn),
                GameAlertCard.ScaleTo(0.96, 80, Easing.CubicIn)
            );
            GameAlertOverlay.IsVisible = false;
            GameAlertOverlay.InputTransparent = true;
        }

        private async Task HandleWrongAsync()
        {
            HeartService.LoseHeart();
            RefreshHearts();

            await ShowWrongModalAsync();
        }

        private async Task ShowWrongModalAsync()
        {
            AlertNarrator.Source = AlamatContent.CurrentNarrator.Avatar;

            // Show title and retry button, hide OK button
            AlertTitleLabel.IsVisible = true;
            AlertRetryButton.IsVisible = true;
            AlertOkButton.IsVisible = false;

            AlertHeartsPanel.Children.Clear();
            for (int i = 0; i < AlamatContent.Hearts; i++)
            {
                AlertHeartsPanel.Children.Add(new Image
                {
                    Source = "heart_full.png",
                    WidthRequest = 24,
                    HeightRequest = 24,
                    Aspect = Aspect.AspectFit
                });
            }

            GameAlertOverlay.IsVisible = true;
            GameAlertOverlay.InputTransparent = false; // Block clicks
            GameAlertOverlay.Opacity = 0;
            GameAlertCard.Scale = 0.96;

            await Task.WhenAll(
                GameAlertOverlay.FadeTo(1, 180, Easing.CubicOut),
                GameAlertCard.ScaleTo(1.0, 180, Easing.CubicOut)
            );
        }

        private async Task HideWrongModalAsync()
        {
            await Task.WhenAll(
                GameAlertOverlay.FadeTo(0, 80, Easing.CubicIn),
                GameAlertCard.ScaleTo(0.96, 80, Easing.CubicIn)
            );
            GameAlertOverlay.IsVisible = false;
            GameAlertOverlay.InputTransparent = true; // Allow clicks again
        }

        private void OnOverlayBackgroundTapped(object? sender, TappedEventArgs e)
        {
            // Do nothing - prevents clicks from passing through
            // This keeps the modal locked until user interacts with buttons
        }

        private async void OnAlertRetryTapped(object? sender, TappedEventArgs e)
        {
            await SoundService.PlayButtonClickAsync();
            await HideWrongModalAsync();

            // Clear answer boxes
            ClearAnswerBoxes();
            ShuffleAndDisplayChoices();
        }

        private void ClearAnswerBoxes()
        {
            foreach (var child in AnswerLetterBoxes.Children)
            {
                if (child is Border border && border.Content is Label label)
                {
                    label.Text = "";
                }
            }
        }

        private async void OnAlertOkTapped(object? sender, TappedEventArgs e)
        {
            await OnAlertOkTappedAsync();
        }

        private async Task OnAlertOkTappedAsync()
        {
            await SoundService.PlayButtonClickAsync();
            await HideSuccessOverlayAsync();

            // Check if there's a pending milestone to show
            if (_pendingMilestone != null)
            {
                var milestone = _pendingMilestone;
                _pendingMilestone = null;

                string milestoneKey = $"Bugtong_Milestone_{milestone.RequiredCorrect}";

                // Navigate to reward page which will handle unlocking the medal and claiming the reward
                await Navigation.PushAsync(new ColoringRewardPage(
                    stars: milestone.RewardStars,
                    medalId: milestone.MedalId,
                    rewardKey: milestoneKey,
                    returnPageType: "BugtongList",
                    returnPageParameter: null
                ));
            }
            else
            {
                // No milestone, just go back to list
                await NavigateToBugtongList();
            }
        }

        private async void OnAlertCloseTapped(object? sender, TappedEventArgs e)
        {
            await SoundService.PlayButtonClickAsync();

            // Check which overlay is showing
            if (AlertOkButton.IsVisible) // Success overlay
            {
                await OnAlertOkTappedAsync();
            }
            else // Wrong answer overlay
            {
                await HideWrongModalAsync();
                await NavigateToBugtongList();
            }
        }

        private async Task NavigateToBugtongList()
        {
            await Navigation.PopAsync();

            // Remove this page after navigation
            Navigation.RemovePage(this);
        }

        private async void OnBackTapped(object? sender, TappedEventArgs e)
        {
            await SoundService.PlayButtonClickAsync();
            await Navigation.PopAsync();
        }

        private async void OnHomeTapped(object? sender, TappedEventArgs e)
        {
            await SoundService.PlayButtonClickAsync();

            // Use NavigationHelper for safer navigation
            var pages = Navigation.NavigationStack.ToList();
            foreach (var page in pages)
            {
                if (page is BugtongQuizPage)
                {
                    Navigation.RemovePage(page);
                }
                if (page is BugtongListPage)
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

        private async void OnPick0(object? sender, TappedEventArgs e) => await HandlePickAsync(0);
        private async void OnPick1(object? sender, TappedEventArgs e) => await HandlePickAsync(1);
        private async void OnPick2(object? sender, TappedEventArgs e) => await HandlePickAsync(2);
    }
}