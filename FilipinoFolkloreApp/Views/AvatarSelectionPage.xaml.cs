using FilipinoFolkloreApp.Models;
using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views.Home;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.IO;                // for Path.GetFileNameWithoutExtension
using System.Linq;
using System.Threading.Tasks;

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

        // Called when the page appears - we check if there's already a saved AvatarCostumeSet
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // If a saved AvatarCostumeSet exists, skip this page and go to IndexPage
            var existing = await App.Database.GetAllAvatarSetsAsync();
            if (existing != null && existing.Count > 0)
            {
                AvatarCustomizationHelper.SelectedAvatarSetId =existing[0].avatarid;
                // Replace this page with IndexPage (prevents stacking this selection page)
                await Navigation.PushAsync(new IndexPage());
                Navigation.RemovePage(this);
                return;
            }
        }

        async void AvatarGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // guard
            if (e.CurrentSelection.FirstOrDefault() is not Avatar selected)
            {
                AvatarGrid.SelectedItem = null;
                return;
            }

            // animation (kept from your original)
            PointsOverlay.Scale = 0.9;
            PointsOverlay.Opacity = 0;
            PointsOverlay.IsVisible = true;

            await Task.WhenAll(
                PointsOverlay.FadeTo(1, 350),
                PointsOverlay.ScaleTo(1.05, 350, Easing.CubicOut)
            );

            await Task.Delay(2500);

            // update existing character points (kept from your original)
            var existingChar = await App.Database.GetCharAsync();
            if (existingChar != null)
            {
                existingChar.points = 50;
                await App.Database.UpdateCharAsync(existingChar);
            }

            // Only save an AvatarCostumeSet if there isn't already one saved
            var existingSets = await App.Database.GetAllAvatarSetsAsync();
            if (existingSets == null || existingSets.Count == 0)
            {
                // derive avatar id from image path, e.g. "avatar/avatar1.png" -> "avatar1"
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
                    avatarwhiteunlocked = false,
                };

                await App.Database.SaveAvatarSetAsync(set);
            }

            // Navigate to IndexPage and remove this page from stack so user can't go back to selection
            await Navigation.PushAsync(new IndexPage());
            Navigation.RemovePage(this);

            // reset selection in case the user returns here (defensive)
            AvatarGrid.SelectedItem = null;
        }
    }

    public class Avatar
    {
        public string ImageSource { get; }
        public Avatar(string imageSource) => ImageSource = imageSource;
    }
}
