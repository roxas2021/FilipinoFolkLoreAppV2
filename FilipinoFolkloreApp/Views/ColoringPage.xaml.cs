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

    // Zoom and Pan variables
    private float _baseScale = 1f;
    private float _currentScale = 1f;
    private float _minScale = 0.5f;
    private float _maxScale = 5f;
    private SKPoint _offset = SKPoint.Empty;
    private SKPoint _panOffset = SKPoint.Empty;
    private SKPoint _lastPanPoint = SKPoint.Empty;
    private bool _isPanning = false;

    // Multi-touch tracking
    private readonly Dictionary<long, SKPoint> _touchPoints = new();
    private float _previousPinchDistance = 0f;
    private SKPoint _pinchCenter = SKPoint.Empty;

    // Track if this is the first image saved
    private const string FIRST_IMAGE_SAVED_KEY = "FirstColoredImageSaved";

    // Flood fill safety limits
    private const int MAX_PIXELS_TO_FILL = 500000; // Maximum pixels per flood fill operation
    private const int COLOR_MATCH_THRESHOLD = 15; // Reduced from 30 for tighter edge detection

    // Filipino color palette
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

        // Highlight first color
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

            // FORCE CONVERT TO RGBA8888 to ensure colors work properly
            _coloringBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height, SKColorType.Rgba8888, SKAlphaType.Premul);

            using (var canvas = new SKCanvas(_coloringBitmap))
            {
                canvas.Clear(SKColors.White); // Ensure white background
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

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        if (_coloringBitmap != null)
        {
            var info = e.Info;

            // Calculate base scale to fit canvas (only once)
            if (_baseScale == 1f)
            {
                float scaleX = (float)info.Width / _coloringBitmap.Width;
                float scaleY = (float)info.Height / _coloringBitmap.Height;
                _baseScale = Math.Min(scaleX, scaleY) * 0.9f; // 90% to add padding
                _currentScale = _baseScale;
            }

            // Calculate centered base offset
            float baseX = (info.Width - _coloringBitmap.Width * _currentScale) / 2;
            float baseY = (info.Height - _coloringBitmap.Height * _currentScale) / 2;

            // Apply pan offset
            float x = baseX + _panOffset.X;
            float y = baseY + _panOffset.Y;
            _offset = new SKPoint(x, y);

            // Create destination rectangle
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

                // Single touch - potential tap for coloring
                if (_touchPoints.Count == 1)
                {
                    _lastPanPoint = e.Location;
                    _isPanning = false;
                }
                else if (_touchPoints.Count == 2)
                {
                    // Initialize pinch gesture
                    var points = new List<SKPoint>(_touchPoints.Values);
                    _previousPinchDistance = SKPoint.Distance(points[0], points[1]);
                    _pinchCenter = new SKPoint((points[0].X + points[1].X) / 2, (points[0].Y + points[1].Y) / 2);
                    _isPanning = true; // Prevent tap action during pinch
                }
                break;

            case SKTouchAction.Moved:
                _touchPoints[e.Id] = e.Location;

                if (_touchPoints.Count == 1)
                {
                    // Single finger pan
                    HandlePan(e.Location);
                }
                else if (_touchPoints.Count == 2)
                {
                    // Two finger pinch zoom
                    HandlePinchZoom();
                }
                break;

            case SKTouchAction.Released:
                // Check if it was a tap (not a pan) and single touch
                if (!_isPanning && _touchPoints.Count == 1 && _coloringBitmap != null)
                {
                    // Convert screen coordinates to bitmap coordinates
                    int x = (int)((e.Location.X - _offset.X) / _currentScale);
                    int y = (int)((e.Location.Y - _offset.Y) / _currentScale);

                    System.Diagnostics.Debug.WriteLine($"Touch at screen ({e.Location.X}, {e.Location.Y}) -> bitmap ({x}, {y})");
                    System.Diagnostics.Debug.WriteLine($"Selected color: {_selectedColor}");

                    // Perform flood fill asynchronously to prevent UI blocking
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
                    // Reset to single touch mode
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

        // If moved more than threshold, it's a pan not a tap
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

        // Calculate current distance between touch points
        float currentDistance = SKPoint.Distance(point1, point2);

        // Only process if we have a previous distance to compare
        if (_previousPinchDistance <= 0)
        {
            _previousPinchDistance = currentDistance;
            return;
        }

        // Calculate center point of the pinch
        var centerX = (point1.X + point2.X) / 2;
        var centerY = (point1.Y + point2.Y) / 2;
        var center = new SKPoint(centerX, centerY);

        // Calculate scale factor based on distance change
        float scaleFactor = currentDistance / _previousPinchDistance;

        // Apply scale limits
        float newScale = _currentScale * scaleFactor;
        newScale = Math.Max(_minScale * _baseScale, Math.Min(_maxScale * _baseScale, newScale));

        // Adjust pan offset to zoom towards pinch center point
        if (newScale != _currentScale)
        {
            float actualScaleFactor = newScale / _currentScale;

            // Calculate the point in bitmap coordinates that should stay under the center
            float bitmapX = (center.X - _offset.X) / _currentScale;
            float bitmapY = (center.Y - _offset.Y) / _currentScale;

            // Calculate new offset to keep that point under the center
            float newOffsetX = center.X - bitmapX * newScale;
            float newOffsetY = center.Y - bitmapY * newScale;

            // Update pan offset (considering base offset)
            var info = ColoringCanvas.CanvasSize;
            float baseX = (info.Width - _coloringBitmap!.Width * newScale) / 2;
            float baseY = (info.Height - _coloringBitmap.Height * newScale) / 2;

            _panOffset = new SKPoint(newOffsetX - baseX, newOffsetY - baseY);
            _currentScale = newScale;
        }

        // Update previous distance for next frame
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

        // Run flood fill on background thread to prevent UI freezing
        await Task.Run(() => FloodFill(x, y, fillColor));

        // Update UI on main thread
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

        // Don't fill if clicking on outline or already same color
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

        int pixelsFilled = 0;
        int width = _coloringBitmap.Width;
        int height = _coloringBitmap.Height;

        // Use HashSet to track visited pixels and prevent reprocessing
        HashSet<int> visited = new HashSet<int>();

        // Use Queue instead of Stack for better memory management (BFS instead of DFS)
        Queue<SKPointI> pixels = new Queue<SKPointI>();
        pixels.Enqueue(new SKPointI(x, y));

        // Helper function to convert point to unique int
        int PointToKey(int px, int py) => py * width + px;

        while (pixels.Count > 0 && pixelsFilled < MAX_PIXELS_TO_FILL)
        {
            var point = pixels.Dequeue();

            // Check bounds
            if (point.X < 0 || point.X >= width || point.Y < 0 || point.Y >= height)
                continue;

            // Check if already visited
            int key = PointToKey(point.X, point.Y);
            if (visited.Contains(key))
                continue;

            visited.Add(key);

            SKColor currentColor = _coloringBitmap.GetPixel(point.X, point.Y);

            // Check if pixel should be filled
            if (ColorsMatch(currentColor, targetColor) && !IsBlackOutline(currentColor))
            {
                _coloringBitmap.SetPixel(point.X, point.Y, fillColor);
                pixelsFilled++;

                // Add neighboring pixels
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
        // Detect black or very dark colors (outlines)
        return color.Red < 50 && color.Green < 50 && color.Blue < 50;
    }

    private bool ColorsMatch(SKColor color1, SKColor color2)
    {
        // Reduced threshold for tighter edge detection (prevent bleeding)
        return Math.Abs(color1.Red - color2.Red) < COLOR_MATCH_THRESHOLD &&
               Math.Abs(color1.Green - color2.Green) < COLOR_MATCH_THRESHOLD &&
               Math.Abs(color1.Blue - color2.Blue) < COLOR_MATCH_THRESHOLD;
    }

    private bool ColorsAreIdentical(SKColor color1, SKColor color2)
    {
        // Check if colors are exactly the same
        return color1.Red == color2.Red &&
               color1.Green == color2.Green &&
               color1.Blue == color2.Blue &&
               color1.Alpha == color2.Alpha;
    }

    private void OnColorSelected(SKColor color, Frame selectedFrame)
    {
        _selectedColor = color;

        System.Diagnostics.Debug.WriteLine($"Color selected: {color}");

        // Reset all frames
        foreach (var child in ColorPalette.Children)
        {
            if (child is Frame frame)
            {
                frame.BorderColor = Colors.White;
                frame.Scale = 1.0;
            }
        }

        // Highlight selected
        selectedFrame.BorderColor = Colors.Gold;
        selectedFrame.Scale = 1.1;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (_coloringBitmap == null)
        {
            await ShowGameAlertAsync("Walang larawan na i-save", false);
            return;
        }

        try
        {
            SaveButton.IsEnabled = false;
            SaveButton.Text = "Nag-se-save...";

            // Create folder in app data directory
            var folderPath = Path.Combine(FileSystem.AppDataDirectory, "ColoredImages");
            Directory.CreateDirectory(folderPath);

            // Generate filename
            var fileName = $"colored_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var filePath = Path.Combine(folderPath, fileName);

            // Save bitmap
            using (var stream = File.OpenWrite(filePath))
            {
                _coloringBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            }

            // Check if this is the first image saved
            bool isFirstImage = !Preferences.Get(FIRST_IMAGE_SAVED_KEY, false);

            if (isFirstImage)
            {
                // Mark that first image has been saved
                Preferences.Set(FIRST_IMAGE_SAVED_KEY, true);

                // Show success message first
                await ShowGameAlertAsync($"Tagumpay na na-save ang larawan", false);

                // Navigate to ColoringRewardPage
                await Navigation.PushAsync(new ColoringRewardPage(20, 21));
            }
            else
            {
                // Show normal success message
                await ShowGameAlertAsync($"Tagumpay na na-save ang larawan", false);
                await Navigation.PopAsync();
            }
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
        await HideGameAlertAsync(true);
    }

    private async void OnAlertYesClicked(object? sender, EventArgs e)
    {
        await HideGameAlertAsync(true);
    }

    private async void OnAlertNoClicked(object? sender, EventArgs e)
    {
        await HideGameAlertAsync(false);
    }

    private async void OnAlertBackgroundTapped(object? sender, EventArgs e)
    {
        await HideGameAlertAsync(false);
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnHomeTapped(object? sender, TappedEventArgs e)
    {
        var pages = Navigation.NavigationStack.ToList();
        foreach (var page in pages)
        {
            if (page is ColoringSelectionPage)
            {
                // Remove RewardPage from the stack
                Navigation.RemovePage(page);
            }
            if (page is ColoringCollectionPage)
            {
                // Remove RewardPage from the stack
                Navigation.RemovePage(page);
            }
            if (page is ColoringPage)
            {
                // Remove RewardPage from the stack
                Navigation.RemovePage(page);
            }
            if (page is MgaLaroPage)
            {
                // Remove RewardPage from the stack
                Navigation.RemovePage(page);
            }

        }

        await Navigation.PushAsync(new IndexPage());
    }

    private record ColorInfo(string Name, SKColor SkColor);
}