using Microsoft.Maui.Controls;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views.Home;

namespace FilipinoFolkloreApp.Views;

public partial class ColoringPage : ContentPage
{
    private SKBitmap? _coloringBitmap;
    private SKColor _selectedColor = SKColors.Red;
    private readonly string _templatePath;
    private TaskCompletionSource<bool>? _alertTcs;

    private class UndoState
    {
        public SKBitmap BitmapSnapshot { get; set; } = null!;
    }

    private readonly Stack<UndoState> _undoStack = new();
    private const int MAX_UNDO_STATES = 10;
    private float _baseScale = 1f;
    private float _currentScale = 1f;
    private float _minScale = 0.5f;
    private float _maxScale = 5f;
    private SKPoint _offset = SKPoint.Empty;
    private SKPoint _panOffset = SKPoint.Empty;
    private SKPoint _lastPanPoint = SKPoint.Empty;
    private bool _isPanning = false;

    private readonly Dictionary<long, SKPoint> _touchPoints = new();
    private float _previousPinchDistance = 0f;
    private SKPoint _pinchCenter = SKPoint.Empty;

    private const string COLORING_COUNT_KEY = "ColoringCompletedCount";

    private const int MAX_PIXELS_TO_FILL = 500000;
    private const int COLOR_MATCH_THRESHOLD = 15;

    private SoundService SoundService =>
        Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;

    private record ColorInfo(string Name, SKColor SkColor);

    private readonly List<ColorInfo> _colors = new()
    {
        new ColorInfo("Pula", SKColors.Red),
        new ColorInfo("Asul", SKColors.Blue),
        new ColorInfo("Dilaw", SKColors.Yellow),
        new ColorInfo("Berde", SKColors.Green),
        new ColorInfo("Kahel", SKColors.Orange),
        new ColorInfo("Lila", SKColors.Purple),
        new ColorInfo("Kayumanggi", new SKColor(139, 69, 19)),
        new ColorInfo("Rosas", SKColors.Pink),
        new ColorInfo("Itim", SKColors.Black),
        new ColorInfo("Puti", SKColors.White),
        new ColorInfo("Kulay-Ginto", SKColors.Gold),
        new ColorInfo("Kulay-Pilak", SKColors.Silver),
        new ColorInfo("Abo", SKColors.Gray),
        new ColorInfo("Tsokolate", new SKColor(101, 67, 33)),
        new ColorInfo("Luntiang-Dahon", new SKColor(34, 139, 34))
    };

    private class ColoringMilestone
    {
        public int RequiredCount { get; set; }
        public int RewardStars { get; set; }
        public int MedalId { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    private readonly List<ColoringMilestone> _milestones = new()
    {
        new ColoringMilestone
        {
            RequiredCount = 5,
            RewardStars = 50,
            MedalId = 20,
            Description = "Nakumpleto ang 5 coloring activity!"
        },
        new ColoringMilestone
        {
            RequiredCount = 10,
            RewardStars = 75,
            MedalId = 21,
            Description = "Nakumpleto ang 10 coloring activity!"
        },
        new ColoringMilestone
        {
            RequiredCount = 15,
            RewardStars = 100,
            MedalId = 22,
            Description = "Nakumpleto ang lahat ng coloring activity!"
        }
    };

    public ColoringPage(string templateImagePath)
    {
        InitializeComponent();
        _templatePath = templateImagePath;
        NavigationPage.SetHasNavigationBar(this, false);
        LoadHUD();
        InitializeColorPalette();
        _ = LoadColoringImageAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadHUD();
    }

    private void LoadHUD()
    {
        HudAvatar.Source = CharacterHelper.CurrentAvatar;
        PlayerNameLabel.Text = CharacterHelper.CurrentName;
        StarsLabel.Text = CharacterHelper.CurrentStars.ToString();
        RefreshHearts();
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

    private void InitializeColorPalette()
    {
        foreach (var colorInfo in _colors)
        {
            var colorFrame = new Frame
            {
                WidthRequest = 60,
                HeightRequest = 60,
                CornerRadius = 30,
                Padding = 0,
                HasShadow = true,
                BackgroundColor = Color.FromRgba(colorInfo.SkColor.Red,
                                                 colorInfo.SkColor.Green,
                                                 colorInfo.SkColor.Blue,
                                                 colorInfo.SkColor.Alpha),
                BorderColor = Colors.White,
                Content = new BoxView
                {
                    BackgroundColor = Color.FromRgba(colorInfo.SkColor.Red,
                                                    colorInfo.SkColor.Green,
                                                    colorInfo.SkColor.Blue,
                                                    colorInfo.SkColor.Alpha),
                    CornerRadius = 30
                }
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => OnColorSelected(colorInfo.SkColor, colorFrame);
            colorFrame.GestureRecognizers.Add(tapGesture);

            ColorPalette.Children.Add(colorFrame);
        }

        if (ColorPalette.Children.Count > 0 && ColorPalette.Children[0] is Frame firstFrame)
        {
            firstFrame.BorderColor = Colors.Gold;
            firstFrame.Scale = 1.1;
        }
    }

    private async Task LoadColoringImageAsync()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            System.Diagnostics.Debug.WriteLine($"Attempting to load: {_templatePath}");

            using var stream = await FileSystem.OpenAppPackageFileAsync(_templatePath);
            var originalBitmap = SKBitmap.Decode(stream);

            if (originalBitmap == null)
            {
                await ShowGameAlertAsync("Cannot decode bitmap", false);
                return;
            }

            _coloringBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height, SKColorType.Rgba8888, SKAlphaType.Premul);

            using (var canvas = new SKCanvas(_coloringBitmap))
            {
                canvas.Clear(SKColors.White);
                canvas.DrawBitmap(originalBitmap, 0, 0);
            }

            originalBitmap.Dispose();

            System.Diagnostics.Debug.WriteLine($"Successfully loaded bitmap: {_coloringBitmap.Width}x{_coloringBitmap.Height}, ColorType: {_coloringBitmap.ColorType}");

            ColoringCanvas.InvalidateSurface();
        }
        catch (FileNotFoundException ex)
        {
            System.Diagnostics.Debug.WriteLine($"File not found: {ex.Message}");
            await ShowGameAlertAsync($"File not found: {_templatePath}", false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading image: {ex}");
            await ShowGameAlertAsync($"Cannot load image: {ex.Message}", false);
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private void SaveUndoState()
    {
        if (_coloringBitmap == null) return;

        var snapshot = _coloringBitmap.Copy();
        if (snapshot == null) return;

        var undoState = new UndoState
        {
            BitmapSnapshot = snapshot
        };

        _undoStack.Push(undoState);

        if (_undoStack.Count > MAX_UNDO_STATES)
        {
            var stackList = _undoStack.ToList();
            var oldestState = stackList[stackList.Count - 1];
            oldestState.BitmapSnapshot?.Dispose();

            _undoStack.Clear();
            for (int i = stackList.Count - 2; i >= 0; i--)
            {
                _undoStack.Push(stackList[i]);
            }
        }

        UpdateUndoButtonState();
    }

    private void UpdateUndoButtonState()
    {
        UndoButton.IsEnabled = _undoStack.Count > 0;
        UndoButton.Opacity = _undoStack.Count > 0 ? 1.0 : 0.5;
    }

    private async void OnUndoClicked(object? sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();

        if (_undoStack.Count == 0 || _coloringBitmap == null)
            return;

        var undoState = _undoStack.Pop();

        _coloringBitmap.Dispose();
        _coloringBitmap = undoState.BitmapSnapshot;

        ColoringCanvas.InvalidateSurface();
        UpdateUndoButtonState();

        System.Diagnostics.Debug.WriteLine($"Undo performed. Remaining undo states: {_undoStack.Count}");
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        if (_coloringBitmap != null)
        {
            var info = e.Info;

            if (_baseScale == 1f)
            {
                float scaleX = (float)info.Width / _coloringBitmap.Width;
                float scaleY = (float)info.Height / _coloringBitmap.Height;
                _baseScale = Math.Min(scaleX, scaleY) * 0.9f;
                _currentScale = _baseScale;
            }

            float baseX = (info.Width - _coloringBitmap.Width * _currentScale) / 2;
            float baseY = (info.Height - _coloringBitmap.Height * _currentScale) / 2;

            float x = baseX + _panOffset.X;
            float y = baseY + _panOffset.Y;
            _offset = new SKPoint(x, y);

            var destRect = new SKRect(
    x,
    y,
    x + _coloringBitmap.Width * _currentScale,
    y + _coloringBitmap.Height * _currentScale
);

            canvas.DrawBitmap(_coloringBitmap, destRect);
        }
    }

    private void OnCanvasTouch(object? sender, SKTouchEventArgs e)
    {
        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                _touchPoints[e.Id] = e.Location;

                if (_touchPoints.Count == 1)
                {
                    _lastPanPoint = e.Location;
                    _isPanning = false;
                }
                else if (_touchPoints.Count == 2)
                {
                    var points = new List<SKPoint>(_touchPoints.Values);
                    _previousPinchDistance = SKPoint.Distance(points[0], points[1]);
                    _pinchCenter = new SKPoint((points[0].X + points[1].X) / 2, (points[0].Y + points[1].Y) / 2);
                    _isPanning = true;
                }
                break;

            case SKTouchAction.Moved:
                _touchPoints[e.Id] = e.Location;

                if (_touchPoints.Count == 1)
                {
                    HandlePan(e.Location);
                }
                else if (_touchPoints.Count == 2)
                {
                    HandlePinchZoom();
                }
                break;

            case SKTouchAction.Released:
                if (!_isPanning && _touchPoints.Count == 1 && _coloringBitmap != null)
                {
                    int x = (int)((e.Location.X - _offset.X) / _currentScale);
                    int y = (int)((e.Location.Y - _offset.Y) / _currentScale);

                    System.Diagnostics.Debug.WriteLine($"Touch at screen ({e.Location.X}, {e.Location.Y}) -> bitmap ({x}, {y})");
                    System.Diagnostics.Debug.WriteLine($"Selected color: {_selectedColor}");

                    _ = FloodFillAsync(x, y, _selectedColor);
                }

                _touchPoints.Remove(e.Id);

                if (_touchPoints.Count == 0)
                {
                    _isPanning = false;
                    _previousPinchDistance = 0f;
                }
                else if (_touchPoints.Count == 1)
                {
                    _previousPinchDistance = 0f;
                    var remainingPoint = _touchPoints.Values.First();
                    _lastPanPoint = remainingPoint;
                }
                break;

            case SKTouchAction.Cancelled:
                _touchPoints.Remove(e.Id);
                if (_touchPoints.Count == 0)
                {
                    _isPanning = false;
                    _previousPinchDistance = 0f;
                }
                break;
        }

        e.Handled = true;
        ColoringCanvas.InvalidateSurface();
    }

    private void HandlePan(SKPoint currentPoint)
    {
        float deltaX = currentPoint.X - _lastPanPoint.X;
        float deltaY = currentPoint.Y - _lastPanPoint.Y;

        if (Math.Abs(deltaX) > 5 || Math.Abs(deltaY) > 5)
        {
            _isPanning = true;
        }

        if (_isPanning)
        {
            _panOffset = new SKPoint(_panOffset.X + deltaX, _panOffset.Y + deltaY);
        }

        _lastPanPoint = currentPoint;
    }

    private void HandlePinchZoom()
    {
        if (_touchPoints.Count != 2) return;

        var points = new List<SKPoint>(_touchPoints.Values);
        var point1 = points[0];
        var point2 = points[1];

        float currentDistance = SKPoint.Distance(point1, point2);

        if (_previousPinchDistance <= 0)
        {
            _previousPinchDistance = currentDistance;
            return;
        }

        var centerX = (point1.X + point2.X) / 2;
        var centerY = (point1.Y + point2.Y) / 2;
        var center = new SKPoint(centerX, centerY);

        float scaleFactor = currentDistance / _previousPinchDistance;

        float newScale = _currentScale * scaleFactor;
        newScale = Math.Max(_minScale * _baseScale, Math.Min(_maxScale * _baseScale, newScale));

        if (newScale != _currentScale)
        {
            float actualScaleFactor = newScale / _currentScale;

            float bitmapX = (center.X - _offset.X) / _currentScale;
            float bitmapY = (center.Y - _offset.Y) / _currentScale;

            float newOffsetX = center.X - bitmapX * newScale;
            float newOffsetY = center.Y - bitmapY * newScale;

            var info = ColoringCanvas.CanvasSize;
            float baseX = (info.Width - _coloringBitmap!.Width * newScale) / 2;
            float baseY = (info.Height - _coloringBitmap.Height * newScale) / 2;

            _panOffset = new SKPoint(newOffsetX - baseX, newOffsetY - baseY);
            _currentScale = newScale;
        }

        _previousPinchDistance = currentDistance;
    }

    private async Task FloodFillAsync(int x, int y, SKColor fillColor)
    {
        if (_coloringBitmap == null) return;

        if (x < 0 || x >= _coloringBitmap.Width || y < 0 || y >= _coloringBitmap.Height)
        {
            System.Diagnostics.Debug.WriteLine($"FloodFill: Out of bounds ({x}, {y})");
            return;
        }

        SKColor targetColor = _coloringBitmap.GetPixel(x, y);

        if (IsBlackOutline(targetColor))
        {
            System.Diagnostics.Debug.WriteLine("Clicked on black outline, skipping fill");
            return;
        }

        if (ColorsAreIdentical(targetColor, fillColor))
        {
            System.Diagnostics.Debug.WriteLine("Already same color, skipping fill");
            return;
        }

        SaveUndoState();

        await Task.Run(() => FloodFill(x, y, fillColor));

        MainThread.BeginInvokeOnMainThread(() =>
{
    ColoringCanvas.InvalidateSurface();
});
    }

    private void FloodFill(int x, int y, SKColor fillColor)
    {
        if (_coloringBitmap == null) return;

        SKColor targetColor = _coloringBitmap.GetPixel(x, y);

        System.Diagnostics.Debug.WriteLine($"Target color at ({x},{y}): {targetColor}");

        int pixelsFilled = 0;
        int width = _coloringBitmap.Width;
        int height = _coloringBitmap.Height;

        HashSet<int> visited = new HashSet<int>();

        Queue<SKPointI> pixels = new Queue<SKPointI>();
        pixels.Enqueue(new SKPointI(x, y));

        int PointToKey(int px, int py) => py * width + px;

        while (pixels.Count > 0 && pixelsFilled < MAX_PIXELS_TO_FILL)
        {
            var point = pixels.Dequeue();

            if (point.X < 0 || point.X >= width || point.Y < 0 || point.Y >= height)
                continue;

            int key = PointToKey(point.X, point.Y);
            if (visited.Contains(key))
                continue;

            visited.Add(key);

            SKColor currentColor = _coloringBitmap.GetPixel(point.X, point.Y);

            if (ColorsMatch(currentColor, targetColor) && !IsBlackOutline(currentColor))
            {
                _coloringBitmap.SetPixel(point.X, point.Y, fillColor);
                pixelsFilled++;

                pixels.Enqueue(new SKPointI(point.X + 1, point.Y));
                pixels.Enqueue(new SKPointI(point.X - 1, point.Y));
                pixels.Enqueue(new SKPointI(point.X, point.Y + 1));
                pixels.Enqueue(new SKPointI(point.X, point.Y - 1));
            }
        }

        if (pixelsFilled >= MAX_PIXELS_TO_FILL)
        {
            System.Diagnostics.Debug.WriteLine($"FloodFill reached maximum pixel limit: {MAX_PIXELS_TO_FILL}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"FloodFill completed: {pixelsFilled} pixels filled with {fillColor}");
        }
    }

    private bool IsBlackOutline(SKColor color)
    {
        return color.Red < 50 && color.Green < 50 && color.Blue < 50;
    }

    private bool ColorsMatch(SKColor color1, SKColor color2)
    {
        return Math.Abs(color1.Red - color2.Red) < COLOR_MATCH_THRESHOLD &&
       Math.Abs(color1.Green - color2.Green) < COLOR_MATCH_THRESHOLD &&
       Math.Abs(color1.Blue - color2.Blue) < COLOR_MATCH_THRESHOLD;
    }

    private bool ColorsAreIdentical(SKColor color1, SKColor color2)
    {
        return color1.Red == color2.Red &&
       color1.Green == color2.Green &&
       color1.Blue == color2.Blue &&
       color1.Alpha == color2.Alpha;
    }

    private void OnColorSelected(SKColor color, Frame selectedFrame)
    {
        _selectedColor = color;

        System.Diagnostics.Debug.WriteLine($"Color selected: {color}");

        foreach (var child in ColorPalette.Children)
        {
            if (child is Frame frame)
            {
                frame.BorderColor = Colors.White;
                frame.Scale = 1.0;
            }
        }

        selectedFrame.BorderColor = Colors.Gold;
        selectedFrame.Scale = 1.1;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        await SoundService.PlayButtonClickAsync();

        if (_coloringBitmap == null)
        {
            await ShowGameAlertAsync("Walang larawan na i-save", false);
            return;
        }

        try
        {
            SaveButton.IsEnabled = false;
            SaveButton.Text = "Nag-se-save...";

            var folderPath = Path.Combine(FileSystem.AppDataDirectory, "ColoredImages");
            Directory.CreateDirectory(folderPath);

            var fileName = $"colored_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = File.OpenWrite(filePath))
            {
                _coloringBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            }

            int currentCount = Preferences.Get(COLORING_COUNT_KEY, 0);
            currentCount++;
            Preferences.Set(COLORING_COUNT_KEY, currentCount);

            var milestone = _milestones.FirstOrDefault(m => m.RequiredCount == currentCount);

            if (milestone != null)
            {
                string milestoneKey = $"Coloring_Milestone_{milestone.RequiredCount}";
                bool alreadyClaimed = Preferences.Get(milestoneKey, false);

                if (!alreadyClaimed)
                {
                    await App.Database.UnlockMedalAsync(milestone.MedalId);

                    await ShowGameAlertAsync($"Tagumpay na na-save ang larawan", false);

                    await Navigation.PushAsync(new ColoringRewardPage(
    stars: milestone.RewardStars,
    medalId: milestone.MedalId,
    rewardKey: milestoneKey,
    returnPageType: "ColoringSelection",
    returnPageParameter: null
));
                    return;
                }
            }

            await ShowGameAlertAsync($"Tagumpay na na-save ang larawan", false);
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await ShowGameAlertAsync($"Hindi ma-save ang larawan: {ex.Message}", false);
        }
        finally
        {
            SaveButton.IsEnabled = true;
            SaveButton.Text = "I-Save";
        }
    }

    private Task<bool> ShowGameAlertAsync(string message, bool showYesNo = false)
    {
        if (GameAlertOverlay.IsVisible && _alertTcs != null)
            return _alertTcs.Task;

        _alertTcs = new TaskCompletionSource<bool>();

        AlertMessageLabel.Text = message;

        AlertButtonsPanel.Children.Clear();

        if (showYesNo)
        {
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

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await Navigation.PopAsync();
    }

    private async void OnHomeTapped(object? sender, TappedEventArgs e)
    {
        await SoundService.PlayButtonClickAsync();

        var pages = Navigation.NavigationStack.ToList();
        foreach (var page in pages)
        {
            if (page is ColoringSelectionPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is ColoringCollectionPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is ColoringPage)
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
}