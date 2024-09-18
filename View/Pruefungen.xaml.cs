using BergNoten.Model;
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

        ExamsListView.ItemsSource = _viewModel.Exams;
        ExamsListView.SelectedItem = _viewModel.CurrentExam;

    }

    private async void OnItemTapped(object sender, TappedEventArgs e)
    {
        var exam = e.Parameter as Exam;
        if (exam != null)
        {
            ExamsListView.SelectedItem = exam;

            await _viewModel.SelectExam(exam);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Refresh();
    }
}

public class PruefungenViewModel : INotifyPropertyChanged
{
    private AppManager _manager;
    private ObservableCollection<Exam>? _exams;
    public ObservableCollection<Exam>? Exams { get => _exams; set { _exams = value; OnPropertyChanged(); } }
    public Exam? CurrentExam => _manager.CurrentExam;

    public PruefungenViewModel(AppManager manager)
    {
        _manager = manager;
        _exams = new ObservableCollection<Exam>();
        Refresh();
    }
    public void Refresh()
    {
        if (_manager.Exams == null)
        { return; }
        _exams.Clear();
        foreach (var ex in _manager.Exams)
        {
            _exams.Add(ex);
        }
    }
    public async Task SelectExam(Exam selectedExam)
    {
        _manager.CurrentExam = selectedExam;
        await Shell.Current.GoToAsync("//Teilnehmer");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
