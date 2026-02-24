using FilipinoFolkloreApp.Models;
using FilipinoFolkloreApp.Services;
using FilipinoFolkloreApp.Views.Home;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Layouts;
using System;

namespace FilipinoFolkloreApp.Views
{
    public partial class AvatarSelectionPage : ContentPage
    {
        private int _tutorialStep = 0;
        private const string TUTORIAL_COMPLETED_KEY = "AvatarSelectionTutorialCompleted";

        private bool _isNavigating = false;

                 private readonly TutorialStep[] _tutorialSteps = new[]
        {
            new TutorialStep
            {
                Title = "Pumili ng Iyong Avatar!",
                Message = "Maligayang pagdating! Pumili ng avatar na magiging iyong kaibigan sa adventure.",
                TargetElementName = null,
            },
            new TutorialStep
            {
                Title = "Tingnan ang mga Pagpipilian",
                Message = "May apat na magagandang avatar na pwede mong piliin. Bawat isa ay may sariling istilo!",
                TargetElementName = "AvatarGrid",
            },
            new TutorialStep
            {
                Title = "I-tap para Piliin",
                Message = "I-click ang avatar na gusto mo. Makakakuha ka ng 50 Pilon Stars para magsimula!",
                TargetElementName = "Avatar3",
            }
        };

        public ObservableCollection<Avatar> Avatars { get; } = new()
        {
            new Avatar("avatarcustomization/avatar1/avatar1.png"),
            new Avatar("avatarcustomization/avatar2/avatar2.png"),
            new Avatar("avatarcustomization/avatar3/avatar3.png"),
            new Avatar("avatarcustomization/avatar4/avatar4.png"),
        };

        public AvatarSelectionPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            AvatarGrid.ItemsSource = Avatars;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

                         _isNavigating = false;

                         bool tutorialCompleted = Preferences.Get(TUTORIAL_COMPLETED_KEY, false);
            if (!tutorialCompleted)
            {
                await Task.Delay(500);
                await ShowTutorial();
            }
        }

        private async Task ShowTutorial()
        {
            _tutorialStep = 0;

            UpdateTutorialStep();
            TutorialOverlay.IsVisible = true;

                         await Task.WhenAll(
                TarsierImage.FadeTo(1, 400, Easing.CubicOut),
                TarsierImage.ScaleTo(1, 400, Easing.BounceOut)
            );

                         await Task.Delay(200);
            await Task.WhenAll(
                SpeechBubbleContainer.FadeTo(1, 300, Easing.CubicOut),
                SpeechBubbleContainer.ScaleTo(1, 300, Easing.BounceOut)
            );

        }

        private async void OnTutorialNextStep(object? sender, EventArgs e)
        {
            _tutorialStep++;

            if (_tutorialStep >= _tutorialSteps.Length)
            {
                await CompleteTutorial();
                return;
            }

            UpdateTutorialStep();
        }

        private async void UpdateTutorialStep()
        {
            var step = _tutorialSteps[_tutorialStep];

            TutorialTitleLabel.Text = step.Title;
            TutorialMessageLabel.Text = step.Message;
            TutorialProgressLabel.Text = $"{_tutorialStep + 1}/{_tutorialSteps.Length}";

                         if (!string.IsNullOrEmpty(step.TargetElementName))
            {
                await PositionArrowToElement(step.TargetElementName);
            }
            else
            {
                ArrowPointer.Opacity = 0;
                                 PositionSpeechBubble(true);
            }

                         HighlightTargetElement(step.TargetElementName);
        }

        private async Task PositionArrowToElement(string elementName)
        {
                         VisualElement? targetElement = null;

            if (elementName == "AvatarGrid")
            {
                targetElement = AvatarGrid;
            }
            else if (elementName == "Avatar3")
            {
                                 targetElement = await GetAvatarItemAtIndex(2);
                if (targetElement == null)
                {
                                         targetElement = AvatarGrid;
                }
            }

            if (targetElement == null)
            {
                ArrowPointer.Opacity = 0;
                return;
            }

                         await Task.Delay(150);

                         var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            double screenHeight = displayInfo.Height / displayInfo.Density;
            double safeZone = 60;  
            Rect targetBounds = GetAbsolutePosition(targetElement);

            if (targetBounds == Rect.Zero) return;

                         double arrowWidth = 50;
            double arrowHeight = 50;
            double padding = 10;  
             
                         double yAbove = targetBounds.Top - arrowHeight - padding;

                         double yBelow = targetBounds.Bottom + padding;

                                      bool preferAbove = targetBounds.Center.Y > (screenHeight / 2);

            double finalArrowY;
            bool isArrowAbove;

            if (preferAbove)
            {
                                 if (yAbove >= safeZone)
                {
                    finalArrowY = yAbove;
                    isArrowAbove = true;
                }
                else
                {
                                         finalArrowY = yBelow;
                    isArrowAbove = false;
                }
            }
            else
            {
                                 if (yBelow + arrowHeight <= screenHeight - safeZone)
                {
                    finalArrowY = yBelow;
                    isArrowAbove = false;
                }
                else
                {
                                         finalArrowY = yAbove;
                    isArrowAbove = true;
                }
            }

                                      double arrowX = targetBounds.Center.X - (arrowWidth / 2);

            AbsoluteLayout.SetLayoutBounds(ArrowPointer, new Rect(arrowX, finalArrowY, arrowWidth, arrowHeight));
            AbsoluteLayout.SetLayoutFlags(ArrowPointer, AbsoluteLayoutFlags.None);

                                                   ArrowPointer.Rotation = isArrowAbove ? 90 : -90;

                                                   bool arrowIsAtTopHalf = finalArrowY < (screenHeight / 2);
            PositionSpeechBubble(!arrowIsAtTopHalf);

            await AnimateArrowPointer();
        }

        private async Task<VisualElement?> GetAvatarItemAtIndex(int index)
        {
                         try
            {
                                 await Task.Delay(200);

                                 var items = FindVisualChildren<Image>(AvatarGrid);
                var itemList = items.ToList();

                if (itemList.Count > index)
                {
                    return itemList[index];
                }
            }
            catch
            {
                             }

            return null;
        }

        private IEnumerable<T> FindVisualChildren<T>(Element element) where T : Element
        {
            if (element == null)
                yield break;

                         if (element is T t)
                yield return t;

                         foreach (var child in GetLogicalChildren(element))
            {
                foreach (var descendant in FindVisualChildren<T>(child))
                {
                    yield return descendant;
                }
            }
        }

        private IEnumerable<Element> GetLogicalChildren(Element element)
        {
            if (element is Layout layout)
            {
                foreach (var child in layout.Children.OfType<Element>())
                    yield return child;
            }
            else if (element is ContentView contentView && contentView.Content != null)
            {
                yield return contentView.Content;
            }
            else if (element is ScrollView scrollView && scrollView.Content != null)
            {
                yield return scrollView.Content;
            }
            else if (element is Frame frame && frame.Content != null)
            {
                yield return frame.Content;
            }
            else if (element is Border border && border.Content != null)
            {
                yield return border.Content;
            }
        }

        private void PositionSpeechBubble(bool positionAtTop)
        {
            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            double screenHeight = displayInfo.Height / displayInfo.Density;

                                                   double bubbleX = 160;

            double bubbleWidth = 350;
            double safePadding = 60;  
            if (positionAtTop)
            {
                                 AbsoluteLayout.SetLayoutBounds(SpeechBubbleContainer, new Rect(bubbleX, safePadding, bubbleWidth, AbsoluteLayout.AutoSize));
            }
            else
            {
                                                  AbsoluteLayout.SetLayoutBounds(SpeechBubbleContainer, new Rect(bubbleX, screenHeight - 200, bubbleWidth, AbsoluteLayout.AutoSize));
            }

            AbsoluteLayout.SetLayoutFlags(SpeechBubbleContainer, AbsoluteLayoutFlags.None);
        }

        private Rect GetAbsolutePosition(VisualElement element)
        {
            if (element == null) return Rect.Zero;

            double x = element.X;
            double y = element.Y;
            double width = element.Width;
            double height = element.Height;

            var current = element.Parent as VisualElement;

            while (current != null && !(current is Page))
            {
                x += current.X;
                y += current.Y;

                if (current is ScrollView scrollView)
                {
                    x -= scrollView.ScrollX;
                    y -= scrollView.ScrollY;
                }

                current = current.Parent as VisualElement;
            }

            return new Rect(x, y, width, height);
        }

        private async Task AnimateArrowPointer()
        {
            ArrowPointer.Opacity = 0;
            ArrowPointer.Scale = 0.8;

            await Task.Delay(100);

            await Task.WhenAll(
                ArrowPointer.FadeTo(1, 300, Easing.CubicOut),
                ArrowPointer.ScaleTo(1, 300, Easing.BounceOut)
            );

                         _ = Task.Run(async () =>
            {
                try
                {
                    while (ArrowPointer.Opacity > 0 && TutorialOverlay.IsVisible)
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            if (ArrowPointer.Opacity > 0 && TutorialOverlay.IsVisible)
                            {
                                await ArrowPointer.ScaleTo(1.2, 500, Easing.CubicInOut);
                                if (ArrowPointer.Opacity > 0 && TutorialOverlay.IsVisible)
                                {
                                    await ArrowPointer.ScaleTo(1.0, 500, Easing.CubicInOut);
                                }
                            }
                        });
                        await Task.Delay(100);
                    }
                }
                catch
                {
                                     }
            });
        }

        private void HighlightTargetElement(string? targetName)
        {
                         AvatarGrid.Opacity = 1;

            if (string.IsNullOrEmpty(targetName))
                return;

                     }

        private async Task CompleteTutorial()
        {
                         Preferences.Set(TUTORIAL_COMPLETED_KEY, true);

                         await Task.WhenAll(
                ArrowPointer.FadeTo(0, 200),
                SpeechBubbleContainer.FadeTo(0, 300),
                TarsierImage.FadeTo(0, 300),
                TutorialOverlay.FadeTo(0, 400)
            );

            TutorialOverlay.IsVisible = false;

                         AvatarGrid.Opacity = 1;
        }

        async void AvatarGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                         if (_isNavigating)
            {
                System.Diagnostics.Debug.WriteLine("Double-tap prevented: Already navigating to IndexPage.");
                AvatarGrid.SelectedItem = null;
                return;
            }

            if (e.CurrentSelection.FirstOrDefault() is not Avatar selected)
            {
                AvatarGrid.SelectedItem = null;
                return;
            }

                         _isNavigating = true;

            try
            {
                                 PointsOverlay.Scale = 0.9;
                PointsOverlay.Opacity = 0;
                PointsOverlay.IsVisible = true;

                await Task.WhenAll(
                    PointsOverlay.FadeTo(1, 350),
                    PointsOverlay.ScaleTo(1.05, 350, Easing.CubicOut)
                );

                await Task.Delay(2500);

                                 var existingChar = await App.Database.GetCharAsync();
                if (existingChar != null)
                {
                    existingChar.points = 50;
                    await App.Database.UpdateCharAsync(existingChar);
                }

                                 var fileName = Path.GetFileNameWithoutExtension(selected.ImageSource ?? "");
                var avatarId = string.IsNullOrWhiteSpace(fileName) ? "avatar1" : fileName;

                var set = new AvatarCostumeSet
                {
                    avatarid = avatarId,
                    avatarblueunlocked = false,
                    avatarblueredunlocked = false,
                    avatargreenunlocked = false,
                    avatarpinkunlocked = false,
                    avatarredunlocked = false,
                };

                await App.Database.SaveAvatarSetAsync(set);
                AvatarCustomizationHelper.SelectedAvatarSetId = set.avatarid;
                CharacterHelper.CurrentAvatar = AvatarCustomizationHelper.GetFirstCostumePathOrDefault(set.avatarid);
                await App.Database.UpdateCurrentAvatarAsync(CharacterHelper.CurrentAvatar);

                                 await Navigation.PushAsync(new IndexPage());
                Navigation.RemovePage(this);

                AvatarGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AvatarGrid_SelectionChanged: {ex}");
                _isNavigating = false;                  AvatarGrid.SelectedItem = null;
            }
                     }

        private class TutorialStep
        {
            public string Title { get; set; } = "";
            public string Message { get; set; } = "";
            public string? TargetElementName { get; set; }
        }
    }

    public class Avatar
    {
        public string ImageSource { get; }
        public Avatar(string imageSource) => ImageSource = imageSource;
    }
}