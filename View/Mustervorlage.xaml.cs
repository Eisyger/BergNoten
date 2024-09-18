using BergNoten.Helper;
using CommunityToolkit.Maui.Storage;
using Org.BouncyCastle.Asn1.Mozilla;

namespace BergNoten.View;

public partial class Mustervorlage : ContentPage
{
    public Mustervorlage()
    {
        InitializeComponent();
        BindingContext = new MustervorlageViewModel();
    }

    private async void OnClicked(object sender, EventArgs e)
    {
        var path = await FolderPicker.Default.PickAsync(CancellationToken.None);
        if (path.IsSuccessful)
        {
            TestIOExcel.Run(Path.Combine(path.Folder.Path, $"BergNoten - Vorlage - {DateTime.Now.ToString("yy-MM-dd HH-mm")}.xls"), false);
        }
    }
}

public class MustervorlageViewModel
{
    public string Einleitungstext { get; set; }

    public MustervorlageViewModel()
    {
        Einleitungstext = "Exportiere eine Excel Vorlage an einen ausgewählten Speicherort. " +
            "Hier kann dann die Teilnehmerliste und die Prüfungen in Excel ergänzt werden. " +
            "Wenn ihr fertig seid, teilt diese Excel-Datei mit eurem Team. Das Dateiformat .xls " +
            "muss beibehalten werden!";
    }
}