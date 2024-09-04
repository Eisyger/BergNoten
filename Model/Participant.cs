using SQLite;
using System.Reflection;

namespace BergNoten.Model
{
    /// <summary>
    /// Stellt einen Teilnehmer dar.
    /// </summary>
    public class Participant : TableData
    {
        #region Properties 
        [MaxLength(30),]
        public string Vorname { get; set; }
        [MaxLength(30)]
        public string Nachname { get; set; }
        [MaxLength(10)]
        public string Geburtsdatum { get; set; }
        [MaxLength(50)]
        public string Verein { get; set; }
        #endregion

        /// <summary>
        /// Erstellt eine neue Instanz der Teilnehmer-Klasse.
        /// </summary>
        public Participant()
        {
            Vorname = String.Empty;
            Nachname = String.Empty;
            Geburtsdatum = String.Empty;
            Verein = String.Empty;
        }
        /// <summary>
        /// Erstellt eine neue Instanz der Teilnehmer-Klasse.
        /// </summary>
        public Participant(string vorname, string nachname, string bday, string verein)
        {
            Vorname = vorname;
            Nachname = nachname;
            Geburtsdatum = bday;
            Verein = verein;
        }

        /// <summary>
        /// Überprüft, ob ein Teilnehmer-Objekt Daten enthält. 
        /// Ein Teilnehmer hat nur dann Daten wenn er einen Vor- oder Nachnamen hat.
        /// </summary>
        /// <param name="participant">Der zu überprüfende Teilnehmer.</param>
        /// <returns>True, wenn der Teilnehmer Daten enthält, ansonsten False.</returns>
        public static bool IsNotEmpty(Participant participant)
        {
            return !string.IsNullOrWhiteSpace(participant.Vorname) ||
                   !string.IsNullOrWhiteSpace(participant.Nachname);
        }

        /// <summary>
        /// Überschreibt die Equals-Methode, um die Gleichheit von Teilnehmer-Objekten zu überprüfen.
        /// Es werden nur nach Vornamen, Nachnamen und Geburtsdatum unterschieden. Sind diese Eigenschaften gleich,
        /// so sind auch die Objekte gleich.
        /// </summary>
        /// <param name="obj">Das zu vergleichende Objekt.</param>
        /// <returns>True, wenn die Objekte gleich sind, ansonsten False.</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || obj is not Participant)
            {
                return false;
            }
            else
            {
                var p = obj as Participant;
                return p?.Vorname == this.Vorname && p.Nachname == this.Nachname && p.Geburtsdatum == this.Geburtsdatum;
            }
        }

        /// <summary>
        /// Überschreibt die GetHashCode-Methode, um den Hashcode des Participant-Objekts zu berechnen.
        /// </summary>
        /// <returns>Der berechnete Hashcode.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Vorname, Nachname, Geburtsdatum, Verein);
        }

        public override List<PropertyInfo> GetProperties()
        {
            var l = typeof(Participant).GetProperties().ToList();
            // Füge das letzt Element an erster Stelle ein.
            // Das letzte Element des Arrays ist die ID, da die ID in dem Konstruktor der Basisklasse,
            // nach dem Aufruf der vererbten Klasse, aufgerufen wird.            
            l.Insert(0, l[^1]);
            l.RemoveAt(l.Count - 1);

            return [.. l];
        }
    }
}