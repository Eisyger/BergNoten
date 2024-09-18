using Bergnoten.Helper;
using BergNoten.Model;
using Microsoft.Maui.Layouts;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BergNoten.View;

public partial class Noten : ContentPage
{
    private AppManager _manager;

    private NotenViewModel _viewModel;
    public Noten(AppManager manager)
    {
        InitializeComponent();
        _manager = manager;
        Refresh();

    }

    private void Refresh()
    {
        _viewModel = new NotenViewModel(_manager);

        BindingContext = _viewModel;

        GradeSlider.Value = 0;
        GradeEntry.Text = _viewModel.Note;
    }

    private void GradeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (_viewModel.HasData)
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

    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_viewModel.HasData)
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
    }

    private void SignButton_Clicked(object sender, EventArgs e)
    {
        if (_viewModel.HasData)
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
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        if (_viewModel.HasData)
        {
            // Navigiere durch die ShuffleIndicies
            _viewModel.Next(-1);

            await this.Content.FadeTo(0, 500);
            Refresh();
            await this.Content.FadeTo(1, 500);
        }

    }

    private async void NextButton_Clicked(object sender, EventArgs e)
    {
        if (_viewModel.HasData)
        {
            // Navigiere durch die ShuffleIndicies
            _viewModel.Next(1);

            await this.Content.FadeTo(0, 500);
            Refresh();
            await this.Content.FadeTo(1, 500);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Refresh();
    }
}

public class NotenViewModel : INotifyPropertyChanged
{
    #region Properties
    private AppManager _manager;
    private Grade _grade;
    private Debouncer _bouncer;
    public bool HasData;

    private bool _hasNext;
    public bool HasNext
    {
        get => _hasNext;
        set
        {
            if (_hasNext != value)
            {
                _hasNext = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _hasBefore;
    public bool HasBefore
    {
        get => _hasBefore;
        set
        {
            if (_hasBefore != value)
            {
                _hasBefore = value;
                OnPropertyChanged();
            }
        }
    }

    private string? _pruefungsName;
    public string? PruefungsName
    {
        get => _pruefungsName;
        set
        {
            if (_pruefungsName != value)
            {
                _pruefungsName = value;
                OnPropertyChanged();
            }
        }
    }

    private string? _name;
    public string? Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    private string? _vorname;
    public string? Vorname
    {
        get => _vorname;
        set
        {
            if (_vorname != value)
            {
                _vorname = value;
                OnPropertyChanged();
            }
        }
    }

    private string? _verein;
    public string? Verein
    {
        get => _verein;
        set
        {
            if (_verein != value)
            {
                _verein = value;
                OnPropertyChanged();
            }
        }
    }

    private string? _nr;
    public string? NR
    {
        get => _nr;
        set
        {
            if (_nr != value)
            {
                _nr = value;
                OnPropertyChanged();
            }
        }
    }

    // TODO Standartwert der Note überarbeiten!
    private string? _note = "-";
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

    private string? _bemerkung;
    public string? Bemerkung
    {
        get => _bemerkung;
        set
        {
            if (_bemerkung != value)
            {
                _bemerkung = value;
                OnPropertyChanged();
            }
        }
    }

    private Color _bgColor = Color.FromArgb("#90EE90");
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

        InitProperties();

        _bouncer = new Debouncer(500);
    }

    public void InitProperties()
    {
        var index = _manager?.CurrentShuffleIndex + 1;

        _grade = _manager.GetCurrentGrade();
        if (_grade != null)
        {
            HasData = true;
            Name = _grade.Participant.Nachname;
            Vorname = _grade.Participant.Vorname;
            Note = _grade.Note;
            Bemerkung = _grade.Bemerkung;

            NR = (index).ToString();
        }
        else
        {
            HasData = false;
        }

        Verein = _manager?.CurrentParticipant?.Verein ?? "Keine Prüfung geladen";
        PruefungsName = _manager?.CurrentExam?.Name ?? "Kein Teilnehmer geladen";

        _hasBefore = true;
        _hasNext = true;

        if (index == 1)
        {
            _hasBefore = false;
            _hasNext = true;
        }
        else if (index == _manager?.Participants?.Count)
        {
            _hasNext = false;
            _hasBefore = true;
        }

    }
    public async void SaveGrade()
    {
        _grade.Note = Note;
        _grade.Bemerkung = Bemerkung;

        await _bouncer.Debounce(Save);
    }
    public void CheckBGColor()
    {
        if (Note == "-")
        {
            BGColor = Color.FromArgb("#90EE90");
            return;
        }
        // TODO Standartwert der Note überarbeiten!
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
    public void Next(int dir)
    {
        if (dir != 1 && dir != -1)
        {
            return;
        }

        var participants = _manager.Participants;

        // Berechne den Index des nächsten Teilnehmers
        int newIndex = _manager.CurrentShuffleIndex + dir;

        // Ist newIndex im gültigen Bereich?
        if (newIndex >= 0 && newIndex < _manager.Participants.Count)
        {
            _manager.CurrentParticipant = participants[_manager.ShuffleIndicies[newIndex]];
            _manager.CurrentShuffleIndex = newIndex;
            HasBefore = true;
            HasNext = true;
        }
    }
    private async Task Save()
    {
        _manager.UpdateGrade(_grade);
        await Task.CompletedTask;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}