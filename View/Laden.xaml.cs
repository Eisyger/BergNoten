namespace BergNoten.View;

public partial class Laden : ContentPage
{
    public Laden()
    {
        InitializeComponent();
    }

    private async void OnClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Pruefungen", true);
    }
}