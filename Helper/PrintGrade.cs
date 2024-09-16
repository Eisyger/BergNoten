using BergNoten.Interfaces;
using BergNoten.Model;
using System.Collections.Generic;
using System.Reflection;


namespace BergNoten.Helper
{
    public class PrintGrade : IExportable
    {
        public int Teilnehmer_ID { get; set; }
        public string Nachname { get; set; }
        public string Vorname { get; set; }

        public int Prüfungs_ID { get; set; }
        public string Prüfung { get; set; }

        public double Note { get; set; }


        public PrintGrade(Participant participant, Exam exam, string grade = "-", string bemerkung = "")
        {
            Teilnehmer_ID = participant.ID;
            Nachname = participant.Nachname;
            Vorname = participant.Vorname;
            Prüfung = exam.Name;
            Prüfungs_ID = exam.ID;
            // Entferne das Minus bei der Benotung, da es bei der Notenberechnung nicht relevant ist. Zum diskutieren kann man die 
            // Tendenz in der App, bzw. Datenbank ansehen.
            Note = double.Parse(grade.Split('-')[0]);
        }

        public List<PropertyInfo> GetProperties()
        {
            return [.. typeof(PrintGrade).GetProperties()];
        }
    }
}
