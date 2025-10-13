using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Linq;

namespace FilipinoFolkloreApp.Views
{
    public partial class CharacterCostume : ContentPage
    {
        private int NicholAmountValue = 300; // Player's current currency
        private List<bool> purchasedCostumes; // List to track the purchase state of each costume
        private List<int> costumePrices; // Prices for each costume
        private int selectedCostumeId = 0; // Tracks the selected costume for BUY/USE

        public List<TapisItem> TapisItems { get; set; }

        public CharacterCostume()
        {
            InitializeComponent();
            

            // Initialize the purchasedCostumes list to track the state of each costume (not purchased initially)
            purchasedCostumes = new List<bool> { false, false, false, false, false, false };  // Example with 6 costumes
            costumePrices = new List<int> { 100, 150, 180, 200, 250, 280 };  // Example prices

            // Initialize list of costumes with associated avatar images
            TapisItems = new List<TapisItem>
            {
                new TapisItem { TapisImageSource = "avatarcustomization/tapis_blue.png", AvatarImageSource = "avatarcustomization/avatar_blue.png", Price = "100", TapisId = 1 },
                new TapisItem { TapisImageSource = "avatarcustomization/tapis_green.png", AvatarImageSource = "avatarcustomization/avatar_green.png", Price = "150", TapisId = 2 },
                new TapisItem { TapisImageSource = "avatarcustomization/tapis_red.png", AvatarImageSource = "avatarcustomization/avatar_red.png", Price = "180", TapisId = 3 },
                new TapisItem { TapisImageSource = "avatarcustomization/tapis_bluered.png", AvatarImageSource = "avatarcustomization/avatar_bluered.png", Price = "200", TapisId = 4 },
                new TapisItem { TapisImageSource = "avatarcustomization/tapis_white.png", AvatarImageSource = "avatarcustomization/avatar_white.png", Price = "250", TapisId = 5 },
                new TapisItem { TapisImageSource = "avatarcustomization/tapis_pink.png", AvatarImageSource = "avatarcustomization/avatar_pink.png", Price = "280", TapisId = 6 }
            };

            // Bind the TapisItems list to the CollectionView
            TapisCollectionView.ItemsSource = TapisItems;

            // Display the initial amount
            NicholAmount.Text = NicholAmountValue.ToString();
        }

        // Handle selecting a costume (Tapis) and updating the avatar
        private void OnTapisSelected(object sender, SelectionChangedEventArgs e)
        {
            var selectedTapis = e.CurrentSelection.FirstOrDefault() as TapisItem;

            if (selectedTapis != null)
            {
                selectedCostumeId = selectedTapis.TapisId;

                // Update the avatar image regardless of purchase status
                CharacterImage.Source = $"avatarcustomization/{selectedTapis.AvatarImageSource}";

                // If purchased, change the Buy button text to "SELECT", otherwise "BUY"
                if (purchasedCostumes[selectedCostumeId - 1])
                {
                    BuyButton.Text = "SELECT";
                }
                else
                {
                    BuyButton.Text = "BUY";
                }
            }
        }

        // Handle the BUY/SELECT button click
        private void OnBuyButtonClicked(object sender, EventArgs e)
        {
            if (selectedCostumeId == 0) return;

            int itemCost = costumePrices[selectedCostumeId - 1];

            if (purchasedCostumes[selectedCostumeId - 1])
            {
                // Already purchased
                DisplayAlert("Already Purchased", "You have already bought this costume.", "OK");
                CharacterImage.Source = $"avatarcustomization/{TapisItems[selectedCostumeId - 1].AvatarImageSource}";
            }
            else
            {
                if (NicholAmountValue >= itemCost)
                {
                    // Proceed with the purchase
                    NicholAmountValue -= itemCost;
                    purchasedCostumes[selectedCostumeId - 1] = true; // Mark the costume as purchased

                    // Update UI
                    NicholAmount.Text = NicholAmountValue.ToString();
                    DisplayAlert("Purchase", "You bought the item!", "OK");

                    // Update the character image with the associated avatar image for the selected Tapis
                    CharacterImage.Source = $"avatarcustomization/{TapisItems[selectedCostumeId - 1].AvatarImageSource}";

                    // Change the button text to "SELECT" after purchase
                    BuyButton.Text = "SELECT";
                }
                else
                {
                    DisplayAlert("Insufficient Funds", "You don't have enough Nichol!", "OK");
                }
            }
        }

        // Handle the Home button click
        private void OnHomeButtonClicked(object sender, EventArgs e)
        {
            // Navigate back to the home page (or go to a different page)
            Navigation.PopAsync();
        }
    }

    public class TapisItem
    {
        public string TapisImageSource { get; set; }  // Path to the Tapis image
        public string AvatarImageSource { get; set; } // Path to the associated avatar image
        public string Price { get; set; }
        public int TapisId { get; set; }
    }
}
