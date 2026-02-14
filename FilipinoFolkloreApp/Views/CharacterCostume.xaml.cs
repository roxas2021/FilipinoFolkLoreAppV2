using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp.Views
{
    public partial class CharacterCostume : ContentPage
    {
        private int PilonStarNicholAmountValue = 300;
        private List<bool> purchasedCostumes;
        private List<int> costumePrices;
        private int selectedCostumeId = 0;

        TaskCompletionSource<bool> _alertTcs;
        private AvatarCustomizationHelper.AvatarSet currentAvatarSet = AvatarCustomizationHelper.CurrentAvatarSet(AvatarCustomizationHelper.SelectedAvatarSetId);
          
        public string CurrentUserName { get; set; } = "Nichol";

        public List<TapisItem> TapisItems { get; set; }

        private SoundService SoundService =>
            Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;

        public CharacterCostume()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            CurrentUserName = CharacterHelper.CurrentName;
            PilonStarNicholAmountValue = CharacterHelper.CurrentStars;
            BindingContext = this;


        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            CharacterImage.Source = CharacterHelper.CurrentAvatar;
            costumePrices = new List<int> { 100, 150, 180, 200, 250 };

            await LoadTapisItemsAsync();
            PilonStarNicholLabel.Text = CharacterHelper.CurrentStars.ToString();

            AlertMessageLabel.Text = $"You don't have enough {CurrentUserName}!";

        }
        private async Task LoadTapisItemsAsync()
        {
            

            await AvatarCustomizationHelper.LoadPurchasedCostume();

            purchasedCostumes = AvatarCustomizationHelper.purchasedCostumes;
            var items = new List<TapisItem>
                {
                    new() { TapisImageSource = currentAvatarSet.TapisPaths[0], AvatarImageSource = currentAvatarSet.CostumePaths[1], IsPurchased = purchasedCostumes[0], Price = 100, TapisId = 1 },
                    new() { TapisImageSource = currentAvatarSet.TapisPaths[1], AvatarImageSource = currentAvatarSet.CostumePaths[2], IsPurchased = purchasedCostumes[1], Price = 150, TapisId = 2 },
                    new() { TapisImageSource = currentAvatarSet.TapisPaths[2], AvatarImageSource = currentAvatarSet.CostumePaths[3], IsPurchased = purchasedCostumes[2], Price = 180, TapisId = 3 },
                    new() { TapisImageSource = currentAvatarSet.TapisPaths[3], AvatarImageSource = currentAvatarSet.CostumePaths[4], IsPurchased = purchasedCostumes[3], Price = 200, TapisId = 4 },
                    new() { TapisImageSource = currentAvatarSet.TapisPaths[4], AvatarImageSource = currentAvatarSet.CostumePaths[5], IsPurchased = purchasedCostumes[4], Price = 250, TapisId = 5 }
                };

            TapisItems = items;
            TapisCollectionView.ItemsSource = TapisItems;


            if (selectedCostumeId > 0 && selectedCostumeId <= TapisItems.Count)
                TapisCollectionView.SelectedItem = TapisItems[selectedCostumeId - 1];

        }
        private bool IsEquipped(TapisItem item)
        {
            return NormalizeAvatarKey(CharacterHelper.CurrentAvatar)
                == NormalizeAvatarKey(item.AvatarImageSource);
        }

        private static string NormalizeAvatarKey(string src)
        {
            if (string.IsNullOrWhiteSpace(src)) return string.Empty;

            src = src.Replace("\\", "/").Trim().ToLowerInvariant();

            var lastSlash = src.LastIndexOf('/');
            return lastSlash >= 0 ? src[(lastSlash + 1)..] : src;
        }

        private async void OnTapisSelected(object sender, SelectionChangedEventArgs e)
        {
            await SoundService.PlayButtonClickAsync();
            
            var selected = e.CurrentSelection.FirstOrDefault() as TapisItem;
            if (selected == null) return;

            foreach (var item in TapisItems)
                item.IsSelected = false;

            selected.IsSelected = true;
            selectedCostumeId = selected.TapisId;

            UpdateBuyButtonState(selected);
            await AnimateAvatarChangeAsync(selected.AvatarImageSource);

        }

        private void UpdateBuyButtonState(TapisItem selected)
        {
            if (selected.IsPurchased && IsEquipped(selected))
            {
                BuyButton.Text = "NAKA-SUOT";
                BuyButton.BackgroundColor = Colors.SeaGreen;
                BuyButton.IsEnabled = false;
                return;
            }

            if (selected.IsPurchased)
            {
                BuyButton.Text = "PILIIN";
                BuyButton.BackgroundColor = Colors.SeaGreen;
                BuyButton.IsEnabled = true;
                return;
            }

            BuyButton.Text = $"BILHIN {selected.Price}";
            BuyButton.BackgroundColor = Colors.DeepPink;
            BuyButton.IsEnabled = PilonStarNicholAmountValue >= selected.Price;
        }


        private async Task AnimateAvatarChangeAsync(string newAvatarSource)
        {
            try
            {
                await CharacterImage.FadeTo(0.25, 120, Easing.CubicIn);

                CharacterImage.Source = $"{newAvatarSource}";

                CharacterImage.Rotation = 0;
                CharacterImage.TranslationY = 0;

                var pop = CharacterImage.ScaleTo(1.18, 180, Easing.CubicOut);
                var rot = CharacterImage.RotateTo(6, 140, Easing.CubicOut);
                await Task.WhenAll(pop, rot);

                var settleScale = CharacterImage.ScaleTo(1.0, 220, Easing.BounceOut);
                var settleRot = CharacterImage.RotateTo(0, 140, Easing.SpringOut);
                var translateUp = CharacterImage.TranslateTo(0, -8, 100, Easing.CubicOut);
                await Task.WhenAll(settleScale, settleRot, translateUp);

                await CharacterImage.TranslateTo(0, 0, 160, Easing.BounceOut);

                await CharacterImage.FadeTo(1.0, 150, Easing.CubicIn);

                await BuyButton.ScaleTo(1.06, 110, Easing.CubicOut);
                await BuyButton.ScaleTo(1.0, 120, Easing.BounceOut);
            }
            catch
            {
                
            }
        }


        private async void OnBuyButtonClicked(object sender, EventArgs e)
        {
            await SoundService.PlayButtonClickAsync();
            
            if (selectedCostumeId == 0) return;

            var selected = TapisItems[selectedCostumeId - 1];
            int itemCost = selected.Price;

            if (selected.IsPurchased)
            {
                CharacterHelper.CurrentAvatar = selected.AvatarImageSource;
                await App.Database.UpdateCurrentAvatarAsync(CharacterHelper.CurrentAvatar);

                await ShowGameAlertAsync(
                    PilonStarNicholAmountValue,
                    "Naka-suot na ang tapis!",
                    "emoji_happy.png"
                );

                BuyButton.Text = "NAKA-SUOT";
                BuyButton.IsEnabled = false;
                return;
            }

            if (PilonStarNicholAmountValue < itemCost)
            {
                await ShowGameAlertAsync(PilonStarNicholAmountValue, null, "emoji_sad.png");
                return;
            }

            PilonStarNicholAmountValue -= itemCost;
            CharacterHelper.CurrentStars = PilonStarNicholAmountValue;
            PilonStarNicholLabel.Text = PilonStarNicholAmountValue.ToString();

            selected.IsPurchased = true;
            purchasedCostumes[selectedCostumeId - 1] = true;

            await App.Database.SetStarsAsync(PilonStarNicholAmountValue);

            string avatarId = AvatarCustomizationHelper.SelectedAvatarSetId;
            string costumeKey = (selectedCostumeId - 1) switch
            {
                0 => "avatarblue",
                1 => "avatarbluered",
                2 => "avatargreen",
                3 => "avatarpink",
                4 => "avatarred",
                _ => throw new ArgumentOutOfRangeException()
            };

            await App.Database.UnlockCostumeAsync(avatarId, costumeKey);

            CharacterHelper.CurrentAvatar = selected.AvatarImageSource;
            await App.Database.UpdateCurrentAvatarAsync(CharacterHelper.CurrentAvatar);

            await ShowGameAlertAsync(
                PilonStarNicholAmountValue,
                "Nabili!",
                "emoji_happy.png"
            );

            UpdateBuyButtonState(selected);
            TapisCollectionView.ItemsSource = null;
            TapisCollectionView.ItemsSource = TapisItems;
        }



        private async void OnHomeButtonClicked(object sender, EventArgs e)
        {
            await SoundService.PlayButtonClickAsync();
            await Navigation.PopAsync();
        }

        public Task ShowGameAlertAsync(int amount, string message = null, string emojiSource = "emoji_sad.png")
        {
            if (GameAlertOverlay.IsVisible && _alertTcs != null)
                return _alertTcs.Task;

            _alertTcs = new TaskCompletionSource<bool>();

            AlertEmoji.Source = emojiSource;

            AlertAmountLabel.Text = PilonStarNicholAmountValue.ToString();
            if (string.IsNullOrWhiteSpace(message))
                message = $"Kulang ang iyong Pilon Star!";

            AlertMessageLabel.Text = message;

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

        private async Task HideGameAlertAsync(bool completedByUser = true)
        {
            if (!GameAlertOverlay.IsVisible) return;

            try
            {
                await GameAlertCard.ScaleTo(0.96, 120, Easing.CubicIn);
                await GameAlertOverlay.FadeTo(0, 140, Easing.CubicOut);
            }
            catch { }

            GameAlertOverlay.IsVisible = false;

            _alertTcs?.TrySetResult(completedByUser);
            _alertTcs = null;
        }

        private async void OnAlertOkClicked(object sender, EventArgs e)
        {
            await SoundService.PlayButtonClickAsync();
            await HideGameAlertAsync(true);
        }

        private async void OnAlertBackgroundTapped(object sender, EventArgs e)
        {
            await HideGameAlertAsync(false);
        }
    }

    public class TapisItem
    {
        public string TapisImageSource { get; set; }
        public string AvatarImageSource { get; set; }
        public bool IsPurchased { get; set; }
        public int Price { get; set; }
        public int TapisId { get; set; }
        public bool IsSelected { get; set; }
    }
}