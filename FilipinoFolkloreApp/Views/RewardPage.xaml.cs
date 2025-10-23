using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace FilipinoFolkloreApp.Views;

public partial class RewardPage : ContentPage
{
    private readonly int _stars;
    public RewardPage(int stars)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _stars = stars;
        RewardText.Text = $"+{_stars} ⭐";
    }

    async void OnRewardOk(object? s, EventArgs e)
    {
        // Return to Alamat (clear to root so there's no back button needed)
        await Navigation.PopToRootAsync();
    }
}
