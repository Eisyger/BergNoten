using BergNoten.Model;

namespace BergNoten.View;

public partial class Username : ContentPage
{
    private readonly AppManager _manager;
    public Username(AppManager m)
    {
        InitializeComponent();
        _manager = m;

        // Prüfe ob ein Username vorhanden ist in der Config, wenn ja 
        // übersrpringe diese Page
        if (m.Configurations.Username != string.Empty)
        {
            nameEntry.Text = m.Configurations.Username;
            NextPage();
        }
    }

    private void OnNameEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        nameButton.IsEnabled = !string.IsNullOrWhiteSpace(e.NewTextValue);
    }

    private void OnNameButtonClicked(object sender, EventArgs e)
    {
        _manager.Configurations.Username = nameEntry.Text;
        NextPage();
    }

    private async void NextPage()
    {
        await Shell.Current.GoToAsync("//Laden");
    }
}