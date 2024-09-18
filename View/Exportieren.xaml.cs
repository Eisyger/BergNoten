using BergNoten.Helper;
using BergNoten.Model;
using CommunityToolkit.Maui.Storage;

namespace BergNoten.View;

public partial class Exportieren : ContentPage
{
    private AppManager _manager;
    public Exportieren(AppManager manager)
    {
        InitializeComponent();
        _manager = manager;
    }

    private async void OnClickedExport(object sender, EventArgs e)
    {
        try
        {
            var pickerResult = await FolderPicker.Default.PickAsync(CancellationToken.None);
            if (pickerResult.IsSuccessful)
            {
                _manager.Export(pickerResult.Folder.Path);
            }
        }
        catch (NullReferenceException ex)
        {
            await DisplayAlert("Fehler", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fehler", ex.Message, "OK");
        }
    }
}