using BergNoten.Helper;
using BergNoten.Model;
using Microsoft.Maui;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace BergNoten.View;

public partial class Pruefungen : ContentPage
{
    private PruefungenViewModel _viewModel;
    public Pruefungen(PruefungenViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;

        BindingContext = _viewModel;
    }

}

public class PruefungenViewModel : INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
