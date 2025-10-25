using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using System.Threading;
using FilipinoFolkloreApp.Services;
using Plugin.Maui.Audio;

namespace FilipinoFolkloreApp.Views;

public partial class StoryPage : ContentPage
{
    private readonly string _storyId;
    int _idx = 0;
    bool _playing = true;
    CancellationTokenSource? _hudCts;
    CancellationTokenSource? _cts;                // used for both autoplay and post-audio delay
    readonly IAudioManager _audioManager = AudioManager.Current;
    private Stream? _audioStream;
    private IAudioPlayer? _player;

    public StoryPage(string storyId)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _storyId = storyId;
        NarratorAvatar.Source = AlamatContent.CurrentNarrator.Avatar;

        // Start first slide (no timer yet; StartForSlide will decide)
        _ = ShowSlideAsync(initial: true);
    }

    async Task ShowSlideAsync(bool initial = false)
    {
        var story = AlamatContent.GetStory(_storyId);

        // If past the last slide, stop timers/audio and go to quiz.
        if (_idx >= story.Slides.Count)
        {
            _cts?.Cancel();
            _player?.Stop();
            _player?.Dispose();
            _player = null;

            _audioStream?.Dispose();
            _audioStream = null;

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

        // Reveal subtitle if present
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

        // Start audio or fallback timer for this slide
        StartForSlide(slide);
    }

    async void StartForSlide(AlamatContent.Slide slide)
    {
        // Cancel any prior timer/delay
        _cts?.Cancel();

        // Stop and dispose previous audio
        _player?.Stop();
        _player?.Dispose();
        _player = null;

        // Dispose previous stream
        _audioStream?.Dispose();
        _audioStream = null;

        var narratorId = AlamatContent.SelectedNarratorId;

        if (slide.AudioByNarrator != null &&
            slide.AudioByNarrator.TryGetValue(narratorId, out var relPath) &&
            !string.IsNullOrWhiteSpace(relPath))
        {
            try
            {
                // New CTS for this slide (used for post-audio delay, too)
                _cts = new CancellationTokenSource();

                // relPath like "audio/juantamad/scene1_tarsier.mp3"
                _audioStream = await FileSystem.OpenAppPackageFileAsync(relPath); // keep it alive
                _player = AudioManager.Current.CreatePlayer(_audioStream);
                _player.Volume = 1.0; // ensure not muted

                _player.PlaybackEnded += async (s, e) =>
                {
                    try
                    {
                        // Wait 3 seconds AFTER audio finishes before advancing
                        await Task.Delay(TimeSpan.FromSeconds(3), _cts!.Token);

                        if (_cts!.IsCancellationRequested) return;

                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            _idx++;
                            await ShowSlideAsync(); // schedules next slide (audio or timer)
                        });
                    }
                    catch (TaskCanceledException)
                    {
                        // ignored: slide changed or page left
                    }
                };

                if (_playing)
                    _player.Play();
            }
            catch
            {
                StartAutoTimerIfPlaying();
            }
        }
        else
        {
            // No audio for this slide → use your 5s autoplay
            StartAutoTimerIfPlaying();
        }
    }
    void OnScreenTapped(object? s, TappedEventArgs e)
    {
        _ = ShowControlsTemporarilyAsync();
    }

    async Task ShowControlsTemporarilyAsync()
    {
        // cancel any pending hide
        _hudCts?.Cancel();
        _hudCts = new CancellationTokenSource();
        var ct = _hudCts.Token;

        // show (fade in) and allow interaction
        ControlsPanel.InputTransparent = false;
        await ControlsPanel.FadeTo(1, 150);

        try
        {
            // wait 2s of inactivity
            await Task.Delay(TimeSpan.FromSeconds(2), ct);
            if (ct.IsCancellationRequested) return;

            // hide (fade out) and block interaction
            await ControlsPanel.FadeTo(0, 150);
            ControlsPanel.InputTransparent = true;
        }
        catch (TaskCanceledException) { /* ignore */ }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _cts?.Cancel();

        _player?.Stop();
        _player?.Dispose();
        _player = null;

        _audioStream?.Dispose();
        _audioStream = null;
    }

    void StartAutoTimerIfPlaying()
    {
        if (!_playing) return;

        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _ = AutoAdvanceAsync(_cts.Token);
    }

    void StopAudio()
    {
        _player?.Stop();
        _player?.Dispose();
        _player = null;

        _audioStream?.Dispose();
        _audioStream = null;
    }

    async Task AutoAdvanceAsync(CancellationToken ct)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(5), ct);
            if (ct.IsCancellationRequested) return;

            _idx++;
            await ShowSlideAsync(); // ShowSlideAsync will schedule audio/timer for the new slide
        }
        catch (TaskCanceledException) { }
    }

    async void OnPrev(object? s, TappedEventArgs e)
    {
        _cts?.Cancel();
        StopAudio();
        _idx--;
        await ShowSlideAsync();
    }

    async void OnNext(object? s, TappedEventArgs e)
    {
        _ = ShowControlsTemporarilyAsync();
        _cts?.Cancel();
        StopAudio();
        _idx++;
        await ShowSlideAsync();
    }

    void OnTogglePlay(object? s, TappedEventArgs e)
    {
        _ = ShowControlsTemporarilyAsync();
        _playing = !_playing;
        PlayIcon.Source = _playing ? "elements/pause.png" : "elements/play.png";

        if (_playing)
        {
            if (_player != null) _player.Play();
            else StartAutoTimerIfPlaying();
        }
        else
        {
            _cts?.Cancel();
            _player?.Pause();
        }
    }
}
