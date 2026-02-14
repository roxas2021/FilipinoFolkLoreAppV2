using CommunityToolkit.Maui;
using FilipinoFolkloreApp.Services;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;    
using SkiaSharp.Views.Maui.Controls.Hosting;
namespace FilipinoFolkloreApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()                 // <-- first
                .UseMauiCommunityToolkit()         // <-- chained immediately after
                .UseSkiaSharp()
                //.UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddSingleton<HeartService>();
            builder.Services.AddSingleton<SoundService>();
            // other configuration (services, handlers) here

            return builder.Build();

        }
    }
}
