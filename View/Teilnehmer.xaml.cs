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
        ParticipantsListView.ItemsSource = _viewModel.Participants;
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
}

public class TeilnehmerViewModel : INotifyPropertyChanged
{
    private AppManager _manager;

    private ObservableCollection<DrawParticipant> participants;
    public ObservableCollection<DrawParticipant> Participants { get => participants; set { participants = value; OnPropertyChanged(); } }
    public string ExamName { get => _manager.CurrentExam?.Name ?? "Prüfungs Name"; }
    public TeilnehmerViewModel(AppManager manager)
    {
        _manager = manager;

        participants = new ObservableCollection<DrawParticipant>();

        // Mische die Teilnehmerliste anhand eines Seeds
        manager.Shuffle();
        for (int i = 0; i < (manager.Participants?.Count ?? 0); i++)
        {
            participants.Add(new DrawParticipant(manager.Participants[manager.ShuffleIndicies[i]], i));
        }
    }
    public async Task Next(DrawParticipant participant)
    {
        _manager.CurrentParticipant = _manager.Participants.FirstOrDefault(x => x.ID == participant.ParticipantID);
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
    public DrawParticipant(Participant participant, int index)
    {
        this.Index = index;
        this.ShowIndex = index + 1;
        this.Nachname = participant.Nachname;
        this.Vorname = participant.Vorname;
        this.ParticipantID = participant.ID;
    }
}