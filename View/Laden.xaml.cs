namespace BergNoten.View;

public partial class Laden : ContentPage
{
    LadenViewModel viewModel;
    public Laden()
    {
        InitializeComponent();
        viewModel = new LadenViewModel();
        BindingContext = viewModel;
    }

    private void OnClicked(object sender, EventArgs e)
    {
        viewModel.LadeDatei();
    }
}

public class LadenViewModel
{
    public LadenViewModel()
    {

    }

    public async void LadeDatei()
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
}