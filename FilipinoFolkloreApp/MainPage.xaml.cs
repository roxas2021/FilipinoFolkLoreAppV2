using System;
using Microsoft.Maui.Controls;
using FilipinoFolkloreApp.Views;
using FilipinoFolkloreApp.Views.Home;
using FilipinoFolkloreApp.Models;

namespace FilipinoFolkloreApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Letter_Clicked(object sender, EventArgs e)
        {
            if (sender is ImageButton btn && btn.CommandParameter is string letter)
            {
                if (letter == "enter")
                {
                    //await App.Database.SaveCharAsync(new Character { name = OutputEntry.Text });

                    await Navigation.PushAsync(new AvatarSelectionPage());
                }
                else
                {
                    OutputEntry.Text += letter;
                }
            }
        }
    }
}
