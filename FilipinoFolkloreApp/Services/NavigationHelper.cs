namespace FilipinoFolkloreApp.Services;

using FilipinoFolkloreApp.Views.Home;

public static class NavigationHelper
{
    public static async Task NavigateToIndexPage(INavigation navigation)
    {
        // Get all pages in the stack
        var pages = navigation.NavigationStack.ToList();
        
        // Find existing IndexPage
        var indexPage = pages.OfType<IndexPage>().FirstOrDefault();
        
        if (indexPage != null)
        {
            // Calculate how many pages to pop to reach IndexPage
            int indexPagePosition = pages.IndexOf(indexPage);
            int pagesToPop = pages.Count - indexPagePosition - 1;
            
            // Pop all pages until we reach IndexPage
            for (int i = 0; i < pagesToPop; i++)
            {
                await navigation.PopAsync(false); // false = no animation for faster execution
            }
        }
        else
        {
            // No IndexPage exists, remove all pages except the first and push IndexPage
            while (navigation.NavigationStack.Count > 1)
            {
                navigation.RemovePage(navigation.NavigationStack[navigation.NavigationStack.Count - 2]);
            }
            
            await navigation.PushAsync(new IndexPage());
        }
    }
}