using BergNoten.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BergNoten.View;

public partial class Teilnehmer : ContentPage
{
    private TeilnehmerViewModel _viewModel;
    public Teilnehmer(TeilnehmerViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;

        BindingContext = _viewModel;
        ParticipantsListView.ItemsSource = _viewModel.ViewParticipantsList;
    }

    private async void OnItemTapped(object sender, TappedEventArgs e)
    {
        var participant = e.Parameter as DrawParticipant;

        if (participant != null)
        {
            ParticipantsListView.SelectedItem = participant;
            await _viewModel.Next(participant);
        }
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Init();
    }
}

public class TeilnehmerViewModel : INotifyPropertyChanged
{
    private AppManager _manager;

    private ObservableCollection<DrawParticipant> _viewParticipantsList;
    public ObservableCollection<DrawParticipant> ViewParticipantsList { get => _viewParticipantsList; set { _viewParticipantsList = value; OnPropertyChanged(); } }

    private string _pruefungsName;
    public string PruefungsName
    {
        get => _pruefungsName;
        set
        {
            if (value != _pruefungsName)
            {
                _pruefungsName = value;
                OnPropertyChanged();
            }
        }
    }

    public TeilnehmerViewModel(AppManager manager)
    {
        _manager = manager;
        _viewParticipantsList = new ObservableCollection<DrawParticipant>();
        _pruefungsName = _manager.CurrentExam.Name;
        Init();
    }

    public void Init()
    {
        SetRandomPositionsForParticipants();
        PruefungsName = _manager.CurrentExam.Name;
    }

    private void SetRandomPositionsForParticipants()
    {
        _viewParticipantsList.Clear();
        // Fülle die _viewParticipantsList mit neuen Exemplaren
        // Die Reihenfolge wird von den ShuffleIndicies bestimmt!
        for (int i = 0; i < (_manager.Participants?.Count ?? 0); i++)
        {
            var participant = new DrawParticipant(_manager.Participants[_manager.ShuffleIndicies[i]], i);
            participant.Note = _manager.GetGrade(participant.ParticipantID, _manager.CurrentExam.ID).Note;
            _viewParticipantsList.Add(participant);
        }
    }

    public async Task Next(DrawParticipant participant)
    {
        _manager.CurrentParticipant = _manager.Participants.FirstOrDefault(x => x.ID == participant.ParticipantID);
        _manager.CurrentShuffleIndex = participant.Index;
        await Shell.Current.GoToAsync("//Noten");
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class DrawParticipant
{
    public int Index { get; set; }
    public int ShowIndex { get; set; }
    public string Nachname { get; set; }
    public string Vorname { get; set; }
    public int ParticipantID { get; set; }

    private string _note;
    public string Note
    {
        get => _note; set
        {
            if (value != _note)
            {
                _note = value; OnPropertyChanged();
            }
        }
    }
    public DrawParticipant(Participant participant, int index)
    {
        this.Index = index;
        this.ShowIndex = index + 1;
        this.Nachname = participant.Nachname;
        this.Vorname = participant.Vorname;
        this.ParticipantID = participant.ID;
        this._note = string.Empty;
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}