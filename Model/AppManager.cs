using BergNoten.Database;
using BergNoten.Helper;


namespace BergNoten.Model
{
    public class AppManager
    {
        private static readonly object _lock = new();
        private static AppManager? _instance;
        private DatabaseManager _data;
        private Config _config;

        public DatabaseManager Database { get { return _data; } set { _data = value; } }
        public Config Configurations { get { return _config; } set { _config = value; } }

        public Exam? CurrentExam { get; set; }
        public List<Exam> Exams { get; set; }
        public Participant? CurrentParticipant { get; set; }

        public List<Participant> Participants { get; set; }
        public int CurrentShuffleIndex { get; set; }

        private int[] _shuffleIndex;
        public int[] ShuffleIndicies => _shuffleIndex;


        public AppManager()
        {
            string pathDatabase;
            string pathConfig;

            //TODO Die Pfade später anpassen relativ zum jeweiligem Gerät.
            if (OperatingSystem.IsWindows())
            {
                pathDatabase = "D:\\BergNoten\\BergNoten\\Misc\\data.db";
                pathConfig = "D:\\BergNoten\\BergNoten\\Misc";
            }
            else
            {
                pathDatabase = Path.Combine(FileSystem.AppDataDirectory, "data.db");
                File.Delete(pathDatabase);

                pathConfig = FileSystem.AppDataDirectory;
            }


            // Initialisiere die Config, ist eine Config vorhanden wird sie geladen, wenn nicht wird eine neue erstellt.
            // In der Config werden alle Eigenschaften initialisiert, oder mit Werten aus der Datei überladen.
            _config = new Config(pathConfig);

            _data = new DatabaseManager(pathDatabase);

            Participants = _data.GetParticipants();
            Exams = _data.GetExams();


            _shuffleIndex = new int[Participants.Count];
            for (int i = 0; i < Participants.Count; i++)
            {
                _shuffleIndex[i] = i;
            }
        }

        public void WriteGrade(string note, string bemerkung)
        {
            _data.AddNote(new Grade(CurrentParticipant, CurrentExam, note, bemerkung));
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
