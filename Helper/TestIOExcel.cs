using BergNoten.Interfaces;
using BergNoten.Model;

namespace BergNoten.Helper
{
    public class TestIOExcel

    {
        public static void Run(string filePath, bool saveGrades)
        {
            // TODO Hier noch mehrere Teilnehmer erstellen!


            // Erstellen von Teilnehmern
            var participants = new List<Participant>
            {
                new() { ID = 1, Vorname = "Jessica", Nachname = "Alba", Geburtsdatum = "28.04.1981", Verein = "SV Los Angeles" },
                new() { ID = 2, Vorname = "Angelina", Nachname = "Jolie", Geburtsdatum = "04.06.1975", Verein = "TSV Hollywood" },
                new() { ID = 3, Vorname = "Brad", Nachname = "Pitt", Geburtsdatum = "18.12.1963", Verein = "FC Springfield" },
                new() { ID = 4, Vorname = "Michaela", Nachname = "Schaffrath", Geburtsdatum = "06.12.1970", Verein = "SV Frankfurt" },
                new() { ID = 5, Vorname = "Megan", Nachname = "Fox", Geburtsdatum = "16.05.1986", Verein = "TSG Malibu" },
                new() { ID = 6, Vorname = "Johnny", Nachname = "Cash", Geburtsdatum = "26.02.1932", Verein = "VfL Nashville" },
                new() { ID = 7, Vorname = "Markus", Nachname = "Söder", Geburtsdatum = "05.01.1967", Verein = "FC Bayern München" },
                new() { ID = 8, Vorname = "Wolfgang", Nachname = "Schäfer", Geburtsdatum = "14.06.1945", Verein = "SC Wildeck" },  // Wildecker Herzbuben
                new() { ID = 9, Vorname = "Wilfried", Nachname = "Gliem", Geburtsdatum = "10.09.1946", Verein = "SC Wildeck" }   // Wildecker Herzbuben
            };

            // Erstellen von Prüfungsfahrten
            var exams = new List<Exam>
            {
                new() { ID = 1, Name = "Mittlere Kurven, geschnitten", Beschreibung = "Tempokontrolle, geschnitten, Korridor" },
                new() { ID = 2, Name = "Kurzschwung", Beschreibung = "Tempokontrolle, runde Kurven, Stockeinsatz, früher Druck auf dem Außenski" },
                new() { ID = 3, Name = "Pflug", Beschreibung = "Pflugstellung, langsamens Tempo, demonstratives beugen und strecken der Beine" }
            };

            var grades = new List<PrintGrade>();
            if (saveGrades)
            {
                // Noten vergeben - mit Grade werden die Daten in der Datenbank gespeichert, mit Foreing Keys auf Exam
                // und Participant. Deshalb muss die Ausgabe in Excel, wo lesbare Daten und keine Indicies vorhanden
                // sein sollen, angepasst werden. Deshalb gibt es die Helper-Klasse PrintGrade.            
                grades = new List<PrintGrade>
            {
                // Jessica Alba
                new(participants[0], exams[0], "1", "Gute Bewegung aus den Beinen"),
                new(participants[0], exams[1], "1", "Beste Technik"),
                new(participants[0], exams[2], "1", "Beste Technik"),

                // Angelina Jolie
                new(participants[1], exams[0], "4,5-", "War nix... fast Themaverfehlung"),
                new(participants[1], exams[1], "5", "Sturz"),
                new(participants[1], exams[2], "3-", "Verbesserungsbedarf im Tempo"),

                // Brad Pitt
                new(participants[2], exams[0], "3", "Guter Rhythmus, aber etwas langsam"),
                new(participants[2], exams[1], "3-", "Braucht mehr Stockeinsatz"),
                new(participants[2], exams[2], "2-", "Sauberer Pflug, leicht wackelig"),

                // Michaela Schaffrath
                new(participants[3], exams[0], "2+", "Sehr flüssige Fahrweise"),
                new(participants[3], exams[1], "2", "Saubere Schwünge, gute Kontrolle"),
                new(participants[3], exams[2], "3", "Durchschnittliche Leistung, könnte stabiler sein"),

                // Megan Fox
                new(participants[4], exams[0], "3", "Gute Technik, aber zögerlich"),
                new(participants[4], exams[1], "3+", "Stabile Kurven, aber Tempo fehlt"),
                new(participants[4], exams[2], "2-", "Sehr präzise, gute Kontrolle"),

                // Johnny Cash
                new(participants[5], exams[0], "4", "Langsam, aber sichere Fahrt"),
                new(participants[5], exams[1], "4-", "Schwierigkeiten bei der Koordination"),
                new(participants[5], exams[2], "3+", "Solide, aber etwas unsicher"),

                // Markus Söder
                new(participants[6], exams[0], "3+", "Gute Bewegung, aber etwas steif"),
                new(participants[6], exams[1], "3-", "Braucht mehr Druck auf dem Außenski"),
                new(participants[6], exams[2], "2", "Korrekte Technik, gut im Pflug"),

                // Wolfgang Schäfer
                new(participants[7], exams[0], "4-", "Etwas schwerfällig, langsame Kurven"),
                new(participants[7], exams[1], "4", "Probleme mit der Stabilität"),
                new(participants[7], exams[2], "3", "Pflug in Ordnung, aber unruhig"),

                // Wilfried Gliem
                new(participants[8], exams[0], "4", "Zögerliche Kurven, Verbesserung nötig"),
                new(participants[8], exams[1], "4+", "Unregelmäßiger Stockeinsatz"),
                new(participants[8], exams[2], "3-", "Solider Pflug, könnte stabiler sein")
            };
            }


            // Speichern der Daten in einer Liste von Listen (jede Liste repräsentiert ein Tabellenblatt)
            var save_sheets = new List<List<IExportable>>
            {
                participants.ConvertAll(p => (IExportable)p),
                exams.ConvertAll(e => (IExportable)e),
                grades.ConvertAll(g => (IExportable)g)
            };

            // Namen der Tabellenblätter
            var sheetNames = new List<string> { "Teilnehmerliste", "Prüfungen", "Noten" };


            // Export der Daten in eine Excel-Datei
            IOExcel.ExportToExcel(save_sheets, sheetNames, filePath);
            Console.WriteLine("Daten erfolgreich exportiert nach " + filePath);

        }
    }
}
