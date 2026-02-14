using FilipinoFolkloreApp.Models;
using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views.Home;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilipinoFolkloreApp.Views;

public partial class BugtongListPage : ContentPage
{
    private HeartService HeartService =>
        Application.Current!.Handler!.MauiContext!.Services.GetService<HeartService>()!;

    private SoundService SoundService =>
        Application.Current!.Handler!.MauiContext!.Services.GetService<SoundService>()!;

    public BugtongListPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        LoadHud();
        LoadBugtongs();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AlamatContent.Hearts = HeartService.GetHearts();
        RefreshHearts();
        RefreshStars();
    }

    private void LoadHud()
    {
        HudAvatar.Source = CharacterHelper.CurrentAvatar;
        PlayerNameLabel.Text = CharacterHelper.CurrentName;
        RefreshStars();
        RefreshHearts();
    }

    private void RefreshStars()
    {
        StarsLabel.Text = CharacterHelper.CurrentStars.ToString();
    }

    private void RefreshHearts()
    {
        HeartsPanel.Children.Clear();
        for (int i = 0; i < AlamatContent.Hearts; i++)
        {
            HeartsPanel.Children.Add(new Image
            {
                Source = "heart_full.png",
                WidthRequest = 24,
                HeightRequest = 24,
                Aspect = Aspect.AspectFit
            });
        }
    }

    private async void LoadBugtongs()
    {
        try
        {
            await App.Database.LoadBugtongsAsync();

            var bugtongViewModels = BugtongService.Bugtongs
                .Where(b => b.IsAvailable)
                .Select(b => new BugtongViewModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    RewardStars = b.RewardStars,
                    HasMedal = b.MedalId.HasValue
                })
                .ToList();

            BugtongCollection.ItemsSource = bugtongViewModels;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading bugtongs: {ex}");
            await DisplayAlert("Error", "Hindi ma-load ang mga bugtong.", "OK");
        }
    }

    private async void OnBugtongTapped(object? sender, TappedEventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        
        if (e.Parameter is string bugtongId)
        {
            await Navigation.PushAsync(new BugtongQuizPage(bugtongId));
        }
    }
    
    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        await Navigation.PopAsync();
    }

    private async void OnHomeTapped(object? sender, TappedEventArgs e)
    {
        await SoundService.PlayButtonClickAsync();
        
        var pages = Navigation.NavigationStack.ToList();
        foreach (var page in pages)
        {
            if (page is BugtongQuizPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is BugtongListPage)
            {
                Navigation.RemovePage(page);
            }
            if (page is MgaLaroPage)
            {
                Navigation.RemovePage(page);
            }
        }

        await Navigation.PushAsync(new IndexPage());
    }
}

// ViewModel for binding
public class BugtongViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int RewardStars { get; set; }
    public bool HasMedal { get; set; }
}