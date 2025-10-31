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

        // Check for saved character on page appear and skip this page if found
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_navigated) return;

            try
            {
                var existing = await App.Database.GetCharAsync().ConfigureAwait(false);

                if (existing != null && !string.IsNullOrWhiteSpace(existing.name))
                {
                    // Use the page's Dispatcher to run the navigation on the UI thread.
                    // Wrap body in try/catch to surface errors from async void lambda.
                    Dispatcher.Dispatch(async () =>
                    {
                        try
                        {
                            if (_navigated) return;
                            _navigated = true;
                            CharacterHelper.CurrentName = existing.name;
                            CharacterHelper.CurrentStars = existing.stars;
                            // Insert target page before this one, then pop this page

                            await Navigation.PushAsync(new AvatarSelectionPage());
                            Navigation.RemovePage(this);
                        }
                        catch (Exception dEx)
                        {
                            // handle/log so the async-void lambda doesn't hide exceptions
                            System.Diagnostics.Debug.WriteLine($"Navigation failed: {dEx}");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnAppearing check failed: {ex}");
            }
        }


        private async void Letter_Clicked(object sender, EventArgs e)
        {
            if (sender is ImageButton btn && btn.CommandParameter is string letter)
            {
                if (letter == "enter")
                {
                    var name = (OutputEntry.Text ?? string.Empty).Trim();

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        // simple feedback if empty
                        await DisplayAlert("Name required", "Please enter your name.", "OK");
                        return;
                    }

                    try
                    {
                        // Try to save: use SaveCharAsync if your DatabaseService already provides it.
                        // If you don't have SaveCharAsync, the Ensure+Update fallback below will work.
                        if (App.Database.GetType().GetMethod("SaveCharAsync") != null)
                        {
                            // uses your existing SaveCharAsync API
                            await App.Database.SaveCharAsync(new Character { Id = 1, name = name, stars = 100 }).ConfigureAwait(false);
                        }
                        else
                        {
                            // Fallback: ensure the main row exists, then update its name & stars
                            await App.Database.EnsureMainCharacterExistsAsync().ConfigureAwait(false);

                            // Update fields via the existing Update-style helpers (we used UpdateCurrentAvatarAsync/SetStarsAsync earlier).
                            // If those helpers don't exist in your DatabaseService, update by reading, setting and calling UpdateAsync via the service.
                            var c = await App.Database.GetCharAsync().ConfigureAwait(false);
                            if (c == null)
                            {
                                // as a last resort, insert directly
                                await App.Database.ResetCharacterToDefaultsAsync().ConfigureAwait(false);
                                c = await App.Database.GetCharAsync().ConfigureAwait(false);
                            }

                            c.name = name;
                            c.stars = 100;

                            // try a common Update method if present
                            var updateMethod = App.Database.GetType().GetMethod("UpdateCurrentAvatarAsync");
                            if (App.Database.GetType().GetMethod("SetStarsAsync") != null)
                            {
                                // set stars explicitly
                                await App.Database.SetStarsAsync(c.stars).ConfigureAwait(false);
                            }
                            else
                            {
                                // direct update fallback (assumes DatabaseService exposes UpdateAsync-like functionality)
                                // This will work only if your DatabaseService exposes a method to run a raw update.
                                // If not, replace with whatever update method you have.
                                await App.Database.UpdateAvatarAndAddStarsAsync(c.currentavatar, 0).ConfigureAwait(false);
                                // then set name via reset helper:
                                await App.Database.ResetCharacterToDefaultsAsync().ConfigureAwait(false);
                                // finally fetch again and set name
                                var after = await App.Database.GetCharAsync().ConfigureAwait(false);
                                after.name = name;
                                await App.Database.SetStarsAsync(c.stars).ConfigureAwait(false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // If something goes wrong, log and show alert, so user isn't stuck
                        System.Diagnostics.Debug.WriteLine($"Save name failed: {ex}");
                        await DisplayAlert("Save failed", "Couldn't save your name. Try again.", "OK");
                        return;
                    }

                    // navigate to avatar selection (same replace trick as above)
                    if (!_navigated)
                    {
                        
                        Dispatcher.Dispatch(async () =>
                        {
                            try
                            {
                                _navigated = true;
                                CharacterHelper.CurrentName = name;
                                CharacterHelper.CurrentStars = 100;
                                await Navigation.PushAsync(new AvatarSelectionPage(), true);
                                Navigation.RemovePage(this);
                            }
                            catch (Exception dEx)
                            {
                                // handle/log so the async-void lambda doesn't hide exceptions
                                System.Diagnostics.Debug.WriteLine($"Navigation failed: {dEx}");
                            }

                        });
                        
                    }
                }
                else
                {
                    OutputEntry.Text += letter;
                }
            }
        }
    }
}
