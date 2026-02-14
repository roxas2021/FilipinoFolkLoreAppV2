using System;
using Microsoft.Maui.Controls;
using FilipinoFolkloreApp.Views;
using FilipinoFolkloreApp.Views.Home;
using FilipinoFolkloreApp.Models;
using System.Threading.Tasks;
using FilipinoFolkloreApp.Services;

namespace FilipinoFolkloreApp
{
    public partial class MainPage : ContentPage
    {
        // guard to avoid navigating twice
        private bool _navigated = false;

        public MainPage()
        {
            InitializeComponent();
        }

        // Removed OnAppearing navigation logic - now handled in App.xaml.cs

        private async void Letter_Clicked(object sender, EventArgs e)
        {
            if (sender is ImageButton btn && btn.CommandParameter is string letter)
            {
                if (letter == "enter")
                {
                    var name = (OutputEntry.Text ?? string.Empty).Trim();

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        await DisplayAlert("Name required", "Please enter your name.", "OK");
                        return;
                    }

                    try
                    {
                        // Save character
                        await App.Database.SaveCharAsync(new Character { Id = 1, name = name, stars = 100 });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Save name failed: {ex}");
                        await DisplayAlert("Save failed", "Couldn't save your name. Try again.", "OK");
                        return;
                    }

                    // Navigate to avatar selection
                    if (!_navigated)
                    {
                        _navigated = true;
                        CharacterHelper.CurrentName = name;
                        CharacterHelper.CurrentStars = 100;

                        await Navigation.PushAsync(new AvatarSelectionPage(), true);
                        Navigation.RemovePage(this);
                    }
                }
                else if (letter == "erase")
                {
                    OutputEntry.Text = string.Empty;
                }
                else
                {
                    OutputEntry.Text += letter.ToUpper();
                }
            }
        }
    }
}