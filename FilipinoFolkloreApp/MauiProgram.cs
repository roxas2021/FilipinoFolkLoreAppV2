using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;    
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
                //.UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddSingleton(AudioManager.Current);
            // other configuration (services, handlers) here

            return builder.Build();

        }
    }
}
