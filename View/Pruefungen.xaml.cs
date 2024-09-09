using BergNoten.Helper;
using Microsoft.Maui;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace BergNoten.View;

public partial class Pruefungen : ContentPage
{
    ObservableCollection<TextHelper> list;
    private string pruefung;
    public string Pruefung
    {
        get => pruefung;
        set
        {
            if (pruefung != value)
            {
                pruefung = value;
                OnPropertyChanged();
            }
        }
    }

    public Pruefungen()
    {
        InitializeComponent();

        list = new() { new TextHelper() { Text = "Prüfung 1" },
                       new TextHelper() { Text = "Prüfung 2" },
                       new TextHelper() { Text = "Prüfung 3" },
                       new TextHelper() { Text = "Prüfung 4" },
                       new TextHelper() { Text = "Prüfung 5" }
                       };

        Pruefung = string.Empty;

        ListView1.ItemsSource = list;

        BindingContext = this;
    }

    private void OnItemTapped(object sender, EventArgs e)
    {
        // Der gesendete StackLayout ist der übergeordnete Container
        var stackLayout = (StackLayout)sender;
        var item = (TextHelper)stackLayout.BindingContext;
        ListView1.SelectedItem = item;

        Pruefung = item.Text;

        DatabaseGarbageCollection.Clean(7);
    }

}
class TextHelper : INotifyPropertyChanged
{
    private string text;

    public string Text
    {
        get => text;
        set
        {
            if (text != value)
            {
                text = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}