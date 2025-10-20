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

        // expose the username so it can be changed programmatically or bound from VM
        public string CurrentUserName { get; set; } = "Nichol";
//        CurrentUserName = "New Name";
//BindingContext = this;            // re-set if necessary, or raise PropertyChanged
//PilonStarNicholLabel.Text = PilonStarNicholAmountValue.ToString(); // balance unchanged
//AlertMessageLabel.Text = $"You don't have enough {CurrentUserName}!"; // update default alert text

        public List<TapisItem> TapisItems { get; set; }

        public CharacterCostume()
        {
            InitializeComponent();

            // set binding context so XAML bindings work
            BindingContext = this;

            purchasedCostumes = new List<bool> { false, false, false, false, false, false };
            costumePrices = new List<int> { 100, 150, 180, 200, 250, 280 };

            TapisItems = new List<TapisItem>
            {
                new TapisItem { TapisImageSource = "avatarcustomization/tapis_blue.png", AvatarImageSource = "avatarcustomization/avatar_blue.png", Price = "100", TapisId = 1 },
                new TapisItem { TapisImageSource = "avatarcustomization/tapis_green.png", AvatarImageSource = "avatarcustomization/avatar_green.png", Price = "150", TapisId = 2 },
                new TapisItem { TapisImageSource = "avatarcustomization/tapis_red.png", AvatarImageSource = "avatarcustomization/avatar_red.png", Price = "180", TapisId = 3 },
                new TapisItem { TapisImageSource = "avatarcustomization/tapis_bluered.png", AvatarImageSource = "avatarcustomization/avatar_bluered.png", Price = "200", TapisId = 4 },
                new TapisItem { TapisImageSource = "avatarcustomization/tapis_white.png", AvatarImageSource = "avatarcustomization/avatar_white.png", Price = "250", TapisId = 5 },
                new TapisItem { TapisImageSource = "avatarcustomization/tapis_pink.png", AvatarImageSource = "avatarcustomization/avatar_pink.png", Price = "280", TapisId = 6 }
            };

            TapisCollectionView.ItemsSource = TapisItems;

            // update balance label
            PilonStarNicholLabel.Text = PilonStarNicholAmountValue.ToString();

            // also ensure alert message uses current user name by default
            AlertMessageLabel.Text = $"You don't have enough {CurrentUserName}!";
        }

        private async void OnTapisSelected(object sender, SelectionChangedEventArgs e)
        {
            var selectedTapis = e.CurrentSelection.FirstOrDefault() as TapisItem;
            if (selectedTapis == null) return;

            selectedCostumeId = selectedTapis.TapisId;

            // animate avatar change (crossfade + pop + bounce)
            await AnimateAvatarChangeAsync(selectedTapis.AvatarImageSource);

            // Update Buy button text depending on purchase state
            if (purchasedCostumes[selectedCostumeId - 1])
                BuyButton.Text = "SELECT";
            else
                BuyButton.Text = "BUY";
        }
        private async Task AnimateAvatarChangeAsync(string newAvatarSource)
        {
            try
            {
                // quick fade out (makes swap look smooth)
                await CharacterImage.FadeTo(0.25, 120, Easing.CubicIn);

                // swap image source
                CharacterImage.Source = $"avatarcustomization/{newAvatarSource}";

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

            int itemCost = costumePrices[selectedCostumeId - 1];

            if (purchasedCostumes[selectedCostumeId - 1])
            {
                // Already purchased -> neutral emoji + message
                await ShowGameAlertAsync(PilonStarNicholAmountValue, "You have selected this costume.", "emoji_happy.png");

                // still set the avatar to the selected one (keeps previous behavior)
                CharacterImage.Source = $"avatarcustomization/{TapisItems[selectedCostumeId - 1].AvatarImageSource}";
            }
            else
            {
                if (PilonStarNicholAmountValue >= itemCost)
                {
                    // Purchase success -> deduct, update UI, show happy emoji
                    PilonStarNicholAmountValue -= itemCost;
                    purchasedCostumes[selectedCostumeId - 1] = true;

                    // update UI
                    PilonStarNicholLabel.Text = PilonStarNicholAmountValue.ToString();

                    // show success modal (happy emoji)
                    await ShowGameAlertAsync(PilonStarNicholAmountValue, "You bought the item!", "emoji_happy.png");

                    CharacterImage.Source = $"avatarcustomization/{TapisItems[selectedCostumeId - 1].AvatarImageSource}";
                    BuyButton.Text = "SELECT";
                }
                else
                {
                    // Insufficient funds -> sad emoji (amount param still shows current balance)
                    await ShowGameAlertAsync(PilonStarNicholAmountValue, null, "emoji_sad.png");
                }
            }
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
        public string Price { get; set; }
        public int TapisId { get; set; }
    }
}
