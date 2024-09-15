using BergNoten.Helper;
using BergNoten.Model;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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
}

public class PruefungenViewModel : INotifyPropertyChanged
{
    private AppManager _manager;
    private List<Exam>? _exams;
    public List<Exam>? Exams { get => _exams; set { _exams = value; OnPropertyChanged(); } }
    public Exam? CurrentExam => _manager.CurrentExam;

    public PruefungenViewModel(AppManager manager)
    {
        _exams = null;
        _manager = manager;
        Exams = _manager.Exams;
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
