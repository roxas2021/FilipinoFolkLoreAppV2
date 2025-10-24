using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views;

public partial class StoryPage : ContentPage
{
    private readonly string _storyId;
    int _idx = 0;
    bool _playing = true;
    CancellationTokenSource? _cts;

    public StoryPage(string storyId)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _storyId = storyId;
        NarratorAvatar.Source = AlamatContent.CurrentNarrator.Avatar;
        _ = ShowSlideAsync(initial: true);
        RestartAutoplay();
    }

    async Task ShowSlideAsync(bool initial = false)
    {
        var story = AlamatContent.GetStory(_storyId);
        if (_idx >= story.Slides.Count)
        {
            await Navigation.PushAsync(new QuizPage(_storyId));
            return;
        }
        if (_idx < 0) _idx = 0;

        var slide = story.Slides[_idx];

        // Hide subtitle until images are fully swapped
        await SubtitleBar.FadeTo(0, 120);

        // cross-fade background
        var front = BgA.Opacity >= BgB.Opacity ? BgA : BgB;
        var back = front == BgA ? BgB : BgA;
        back.Source = slide.Background;
        back.Opacity = initial ? 1 : 0;
        if (!initial)
            await Task.WhenAll(front.FadeTo(0, 250), back.FadeTo(1, 250));

        // layered characters
        CharacterLayer.Children.Clear();
        foreach (var img in slide.Characters)
        {
            CharacterLayer.Children.Add(new Image
            {
                Source = img,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                HeightRequest = 240
            });
        }

        // Now reveal subtitle if present
        if (!string.IsNullOrWhiteSpace(slide.Subtitle))
        {
            SubtitleText.Text = slide.Subtitle!;
            await SubtitleBar.FadeTo(1, 150);
        }
        else
        {
            SubtitleText.Text = "";
            SubtitleBar.Opacity = 0; // stay hidden
        }
    }

    void RestartAutoplay()
    {
        _cts?.Cancel();
        if (!_playing) return;
        _cts = new CancellationTokenSource();
        _ = AutoAdvanceAsync(_cts.Token);
    }

    async Task AutoAdvanceAsync(CancellationToken ct)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(5), ct);
            if (ct.IsCancellationRequested) return;
            _idx++;
            await ShowSlideAsync();
            RestartAutoplay();
        }
        catch (TaskCanceledException) { }
    }

    async void OnPrev(object? s, TappedEventArgs e)
    {
        _idx--;
        await ShowSlideAsync();
        RestartAutoplay();
    }

    async void OnNext(object? s, TappedEventArgs e)
    {
        _idx++;
        await ShowSlideAsync();
        RestartAutoplay();
    }

    void OnTogglePlay(object? s, TappedEventArgs e)
    {
        _playing = !_playing;
        PlayIcon.Source = _playing ? "elements/pause.png" : "elements/play.png";
        if (_playing) RestartAutoplay(); else _cts?.Cancel();
    }
}
