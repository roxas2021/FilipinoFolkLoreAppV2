using System;
using System.Threading.Tasks;
using Plugin.Maui.Audio;

namespace FilipinoFolkloreApp.Services;

public class SoundService
{
    private readonly IAudioManager _audioManager;
    private IAudioPlayer? _buttonClickPlayer;
    private bool _isPlaying;

    public SoundService(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public async Task PlayButtonClickAsync()
    {
        // Prevent overlapping button click sounds
        if (_isPlaying)
            return;

        try
        {
            _isPlaying = true;

            // Create a new stream and player for each click (don't reuse)
            using var stream = await FileSystem.OpenAppPackageFileAsync("sounds/button_click.mp3");
            using var player = _audioManager.CreatePlayer(stream);

            // Set volume and play
            player.Volume = 0.5; // Adjust as needed
            player.Play();

            // Wait for the sound to finish (assuming short duration ~200-500ms)
            await Task.Delay(300); // Adjust based on your sound file duration
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to play button click sound: {ex.Message}");
        }
        finally
        {
            _isPlaying = false;
        }
    }

    // Alternative method if you want fire-and-forget (no waiting)
    public void PlayButtonClickFireAndForget()
    {
        _ = Task.Run(async () =>
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("sounds/button_click.mp3");
                using var player = _audioManager.CreatePlayer(stream);
                player.Volume = 1.0;
                player.Play();
                await Task.Delay(300);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to play button click sound: {ex.Message}");
            }
        });
    }
}
