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
    int _totalQuizStarsEarned = 0;

    CancellationTokenSource? _cts;
    private HeartService HeartService =>
    Application.Current!.Handler!.MauiContext!.Services.GetService<HeartService>()!;

    private SoundService SoundService =>
        Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;


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

        var q = story.Quiz[_quizIndex];

        await AnimateQuizSwapAsync(() =>
        {
            QuizPrompt.Text = q.Prompt;
            _correctIndex = q.CorrectIndex;

            // Set text answers instead of images
            Choice0Text.Text = q.ChoiceTexts.ElementAtOrDefault(0) ?? "";
            Choice1Text.Text = q.ChoiceTexts.ElementAtOrDefault(1) ?? "";
            Choice2Text.Text = q.ChoiceTexts.ElementAtOrDefault(2) ?? "";
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
        await SoundService.PlayButtonClickAsync();
        
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

        var story = AlamatContent.GetStory(_storyId);
        var currentQuestion = story.Quiz[_quizIndex];

        // Check if this is the first time answering this question correctly
        bool isFirstTime = await App.Database.SetQuizQuestionAnsweredAsync(_storyId, _quizIndex);
        
        if (isFirstTime)
        {
            // Award stars for first correct answer
            int starsToAward = currentQuestion.RewardStars;
            _totalQuizStarsEarned += starsToAward;
            
            await App.Database.SetStarsAsync(CharacterHelper.CurrentStars + starsToAward);
            CharacterHelper.CurrentStars += starsToAward;
            RefreshStars();
        }

        _quizIndex++;

        // More quizzes left → load next quiz
        if (_quizIndex < story.Quiz.Count)
        {
            await LoadQuiz();
            return;
        }

        // No more quizzes → reward player with story completion stars
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
    GameAlertOverlay.FadeTo(0, 80, Easing.CubicIn),
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
        await SoundService.PlayButtonClickAsync();
        await HideWrongModalAsync();
        await Navigation.PushAsync(new StoryPage(_storyId));
    }

    async void OnAlertCloseTapped(object? s, TappedEventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        _cts?.Cancel();
        var pages = Navigation.NavigationStack.ToList();
        foreach (var page in pages)
        {
            if (page is RewardPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is QuizPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is StoryPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is NarratorPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is AlamatPage)
            {
                Navigation.RemovePage(page);
            }
        }
        await Navigation.PushAsync(new IndexPage());
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
        PlayerNameLabel.Text = CharacterHelper.CurrentName;
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
        await SoundService.PlayButtonClickAsync();
        _cts?.Cancel();
        var pages = Navigation.NavigationStack.ToList();
        foreach (var page in pages)
        {
            if (page is RewardPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is QuizPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is StoryPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is NarratorPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is AlamatPage)
            {
                Navigation.RemovePage(page);
            }
        }

        await Navigation.PushAsync(new IndexPage());
    }

}
