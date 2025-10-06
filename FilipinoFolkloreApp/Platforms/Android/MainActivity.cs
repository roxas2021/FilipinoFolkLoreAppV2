using Android.App;
using Android.Content.PM;
using Android.OS;

namespace FilipinoFolkloreApp
{
    [Activity(Theme = "@style/Maui.SplashTheme",
        MainLauncher = true, 
        LaunchMode = LaunchMode.SingleTop, 
        ConfigurationChanges = ConfigChanges.ScreenSize 
        | ConfigChanges.Orientation 
        | ConfigChanges.UiMode 
        | ConfigChanges.ScreenLayout
        | ConfigChanges.Density
        | ConfigChanges.SmallestScreenSize, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}
