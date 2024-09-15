using BergNoten.Interfaces;
using BergNoten.Model;
using SQLite;


namespace BergNoten.Database
{
    /// <summary>
    /// Verwaltet die Datenbankoperationen für Teilnehmer und Prüfungen.
    /// </summary>
    public class DatabaseManager
    {
        private readonly SQLiteConnection _database;
        private bool _hasData = false;
        public bool HasData => _hasData;

        /// <summary>
        /// Initialisiert eine Datenbank und erstellt Tabellen für Teilnehmer und Prüfungen.
        /// Doppelte Einträge werden bei der Initialisierung entfernt. Öffnet die Datenebank wenn vorhanden,
        /// wenn nicht wird eine neue Datenbank erstellt.
        /// </summary>
        /// <param name="path">Der Pfad zur Datenbankdatei.</param>
        public DatabaseManager(string path)
        {
            try
            {
                _database = new SQLiteConnection(path);

            }
            catch (Exception ex)
            {
                throw new FileNotFoundException($"Der Pfad existiert nicht: {path}", path, ex);
            }
            _database.CreateTable<Participant>();
            _database.CreateTable<Exam>();
            _database.CreateTable<Grade>();
            EraseDuplicates();
        }

        #region Private Methods
        /// <summary>
        /// Entfernt doppelte Einträge von Teilnehmern und Prüfungen beim Initialisieren der Datenbank.
        /// </summary>
        private void EraseDuplicates()
        {
            RemoveDuplicateParticipants();
            RemoveDuplicateExams();
        }

        private void RemoveDuplicateParticipants()
        {
            var duplicates = new List<Participant>();
            var participants = GetParticipants();

            if (participants != null)
            {
                foreach (var p in participants)
                {
                    if (p != null)
                    {
                        _hasData = true;
                    }

                    if (participants.Count(x => x.Equals(p)) > 1)
                    {
                        duplicates.Add(p);
                    }
                }

                duplicates.ForEach(x => RemoveParticipant(x));
            }
        }

        private void RemoveDuplicateExams()
        {
            var duplicates = new List<Exam>();
            var exams = GetExams();

            if (exams != null)
            {
                foreach (var e in exams)
                {
                    if (e != null)
                    {
                        _hasData = true;
                    }

                    if (exams.Count(x => x.Equals(e)) > 1)
                    {
                        duplicates.Add(e);
                    }
                }

                duplicates.ForEach(x => RemoveExams(x));
            }
        }

        private void RemoveExams(Exam exam)
        {
            _database.Delete(exam);
        }
        #endregion

        #region Exam
        /// <summary>
        /// Gibt eine Liste aller Prüfungen aus der Datenbank zurück.
        /// </summary>
        /// <returns>Alle Prüfungen als Liste.</returns>
        public List<Exam> GetExams()
        {
            return new List<Exam>(_database.Table<Exam>());
        }

        /// <summary>
        /// Fügt eine Name zur Datenbank hinzu, wenn sie nicht bereits existiert und Daten enthält.
        /// </summary>
        /// <param name="exam">Die hinzuzufügende Name.</param>
        public void AddExam(Exam exam)
        {
            if (exam != null && Exam.IsNotEmpty(exam) && !_database.Table<Exam>().Any(e => e.Equals(exam)))
            {
                _database.Insert(exam);
                _hasData = true;
            }
        }

        public void AddExams(IEnumerable<IExportable> exams)
        {
            var examsList = exams.Where(e => e is Exam).Cast<Exam>().ToList();

            foreach (var exam in examsList)
            {
                AddExam(exam);
            }
        }
        #endregion

        #region Participant
        /// <summary>
        /// Gibt eine Liste aller Teilnehmer aus der Datenbank zurück.
        /// </summary>
        /// <returns>Alle Teilnehmer als Liste.</returns>
        public List<Participant> GetParticipants()
        {
            return new List<Participant>(_database.Table<Participant>());
        }

        /// <summary>
        /// Entfernt einen Teilnehmer aus der Datenbank.
        /// </summary>
        /// <param name="participant">Der zu entfernende Teilnehmer.</param>
        public void RemoveParticipant(Participant participant)
        {
            _database.Delete(participant);
        }

        /// <summary>
        /// Fügt einen neuen Teilnehmer zur Datenbank hinzu, wenn er nicht bereits existiert und Daten enthält.
        /// </summary>
        /// <param name="participant">Der hinzuzufügende Teilnehmer.</param>
        public void AddParticipant(Participant participant)
        {
            if (participant != null && Participant.IsNotEmpty(participant) && !_database.Table<Participant>().Any(p => p.Equals(participant)))
            {
                _database.Insert(participant);
                _hasData = true;
            }
        }

        /// <summary>
        /// Fügt eine Liste von exportierbaren Objekten zur Datenbank hinzu (nur Teilnehmer werden berücksichtigt).
        /// </summary>
        /// <param name="participants">Die hinzuzufügenden Teilnehmer.</param>
        public void AddParticipants(IEnumerable<IExportable> participants)
        {
            var participantList = participants.Where(p => p is Participant).Cast<Participant>().ToList();

            foreach (Participant participant in participantList)
            {
                AddParticipant(participant);
            }
        }
        #endregion

        #region Note
        public void AddNote(Grade note)
        {
            if (note != null && !_database.Table<Grade>().Any(n => n.Equals(note)))
            {
                _database.Insert(note);
            }
        }
        public List<Grade> GetGrades()
        {
            return new List<Grade>(_database.Table<Grade>());
        }
        public void UpdateGrade(Grade grade)
        {
            _database.Update(grade);
        }
        #endregion

        /// <summary>
        /// Schreibt alle Noten in eine Textdatei.
        /// </summary>
        public void Dump()
        {
            string filePath = "D:\\C# - MAUI\\AusbilderAppViews\\Model\\NotenPrint.txt";

            using var writer = new StreamWriter(filePath, append: false);
            var noten = new List<Grade>(_database.Table<Grade>());
            foreach (var note in noten)
            {
                string line = $"{note.Participant.Vorname} {note.Participant.Nachname} - {note.Exam.Name}: {note.Note}";
                writer.WriteLine(line);
            }
        }
    }
}