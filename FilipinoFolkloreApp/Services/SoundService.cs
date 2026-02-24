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
       
        if (_isPlaying)
            return;

        try
        {
            _isPlaying = true;

            using var stream = await FileSystem.OpenAppPackageFileAsync("sounds/button_click.mp3");
            using var player = _audioManager.CreatePlayer(stream);

            player.Volume = 1; 
            player.Play();

            await Task.Delay(300); 
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
