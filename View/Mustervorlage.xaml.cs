using BergNoten.Helper;
using CommunityToolkit.Maui.Storage;

namespace BergNoten.View;

public partial class Mustervorlage : ContentPage
{
    public Mustervorlage()
    {
        InitializeComponent();
    }

    private async void OnClicked(object sender, EventArgs e)
    {
        var path = await FolderPicker.Default.PickAsync(CancellationToken.None);
        if (path.IsSuccessful)
        {
            TestIOExcel.Run(Path.Combine(path.Folder.Path, "TEST_Data.xls"), false);
        }
    }
}