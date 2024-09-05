using BergNoten.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BergNoten.View;

public partial class Laden : ContentPage
{
    LadenViewModel viewModel;
    public Laden(LadenViewModel vm)
    {
        InitializeComponent();
        viewModel = vm;
        BindingContext = viewModel;
    }

    private void OnClickedLaden(object sender, EventArgs e)
    {
        viewModel.Zeile1 = "asdfsdf";
    }

    private void OnClickedNeu(object sender, EventArgs e)
    {
        viewModel.SucheDatei();

    }
}

public class LadenViewModel : INotifyPropertyChanged
{
    #region Private Member
    private AppManager _manager;
    private string _zeile1, _zeile2;
    #endregion

    #region Public Member
    public bool HasData;
    public string Zeile1
    {
        get { return _zeile1; }
        set { _zeile1 = value; OnPropertyChaged(); }
    }
    public string Zeile2
    {
        get { return _zeile2; }
        set { _zeile2 = value; OnPropertyChaged(); }
    }
    #endregion

    public LadenViewModel(AppManager manager)
    {
        _manager = manager;
        _zeile1 = string.Empty;
        _zeile2 = string.Empty;

        LadeConfig();
    }

    private void LadeConfig()
    {
        var path = _manager.Configurations.PathToData;
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            _zeile1 = "Keine Daten geladen!";
            _zeile2 = string.Empty;
            HasData = false;
        }
        else
        {
            _zeile1 = "Vorhandene Daten:";
            _zeile2 = _manager.Configurations.PathToData;
            HasData = true;
        }
    }

    public async void SucheDatei()
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Bitte wählen Sie eine .xls oder .db aus",
            FileTypes = null
        });

        if (result != null)
        {

        }
        return;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChaged([CallerMemberName] string name = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}