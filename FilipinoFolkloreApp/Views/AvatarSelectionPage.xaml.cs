using FilipinoFolkloreApp.Models;
using FilipinoFolkloreApp.Views.Home;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace FilipinoFolkloreApp.Views
{
    public partial class AvatarSelectionPage : ContentPage
    {
        public ObservableCollection<Avatar> Avatars { get; } = new()
        {
            new Avatar("avatar/avatar1.png"),
            new Avatar("avatar/avatar2.png"),
            new Avatar("avatar/avatar3.png"),
            new Avatar("avatar/avatar4.png"),
        };

        public AvatarSelectionPage()
        {
            InitializeComponent();
            AvatarGrid.ItemsSource = Avatars;
        }

        async void AvatarGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Avatar selected)
            {
                PointsOverlay.Scale = 0.9;
                PointsOverlay.Opacity = 0;
                PointsOverlay.IsVisible = true;

                await Task.WhenAll(
                    PointsOverlay.FadeTo(1, 350),
                    PointsOverlay.ScaleTo(1.05, 350, Easing.CubicOut)
                );

                await Task.Delay(2500);

                var existingChar = await App.Database.GetCharAsync();
                if (existingChar != null)
                {
                    existingChar.points = 50;
                    await App.Database.UpdateCharAsync(existingChar);
                }

                await Navigation.PushAsync(new IndexPage());
            }
        }
    }

    public class Avatar
    {
        public string ImageSource { get; }
        public Avatar(string imageSource) => ImageSource = imageSource;
    }
}