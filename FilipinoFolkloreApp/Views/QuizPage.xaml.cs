using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views;

public partial class QuizPage : ContentPage
{
    private readonly string _storyId;
    int _correctIndex = 0;
    CancellationTokenSource? _cts;

    public QuizPage(string storyId)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _storyId = storyId;

        // HUD
        QuizAvatar.Source = AlamatContent.CurrentNarrator.Avatar;
        QuizHearts.Text = $"{AlamatContent.Hearts}";

        // Use first question for now (supports more later)
        var q = AlamatContent.GetStory(_storyId).Quiz[0];
        QuizPrompt.Text = q.Prompt;
        _correctIndex = q.CorrectIndex;
        Choice0.Source = q.ChoiceImages[0];
        Choice1.Source = q.ChoiceImages[1];
        Choice2.Source = q.ChoiceImages[2];

        _cts = new CancellationTokenSource();
        _ = TimerAsync(q.TimeLimitSec, _cts.Token);
    }

    async Task TimerAsync(int seconds, CancellationToken ct)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds), ct);
            if (!ct.IsCancellationRequested) await HandleWrongAsync();
        }
        catch (TaskCanceledException) { }
    }

    Task HandlePickAsync(int idx) => idx == _correctIndex ? HandleCorrectAsync() : HandleWrongAsync();

    async Task HandleCorrectAsync()
    {
        _cts?.Cancel();
        var reward = AlamatContent.GetStory(_storyId).RewardStars;
        AlamatContent.Stars += reward;
        await Navigation.PushAsync(new RewardPage(reward));
    }

    async Task HandleWrongAsync()
    {
        _cts?.Cancel();
        if (AlamatContent.Hearts > 0) AlamatContent.Hearts--;
        QuizHearts.Text = $"{AlamatContent.Hearts}";
        await DisplayAlert("Mali", "Panoorin muli ang kuwento.", "OK");
        await Navigation.PushAsync(new StoryPage(_storyId));
    }

    async void OnPick0(object? s, TappedEventArgs e) => await HandlePickAsync(0);
    async void OnPick1(object? s, TappedEventArgs e) => await HandlePickAsync(1);
    async void OnPick2(object? s, TappedEventArgs e) => await HandlePickAsync(2);
}
