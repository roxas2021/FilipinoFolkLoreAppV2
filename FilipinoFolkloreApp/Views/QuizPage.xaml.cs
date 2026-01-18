using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using FilipinoFolkloreApp.Views.Home;
using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views;

public partial class QuizPage : ContentPage
{
    private readonly string _storyId;
    int _correctIndex = 0;
    int _quizIndex = 0;

    CancellationTokenSource? _cts;
    private HeartService HeartService =>
    Application.Current!.Handler!.MauiContext!.Services.GetService<HeartService>()!;


    public QuizPage(string storyId)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        _storyId = storyId;

        // Header HUD
        LoadHud();

        // Load first question (supports more later)
        var story = AlamatContent.GetStory(_storyId);
        if (story.Quiz == null || story.Quiz.Count == 0)
        {
            // No quiz? Just reward and exit gracefully
            _ = HandleCorrectAsync();
            return;
        }

        //var q = story.Quiz[0];
        //QuizPrompt.Text = q.Prompt;
        //_correctIndex = q.CorrectIndex;
        //Choice0.Source = q.ChoiceImages.ElementAtOrDefault(0);
        //Choice1.Source = q.ChoiceImages.ElementAtOrDefault(1);
        //Choice2.Source = q.ChoiceImages.ElementAtOrDefault(2);

        //_cts = new CancellationTokenSource();
        //_ = TimerAsync(q.TimeLimitSec, _cts.Token);
        LoadQuiz();

    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _cts?.Cancel();
    }
    async Task LoadQuiz()
    {
        var story = AlamatContent.GetStory(_storyId);

        // Finished all quizzes
        //if (_quizIndex >= story.Quiz.Count)
        //{
        //    _ = HandleAllQuizzesCompletedAsync();
        //    return;
        //}

        var q = story.Quiz[_quizIndex];

        await AnimateQuizSwapAsync(() =>
        {
            QuizPrompt.Text = q.Prompt;
            _correctIndex = q.CorrectIndex;

            Choice0.Source = q.ChoiceImages.ElementAtOrDefault(0);
            Choice1.Source = q.ChoiceImages.ElementAtOrDefault(1);
            Choice2.Source = q.ChoiceImages.ElementAtOrDefault(2);
        });
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _ = TimerAsync(q.TimeLimitSec, _cts.Token);
    }

    async Task TimerAsync(int seconds, CancellationToken ct)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds), ct);
            if (!ct.IsCancellationRequested)
                await HandleWrongAsync();
        }
        catch (TaskCanceledException) { }
    }
    async Task AnimateQuizSwapAsync(Action swapContent)
    {
        // Animate out
        await Task.WhenAll(
            QuizContentWrapper.FadeTo(0, 150, Easing.CubicIn),
            QuizContentWrapper.TranslateTo(0, -12, 150, Easing.CubicIn)
        );

        // Swap quiz content
        swapContent.Invoke();

        // Reset position for animate-in
        QuizContentWrapper.TranslationY = 12;

        // Animate in
        await Task.WhenAll(
            QuizContentWrapper.FadeTo(1, 180, Easing.CubicOut),
            QuizContentWrapper.TranslateTo(0, 0, 180, Easing.CubicOut)
        );
    }

    async Task HandlePickAsync(int idx)
    {
        // ?? Guard: no hearts left
        if (HeartService.GetHearts() <= 0)
        {
            await DisplayAlert(
                "Wala nang ??",
                "Babalik ang mga puso pagkalipas ng 5 minuto.",
                "OK"
            );
            return;
        }

        // Normal flow
        if (idx == _correctIndex)
            await HandleCorrectAsync();
        else
            await HandleWrongAsync();
    }
    async Task HandleCorrectAsync()
    {
        _cts?.Cancel();

        _quizIndex++;

        var story = AlamatContent.GetStory(_storyId);

        // More quizzes left → load next quiz
        if (_quizIndex < story.Quiz.Count)
        {
            await LoadQuiz();
            return;
        }

        // No more quizzes → reward player
        var reward = story.RewardStars;
        var isClaimed = story.IsRewardClaimed;

        if (!isClaimed)
        {
            await App.Database.SetStarsAsync(CharacterHelper.CurrentStars + reward);
            CharacterHelper.CurrentStars += reward;
            RefreshStars();
        }

        await Navigation.PushAsync(new RewardPage(reward, _storyId));
    }


    //async Task HandleCorrectAsync()
    //{
    //    _cts?.Cancel();

    //    var reward = AlamatContent.GetStory(_storyId).RewardStars;
    //    //AlamatContent.Stars += reward;
    //    var getStory = AlamatContent.GetStory(_storyId).IsRewardClaimed;

    //    if (!getStory)
    //    {
    //        await App.Database.SetStarsAsync(CharacterHelper.CurrentStars + reward);
    //        CharacterHelper.CurrentStars += reward; // keep in sync
    //        RefreshStars(); // reflect new stars in header
    //    }


    //    await Navigation.PushAsync(new RewardPage(reward, _storyId));
    //}

    async Task HandleWrongAsync()
    {
        _cts?.Cancel();
        HeartService.LoseHeart();
        RefreshHearts();

        await ShowWrongModalAsync();
    }

    async Task ShowWrongModalAsync()
    {
        var story = AlamatContent.GetStory(_storyId);

        AlertNarrator.Source = AlamatContent.CurrentNarrator.Avatar;

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

        AlertThumb.Source = story.Thumb;

        GameAlertOverlay.IsVisible = true;
        GameAlertOverlay.Opacity = 0;
        GameAlertCard.Scale = 0.96;

        await Task.WhenAll(
            GameAlertOverlay.FadeTo(1, 180, Easing.CubicOut),
            GameAlertCard.ScaleTo(1.0, 180, Easing.CubicOut)
        );
    }

    async Task HideWrongModalAsync()
    {
        await Task.WhenAll(
    GameAlertOverlay.FadeTo(0, 80, Easing.CubicIn), // Reduced animation time
    GameAlertCard.ScaleTo(0.96, 80, Easing.CubicIn)
);
        GameAlertOverlay.IsVisible = false;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        AlamatContent.Hearts = HeartService.GetHearts();
        // ?? Restore hearts if 5 minutes already passed
        RefreshHearts();
    }

    async void OnAlertReplayTapped(object? s, TappedEventArgs e)
    {
        await HideWrongModalAsync();
        await Navigation.PushAsync(new StoryPage(_storyId));
    }

    async void OnAlertCloseTapped(object? s, TappedEventArgs e)
    {
        _cts?.Cancel();
        await Navigation.PushAsync(new AlamatPage(AlamatContent.category));
    }

    void OnAlertBackgroundTapped(object? s, TappedEventArgs e)
    {
        // Intentionally no-op to prevent skipping
    }


    async void OnPick0(object? s, TappedEventArgs e) => await HandlePickAsync(0);
    async void OnPick1(object? s, TappedEventArgs e) => await HandlePickAsync(1);
    async void OnPick2(object? s, TappedEventArgs e) => await HandlePickAsync(2);

    // ---------- HUD helpers ----------
    void LoadHud()
    {
        HudAvatar.Source = CharacterHelper.CurrentAvatar;
        NarratorAvatar.Source = AlamatContent.CurrentNarrator.Avatar;
        PlayerNameLabel.Text = CharacterHelper.CurrentName; // replace if you have a real player name
        RefreshStars();
        RefreshHearts();
    }

    void RefreshStars()
    {
        StarsLabel.Text = CharacterHelper.CurrentStars.ToString();
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

    // ---------- Header actions ----------
    async void OnHomeTapped(object? s, TappedEventArgs e)
    {
        _cts?.Cancel();
        await NavigationHelper.NavigateToIndexPage(Navigation);
    }
}
