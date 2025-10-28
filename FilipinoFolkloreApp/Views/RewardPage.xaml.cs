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
        var pages = Navigation.NavigationStack.ToList();
        foreach (var page in pages)
        {
            if (page is RewardPage)
            {
                // Remove RewardPage from the stack
                Navigation.RemovePage(page);
            }
            if (page is QuizPage)
            {
                // Remove QuizPage from the stack
                Navigation.RemovePage(page);
            }
            if (page is StoryPage)
            {
                // Remove StoryPage from the stack
                Navigation.RemovePage(page);
            }
            if (page is NarratorPage)
            {
                Navigation.RemovePage(page);
            }
        }

        // Optionally, navigate to another page
        await Navigation.PushAsync(new AlamatPage());

    }
}
