namespace FilipinoFolkloreApp.Views.Home;

public partial class IndexPage : ContentPage
{
	public IndexPage()
	{
		InitializeComponent();

        var data = App.Database.GetCharByIdAsync(1);
    }
}       