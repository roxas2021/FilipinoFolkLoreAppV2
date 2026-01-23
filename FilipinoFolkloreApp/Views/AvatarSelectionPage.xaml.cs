using FilipinoFolkloreApp.Models;
using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views.Home;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FilipinoFolkloreApp.Views
{
    public partial class AvatarSelectionPage : ContentPage
    {
        public ObservableCollection<Avatar> Avatars { get; } = new()
        {
            new Avatar("avatarcustomization/avatar1/avatar1.png"),
            new Avatar("avatarcustomization/avatar2/avatar2.png"),
            new Avatar("avatarcustomization/avatar3/avatar3.png"),
            new Avatar("avatarcustomization/avatar4/avatar4.png"),
        };

        public AvatarSelectionPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            AvatarGrid.ItemsSource = Avatars;
        }

        // Removed OnAppearing navigation logic - now handled in App.xaml.cs

        async void AvatarGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is not Avatar selected)
            {
                AvatarGrid.SelectedItem = null;
                return;
            }

            // Animation
            PointsOverlay.Scale = 0.9;
            PointsOverlay.Opacity = 0;
            PointsOverlay.IsVisible = true;

            await Task.WhenAll(
                PointsOverlay.FadeTo(1, 350),
                PointsOverlay.ScaleTo(1.05, 350, Easing.CubicOut)
            );

            await Task.Delay(2500);

            // Update character points
            var existingChar = await App.Database.GetCharAsync();
            if (existingChar != null)
            {
                existingChar.points = 50;
                await App.Database.UpdateCharAsync(existingChar);
            }

            // Save avatar set
            var fileName = Path.GetFileNameWithoutExtension(selected.ImageSource ?? "");
            var avatarId = string.IsNullOrWhiteSpace(fileName) ? "avatar1" : fileName;

            var set = new AvatarCostumeSet
            {
                avatarid = avatarId,
                avatarblueunlocked = false,
                avatarblueredunlocked = false,
                avatargreenunlocked = false,
                avatarpinkunlocked = false,
                avatarredunlocked = false,
            };

            await App.Database.SaveAvatarSetAsync(set);
            AvatarCustomizationHelper.SelectedAvatarSetId = set.avatarid;
            CharacterHelper.CurrentAvatar = AvatarCustomizationHelper.GetFirstCostumePathOrDefault(set.avatarid);
            await App.Database.UpdateCurrentAvatarAsync(CharacterHelper.CurrentAvatar);

            // Navigate to IndexPage
            await Navigation.PushAsync(new IndexPage());
            Navigation.RemovePage(this);

            AvatarGrid.SelectedItem = null;
        }
    }

    public class Avatar
    {
        public string ImageSource { get; }
        public Avatar(string imageSource) => ImageSource = imageSource;
    }
}