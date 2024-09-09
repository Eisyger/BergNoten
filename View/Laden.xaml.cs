using BergNoten.Helper;
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

    private async void OnClickedLaden(object sender, EventArgs e)
    {
        await viewModel.SucheDatei();
        if (viewModel.HasData)
        {
            await Shell.Current.GoToAsync("//Teilnehmer");
        }
        else
        {
            await DisplayAlert("Fehler", "Fehlerhafte Excel Datei.", "OK");
        }
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

        HasData = false;

        LadeConfig();
    }

    public async Task SucheDatei()
    {
        var result = await FilePicker.Default.PickAsync();

        if (result != null)
        {
            // Daten in UI anzeigen
            Zeile1 = "Daten geladen aus:";
            Zeile2 = result.FileName;

            // Config aktuallisieren
            _manager.Configurations.PathToData = result.FullPath;
            _manager.Configurations.FileName = result.FileName;

            // Lade die aktuellen Daten in die Datenbank            
            LadeDatei();
        }
        return;
    }

    private void LadeConfig()
    {
        var path = _manager.Configurations.PathToData;
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            _zeile1 = "Keine Daten geladen!";
            _zeile2 = string.Empty;
        }
        else
        {
            _zeile1 = "Daten geladen aus:";
            _zeile2 = _manager.Configurations.FileName;
        }
    }

    private void LadeDatei()
    {
        HasData = true;
        try
        {
            // TODO Neue Datenbank erstellen, die default datebank löschen!
            var data = IOExcel.ImportFromExcel(_manager.Configurations.PathToData);
            
            _manager.Database.AddParticipants(data[0]);
            _manager.Database.AddExams(data[1]);
        }
        catch (Exception e)
        {
            HasData = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChaged([CallerMemberName] string name = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}