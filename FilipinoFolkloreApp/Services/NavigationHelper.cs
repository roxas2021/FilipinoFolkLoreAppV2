namespace FilipinoFolkloreApp.Services;

using FilipinoFolkloreApp.Views.Home;

public static class NavigationHelper
{
    public static async Task NavigateToIndexPage(INavigation navigation)
    {
        var pages = navigation.NavigationStack.ToList();
        
        var indexPage = pages.OfType<IndexPage>().FirstOrDefault();
        
        if (indexPage != null)
        {
            int indexPagePosition = pages.IndexOf(indexPage);
            int pagesToPop = pages.Count - indexPagePosition - 1;
            
            for (int i = 0; i < pagesToPop; i++)
            {
                await navigation.PopAsync(false); 
            }
        }
        else 
        {
            
            while (navigation.NavigationStack.Count > 1)
            {
                navigation.RemovePage(navigation.NavigationStack[navigation.NavigationStack.Count - 2]);
            }
            
            await navigation.PushAsync(new IndexPage());
        }
    }
}