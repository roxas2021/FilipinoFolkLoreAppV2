using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using System.Threading;
using FilipinoFolkloreApp.Services;
using Plugin.Maui.Audio;

namespace FilipinoFolkloreApp.Views;

public partial class StoryPage : ContentPage
{
    bool _isNavigatingToQuiz = false;

    private readonly string _storyId;
    int _idx = 0;
    bool _playing = true;
    CancellationTokenSource? _hudCts;
    CancellationTokenSource? _cts; readonly IAudioManager _audioManager = AudioManager.Current;
    private Stream? _audioStream;
    private IAudioPlayer? _player;

    public StoryPage(string storyId)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _storyId = storyId;
        NarratorAvatar.Source = AlamatContent.CurrentNarrator.Avatar;
        if (Application.Current is App app)
        {
            app.PauseBackgroundMusic();
        }
        _ = ShowSlideAsync(initial: true);
    }

    async Task ShowSlideAsync(bool initial = false)
    {
        var story = AlamatContent.GetStory(_storyId);

        if (_idx >= story.Slides.Count)
        {
            if (_isNavigatingToQuiz)
                return;
            _isNavigatingToQuiz = true;
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

        await SubtitleBar.FadeTo(0, 120);

        var front = BgA.Opacity >= BgB.Opacity ? BgA : BgB;
        var back = front == BgA ? BgB : BgA;
        back.Source = slide.Background;
        back.Opacity = initial ? 1 : 0;
        if (!initial)
            await Task.WhenAll(front.FadeTo(0, 250), back.FadeTo(1, 250));

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

        if (!string.IsNullOrWhiteSpace(slide.Subtitle))
        {
            SubtitleText.Text = slide.Subtitle!;
            await SubtitleBar.FadeTo(1, 150);
        }
        else
        {
            SubtitleText.Text = "";
            SubtitleBar.Opacity = 0;
        }

        StartForSlide(slide);
    }

    async void StartForSlide(AlamatContent.Slide slide)
    {
        _cts?.Cancel();

        _player?.Stop();
        _player?.Dispose();
        _player = null;

        _audioStream?.Dispose();
        _audioStream = null;

        var narratorId = AlamatContent.SelectedNarratorId;

        if (slide.AudioByNarrator != null &&
            slide.AudioByNarrator.TryGetValue(narratorId, out var relPath) &&
            !string.IsNullOrWhiteSpace(relPath))
        {
            try
            {
                _cts = new CancellationTokenSource();

                _audioStream = await FileSystem.OpenAppPackageFileAsync(relPath); _player = AudioManager.Current.CreatePlayer(_audioStream);
                _player.Volume = AlamatContent.NarratorVolume;
                _player.PlaybackEnded += async (s, e) =>
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3), _cts!.Token);

                        if (_cts!.IsCancellationRequested) return;

                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            _idx++;
                            await ShowSlideAsync();
                        });
                    }
                    catch (TaskCanceledException)
                    {
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
            StartAutoTimerIfPlaying();
        }
    }
    void OnScreenTapped(object? s, TappedEventArgs e)
    {
        _ = ShowControlsTemporarilyAsync();
    }

    async Task ShowControlsTemporarilyAsync()
    {
        _hudCts?.Cancel();
        _hudCts = new CancellationTokenSource();
        var ct = _hudCts.Token;

        ControlsPanel.InputTransparent = false;
        await ControlsPanel.FadeTo(1, 150);

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(2), ct);
            if (ct.IsCancellationRequested) return;

            await ControlsPanel.FadeTo(0, 150);
            ControlsPanel.InputTransparent = true;
        }
        catch (TaskCanceledException) { /* ignore */ }
    }
    protected override void OnDisappearing()
    {
        _isNavigatingToQuiz = true;
        base.OnDisappearing();

        _cts?.Cancel();

        _player?.Stop();
        _player?.Dispose();
        _player = null;

        _audioStream?.Dispose();
        _audioStream = null;
        if (Application.Current is App app)
        {
            app.UpdateBackgroundMusic(AlamatContent.MusicIsEnabled);
        }
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
            await ShowSlideAsync();
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
        if (_isNavigatingToQuiz)
            return;
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
