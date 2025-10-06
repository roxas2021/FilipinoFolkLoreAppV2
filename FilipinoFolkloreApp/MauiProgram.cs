using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // other configuration (services, handlers) here

            return builder.Build();
        }
    }
}
