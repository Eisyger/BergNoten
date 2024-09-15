﻿using BergNoten.Database;
using BergNoten.Helper;


namespace BergNoten.Model
{
    public class AppManager
    {
        private DatabaseManager? _data;
        private Config _config;
        private Exam _currentExam;
        public DatabaseManager? Database { get { return _data; } set { _data = value; } }
        public Config Configurations { get { return _config; } set { _config = value; } }

        public Exam? CurrentExam
        {
            get => _currentExam; set { _currentExam = value; Shuffle(); }
        }
        public List<Exam>? Exams { get; set; }

        public Participant? CurrentParticipant { get; set; }
        public List<Participant>? Participants { get; set; }
        public int CurrentShuffleIndex { get; set; }

        private int[] _shuffleIndicies;
        public int[] ShuffleIndicies => _shuffleIndicies;


        public AppManager()
        {
            string pathConfig = Path.Combine(FileSystem.Current.AppDataDirectory, "config.json");

            // Initialisiere die Config, ist eine Config vorhanden wird sie geladen, wenn nicht wird eine neue erstellt.
            // In der Config werden alle Eigenschaften initialisiert, oder mit Werten aus der Datei überladen.
            _config = new Config(pathConfig);

            // Lösche alte Datenbanken welche älter als 7 Tage sind.
            DatabaseGarbageCollection.Clean(1);
            // Erstellt eine default Datenbank, sobald Daten aus einer .xls gealden werden wird eine neue Datenbank erstellt.                        
            CreateDatabase("default.db", isDefault: true);

            InitShuffleIndicies();
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
                // Database erstellen mit dem Namen der geladenen .xls Datei
                pathDatabase = Path.Combine(FileSystem.Current.AppDataDirectory, name.Split(".")[0] + ".db");
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
            if (_shuffleIndicies == null)
            {
                return;
            }

            InitShuffleIndicies();

            // Erstelle ein Random Exemplar mit einem Seed.             
            Random seededRandom = new Random(GetSeed());

            for (int i = 0; i < _shuffleIndicies.Length; i++)
            {
                // Wähle einen zufälligen Index aus dem Bereich 0 bis i
                int j = seededRandom.Next(0, _shuffleIndicies.Length);

                // Tausche die Elemente an den Indizes i und j
                var temp = _shuffleIndicies[i];
                _shuffleIndicies[i] = _shuffleIndicies[j];
                _shuffleIndicies[j] = temp;
            }
        }
        private void InitShuffleIndicies()
        {
            // Initialisere das ShuffleIndex Array von 0 bis n aufsteigend
            _shuffleIndicies = new int[Participants?.Count ?? 0];
            for (int i = 0; i < (Participants?.Count ?? 0); i++)
            {
                _shuffleIndicies[i] = i;
            }
        }

        /// <summary>
        /// Erstelle den Seed mit hilfe der CurrentExam ID.
        /// Der Seed ist für alle Benutzer der App gleich, sofern die Position und Name der Prüfung
        /// in der .xls die geladen wurde, gleich sind.
        /// </summary>
        /// <returns></returns>
        public int GetSeed()
        {
            int seed = 1337;

            foreach (var c in _currentExam.Name)
            {
                seed += Convert.ToInt32(c);
            }
            return seed + 42 + (_currentExam.ID * 42);
        }
    }
}
