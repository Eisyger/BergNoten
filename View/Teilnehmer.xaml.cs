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

        // TODO Warum geht die Scrollview nicht in Windows?
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await _viewModel.Next(e, sender);
    }
}

public class TeilnehmerViewModel : INotifyPropertyChanged
{
    private AppManager _manager;

    private ObservableCollection<DrawParticipant> participants;
    public ObservableCollection<DrawParticipant> Participants { get { return participants; } }
    public string ExamName { get { return _manager.CurrentExam?.Name ?? "Prüfungs Name"; } }

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

    public async Task Next(SelectionChangedEventArgs e, object sender)
    {
        if (e.CurrentSelection.Count == 1)
        {
            // Ermittle den Shuffle Index der Selection
            int index = ((DrawParticipant)e.CurrentSelection[0]).Index;
            // Dieser Index repräsentiert den aktuellen Index in dem ShuffleIndicies-Array,
            // zur Navigation für durch das Array
            _manager.CurrentShuffleIndex = index;
            // Ermittle den CurrentParticipant anhand des aktuellen Shuffle Index
            _manager.CurrentParticipant = _manager.Participants[_manager.ShuffleIndicies[index]];
            // Wechsel auf die ExamsPage
            await Shell.Current.GoToAsync("//Noten");
        }
        // Nach dem auswählen die Auswahl aufheben. Dies löst tatsächlich das Event nochmal aus
        // aber die Pürfung, ob die Anzahl der ausgewählten Elemente größer als 0 ist, macht das weg.
        // Evtl gibt es eine elegantere lösung, zumal ja nur Selection und nicht Taps das Event auslösen.
            ((CollectionView)sender).SelectedItem = null;
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