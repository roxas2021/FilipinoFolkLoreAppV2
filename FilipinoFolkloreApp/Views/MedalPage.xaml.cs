using FilipinoFolkloreApp.Models;
using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views.Home;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static Microsoft.Maui.ApplicationModel.Permissions;
namespace FilipinoFolkloreApp.Views;

public partial class MedalPage : ContentPage
{
    private HeartService HeartService =>
    Application.Current!.Handler!.MauiContext!.Services.GetService<HeartService>()!;
    public ObservableCollection<Medals> Medals { get; set; } = new();

    public MedalPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        BindingContext =this;

	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Load/sync story monitored data from DB into AlamatContent.Stories
        //try
        //{
        //    await App.Database.LoadStoriesAsync();
        //}

        // refresh UI after DB sync
        LoadHud();
        await LoadMedals();
    }
    public async Task LoadMedals()
    {
        Medals.Clear();

        var medalsFromDb = await App.Database.GetMedalAsync();

        // Add existing medals (if any)
        if (medalsFromDb != null)
        {
            foreach (var medal in medalsFromDb)
                Medals.Add(medal);
        }

        // Fill remaining slots up to 30
        int missingCount = 30 - Medals.Count;

        for (int i = 0; i < missingCount; i++)
        {
            Medals.Add(new Medals
            {
                MedalImagePath = "medal_empty.png", // placeholder image
                isUnlocked = false
            });
        }

        MedalsView.ItemsSource = Medals;
    }


    async void OnMedalTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Grid g || g.BindingContext is not Medals medal) return;
        if (medal.isUnlocked == false) return;

        // Navigate to medal detail page
        await Navigation.PushAsync(new MedalDetailPage(medal));
    }
    public ICommand MedalTappedCommand => new Command<Medals>(async (medal) =>
    {
        if (medal.isUnlocked)
        {
            await Navigation.PushAsync(new MedalDetailPage(medal));
        }
    });

    void RefreshHearts()
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
    void LoadHud()
    {
        HudAvatar.Source = CharacterHelper.CurrentAvatar;
        PlayerNameLabel.Text = CharacterHelper.CurrentName;
        StarsLabel.Text = CharacterHelper.CurrentStars.ToString();
        RefreshHearts();
    }
    async void OnHomeTapped(object? s, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }
}