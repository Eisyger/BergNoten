using BergNoten.Database;
using BergNoten.Helper;


namespace BergNoten.Model
{
    public class AppManager
    {
        private DatabaseManager? _data;
        private Config _config;

        public DatabaseManager? Database { get { return _data; } set { _data = value; } }
        public Config Configurations { get { return _config; } set { _config = value; } }

        public Exam? CurrentExam { get; set; }
        public List<Exam>? Exams { get; set; }
        public Participant? CurrentParticipant { get; set; }

        public List<Participant>? Participants { get; set; }
        public int CurrentShuffleIndex { get; set; }

        private int[] _shuffleIndex;
        public int[] ShuffleIndicies => _shuffleIndex;


        public AppManager()
        {
            //Lösche alte Datenbanken
            DatabaseGarbageCollection.Clean(7);

            string pathConfig = Path.Combine(FileSystem.Current.AppDataDirectory, "config.json");

            // Initialisiere die Config, ist eine Config vorhanden wird sie geladen, wenn nicht wird eine neue erstellt.
            // In der Config werden alle Eigenschaften initialisiert, oder mit Werten aus der Datei überladen.
            _config = new Config(pathConfig);

            // Erstellt eine default Datenbank, sobald Daten aus einer .xls gealden werden wird eine neue Datenbank erstellt.                        
            CreateDatabase("default.db", isDefault: true);

            // Initialisere das ShuffleIndex Array
            _shuffleIndex = new int[Participants?.Count ?? 0];
            for (int i = 0; i < (Participants?.Count ?? 0); i++)
            {
                _shuffleIndex[i] = i;
            }
        }

        public void CreateDatabase(string name, bool isDefault = false)
        {
            string pathDatabase;

            if (isDefault)
            {
                // Default Database erstellen ohne Datum am Ende.
                pathDatabase = Path.Combine(FileSystem.Current.AppDataDirectory, name);
            }
            else
            {
                // Datum am Ende des Dateinamens hinzufügen
                pathDatabase = Path.Combine(FileSystem.Current.AppDataDirectory,
                    name.Split(".")[0] + " " + DateTime.Now.ToString("yyyy-MM-dd HH-mm") + ".db");
            }

            // Erstelle die Database
            _data = new DatabaseManager(pathDatabase);

            // Daten laden aus Excel
            var dataFromExcel = IOExcel.ImportFromExcel(_config.PathToData);

            // Extrahieren der Daten und einfügen in die Table der Database
            _data.AddParticipants(dataFromExcel[0]);
            _data.AddExams(dataFromExcel[1]);

            // Setze die Eigenschaften mit den Database Daten
            Participants = _data.GetParticipants();
            Exams = _data.GetExams();

            CurrentParticipant = Participants?[0];
            CurrentExam = Exams?[0];
        }

        public void WriteGrade(string note, string bemerkung)
        {
            _data?.AddNote(new Grade(CurrentParticipant, CurrentExam, note, bemerkung));
        }
        public Grade GetGrade()
        {
            var grades = _data.GetGrades();
            var existing_grade = grades.Where(x => x.ID_Participant == CurrentParticipant.ID && x.ID_Exam == CurrentExam.ID).FirstOrDefault();
            if (existing_grade == null)
            {
                var new_grade = new Grade(CurrentParticipant, CurrentExam);
                //Füge die Note der Datenbank hinzu
                _data.AddNote(new_grade);
                return new_grade;
            }
            else
            {
                existing_grade.Participant = Participants.First(p => p.ID == existing_grade.ID_Participant);
                existing_grade.Exam = _data.GetExams().First(e => e.ID == existing_grade.ID_Exam);
                return existing_grade;
            }
        }
        public void UpdateGrade(Grade grade)
        {
            _data.UpdateGrade(grade);
        }
        public void Shuffle()
        {
            int seed = CurrentExam.ID + 1337;

            Random seededRandom = new Random(seed);

            for (int i = 0; i < _shuffleIndex.Length; i++)
            {
                // Wähle einen zufälligen Index aus dem Bereich 0 bis i
                int j = seededRandom.Next(0, _shuffleIndex.Length);

                // Tausche die Elemente an den Indizes i und j
                var temp = _shuffleIndex[i];
                _shuffleIndex[i] = _shuffleIndex[j];
                _shuffleIndex[j] = temp;
            }
        }
    }
}
