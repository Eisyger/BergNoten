using Bergnoten.Helper;
using BergNoten.Model;
using NPOI.SS.Formula.Functions;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;

namespace BergNoten.View;

public partial class Noten : ContentPage
{
    private NotenViewModel _viewModel;
    public Noten(NotenViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;

        BindingContext = _viewModel;
    }

    private void GradeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        var slider = sender as Slider;
        slider.Value = Math.Round(slider.Value / 0.5) * 0.5;
        if (slider.Value >= 1 && slider.Value >= 1)
        {
            _viewModel.Note = slider.Value.ToString("F1");
            GradeEntry.Text = "";

            _viewModel.SaveGrade();
        }

    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        if (double.TryParse(entry.Text, out double zahl))
        {
            if (zahl >= 1 && zahl <= 6)
            {
                _viewModel.Note = zahl.ToString("F1");
                GradeSlider.Value = GradeSlider.Minimum;

                _viewModel.SaveGrade();
            }
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        if (_viewModel.Note.Contains('-'))
        {
            _viewModel.Note = _viewModel.Note.Split('-')[0];
            _viewModel.SaveGrade();
        }
        else
        {
            _viewModel.Note = _viewModel.Note + "-";
            _viewModel.SaveGrade();
        }
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await _viewModel.Next(-1);
    }

    private async void NextButton_Clicked(object sender, EventArgs e)
    {
        await _viewModel.Next(1);
    }
}

public class NotenViewModel : INotifyPropertyChanged
{
    #region Properties
    private AppManager _manager;
    private Grade _grade;
    private string _note = "-";
    private Color _bgColor = Color.FromArgb("#90EE90");
    private Debouncer _bouncer;
    public string? Name { get; set; }
    public string? Vorname { get; set; }
    public string? Verein { get; set; }
    public string? NR { get; set; }
    public string? Note
    {
        get => _note;
        set
        {
            if (_note != value)
            {
                _note = value;
                OnPropertyChanged();
                CheckBGColor();
            }
        }
    }
    public string? Bemerkung { get; set; }
    public Color BGColor
    {
        get => _bgColor;
        set
        {
            if (_bgColor != value)
            {
                _bgColor = value;
                OnPropertyChanged();
            }
        }
    }
    #endregion

    public NotenViewModel(AppManager manager)
    {
        _manager = manager;

        _grade = _manager.GetGrade();
        Name = _grade.Participant.Nachname;
        Vorname = _grade.Participant.Vorname;
        Verein = _manager?.CurrentParticipant?.Verein;
        NR = (_manager?.CurrentShuffleIndex + 1).ToString();
        Bemerkung = _grade.Bemerkung;
        Note = _grade.Note;

        _bouncer = new Debouncer(500);
    }
    public async void SaveGrade()
    {
        _grade.Note = Note;
        _grade.Bemerkung = Bemerkung;

        await _bouncer.Debounce(Save);
    }
    public void CheckBGColor()
    {
        double n = double.Parse(Note.Split('-')[0]);
        if (n >= 1 && n <= 3.5)
        {
            BGColor = Color.FromArgb("#90EE90");
        }
        else if (n >= 4 && n <= 4.5)
        {
            BGColor = Color.FromArgb("#FFD700");
        }
        else if (n >= 5 && n <= 6)
        {
            BGColor = Color.FromArgb("#FF4500");
        }
    }
    public async Task Next(int dir)
    {
        if (dir != 1 || dir != -1)
        {
            return;
        }

        var participants = _manager.Participants;

        // Berechne den Index des nächsten Teilnehmers
        int newIndex = _manager.CurrentShuffleIndex + dir;

        if (newIndex >= 0)
        {
            _manager.CurrentParticipant = participants[_manager.ShuffleIndicies[newIndex]];
            _manager.CurrentShuffleIndex = newIndex;
            await Shell.Current.GoToAsync("//Noten");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private async Task Save()
    {
        _manager.UpdateGrade(_grade);
        await Task.CompletedTask;
    }
}