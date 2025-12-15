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
        // user's balance
        private int PilonStarNicholAmountValue = 300;
        private List<bool> purchasedCostumes;
        private List<int> costumePrices;
        private int selectedCostumeId = 0;

        // awaitable alert TCS 
        TaskCompletionSource<bool> _alertTcs;
        private AvatarCustomizationHelper.AvatarSet currentAvatarSet = AvatarCustomizationHelper.CurrentAvatarSet(AvatarCustomizationHelper.SelectedAvatarSetId);
        // expose the username so it can be changed programmatically or bound from VM         
        public string CurrentUserName { get; set; } = "Nichol";

        public List<TapisItem> TapisItems { get; set; }

        public CharacterCostume()
        {
            InitializeComponent();
            CurrentUserName = CharacterHelper.CurrentName;
            PilonStarNicholAmountValue = CharacterHelper.CurrentStars;
            // set binding context so XAML bindings work
            BindingContext = this;


        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            CharacterImage.Source = CharacterHelper.CurrentAvatar;
            costumePrices = new List<int> { 100, 150, 180, 200, 250 };

            await LoadTapisItemsAsync();
            // update balance label
            PilonStarNicholLabel.Text = CharacterHelper.CurrentStars.ToString();

            // also ensure alert message uses current user name by default
            AlertMessageLabel.Text = $"You don't have enough {CurrentUserName}!";

        }
        // put near other private helpers
        private async Task LoadTapisItemsAsync()
        {
            // ensure purchasedCostumes is up to date (AvatarCustomizationHelper.LoadPurchasedCostume is async in your code)

            await AvatarCustomizationHelper.LoadPurchasedCostume();

            purchasedCostumes = AvatarCustomizationHelper.purchasedCostumes;

            // (re)create your tapis list from currentAvatarSet and the latest purchasedCostumes
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


            // optional: restore selection to the item you were on (if still valid)
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

            // normalize slashes + lower
            src = src.Replace("\\", "/").Trim().ToLowerInvariant();

            // compare by file name (robust even if one string has folders)
            var lastSlash = src.LastIndexOf('/');
            return lastSlash >= 0 ? src[(lastSlash + 1)..] : src;
        }

        private async void OnTapisSelected(object sender, SelectionChangedEventArgs e)
        {
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
            // If purchased AND currently equipped -> show EQUIPPED
            if (selected.IsPurchased && IsEquipped(selected))
            {
                BuyButton.Text = "EQUIPPED";
                BuyButton.BackgroundColor = Colors.SeaGreen;
                BuyButton.IsEnabled = false;
                return;
            }

            if (selected.IsPurchased)
            {
                BuyButton.Text = "SELECT";
                BuyButton.BackgroundColor = Colors.SeaGreen;
                BuyButton.IsEnabled = true;
                return;
            }

            // NOT purchased
            BuyButton.Text = $"{selected.Price} BUY";
            BuyButton.BackgroundColor = Colors.DeepPink;
            BuyButton.IsEnabled = PilonStarNicholAmountValue >= selected.Price;
        }


        private async Task AnimateAvatarChangeAsync(string newAvatarSource)
        {
            try
            {
                // quick fade out (makes swap look smooth)
                await CharacterImage.FadeTo(0.25, 120, Easing.CubicIn);

                // swap image source
                CharacterImage.Source = $"{newAvatarSource}";

                // prepare for pop/rotate: ensure base transforms are reset
                CharacterImage.Rotation = 0;
                CharacterImage.TranslationY = 0;

                // do pop + rotate in parallel
                var pop = CharacterImage.ScaleTo(1.18, 180, Easing.CubicOut);
                var rot = CharacterImage.RotateTo(6, 140, Easing.CubicOut);
                await Task.WhenAll(pop, rot);

                // bounce back to normal scale and rotate back to 0
                var settleScale = CharacterImage.ScaleTo(1.0, 220, Easing.BounceOut);
                var settleRot = CharacterImage.RotateTo(0, 140, Easing.SpringOut);
                // small upward bounce
                var translateUp = CharacterImage.TranslateTo(0, -8, 100, Easing.CubicOut);
                await Task.WhenAll(settleScale, settleRot, translateUp);

                // fall back into place
                await CharacterImage.TranslateTo(0, 0, 160, Easing.BounceOut);

                // final fade in to fully visible
                await CharacterImage.FadeTo(1.0, 150, Easing.CubicIn);

                // tiny pulse on the buy/select button to show action affordance
                await BuyButton.ScaleTo(1.06, 110, Easing.CubicOut);
                await BuyButton.ScaleTo(1.0, 120, Easing.BounceOut);
            }
            catch
            {
                // swallow animation errors so nothing breaks if the platform can't animate
            }
        }


        private async void OnBuyButtonClicked(object sender, EventArgs e)
        {
            if (selectedCostumeId == 0) return;

            var selected = TapisItems[selectedCostumeId - 1];
            int itemCost = selected.Price;

            // SELECT (already owned)
            if (selected.IsPurchased)
            {
                CharacterHelper.CurrentAvatar = selected.AvatarImageSource;
                await App.Database.UpdateCurrentAvatarAsync(CharacterHelper.CurrentAvatar);

                await ShowGameAlertAsync(
                    PilonStarNicholAmountValue,
                    "Costume equipped!",
                    "emoji_happy.png"
                );

                BuyButton.Text = "EQUIPPED";
                BuyButton.IsEnabled = false;
                return;
            }

            // BUY
            if (PilonStarNicholAmountValue < itemCost)
            {
                await ShowGameAlertAsync(PilonStarNicholAmountValue, null, "emoji_sad.png");
                return;
            }

            // PURCHASE SUCCESS
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
                "Unlocked!",
                "emoji_happy.png"
            );

            UpdateBuyButtonState(selected);
            TapisCollectionView.ItemsSource = null;
            TapisCollectionView.ItemsSource = TapisItems;
        }



        private void OnHomeButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        // awaitable overlay — message is optional; default uses CurrentUserName
        public Task ShowGameAlertAsync(int amount, string message = null, string emojiSource = "emoji_sad.png")
        {
            if (GameAlertOverlay.IsVisible && _alertTcs != null)
                return _alertTcs.Task;

            _alertTcs = new TaskCompletionSource<bool>();

            // set the emoji image (use provided source)
            AlertEmoji.Source = emojiSource;

            // show current balance in the pill
            AlertAmountLabel.Text = PilonStarNicholAmountValue.ToString();
            // default message uses the CurrentUserName if not supplied
            if (string.IsNullOrWhiteSpace(message))
                message = $"You don't have enough pilon star!";

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

        // UI states
        public bool IsSelected { get; set; }
    }
}