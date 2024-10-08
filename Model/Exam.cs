﻿using SQLite;
using System.Reflection;

namespace BergNoten.Model
{
    /// <summary>
    /// Stellt eine Name dar.
    /// </summary>
    public class Exam : TableData
    {
        #region Properties       

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Beschreibung { get; set; }
        #endregion

        /// <summary>
        /// Initialisiert eine neue Instanz der Exam-Klasse.
        /// </summary>
        public Exam()
        {
            Name = string.Empty;
            Beschreibung = string.Empty;
        }

        /// <summary>
        /// Überprüft, ob ein Prüfungs-Objekt Daten enthält.
        /// Eine Name gilt als nicht leer, wenn das Feld 'Name' einen Wert enthält.
        /// </summary>
        /// <param name="exam">Die zu prüfende Name.</param>
        /// <returns>True, wenn die Name Daten enthält, ansonsten False.</returns>
        public static bool IsNotEmpty(Exam exam)
        {
            return !string.IsNullOrWhiteSpace(exam.Name);
        }

        /// <summary>
        /// Überschreibt die Equals-Methode, um die Gleichheit von Prüfungs-Objekten zu überprüfen.
        /// Prüfungs-Objekte sind gleich, wenn ihre Eigenschaften 'Name' und 'Beschreibung' übereinstimmen.
        /// </summary>
        /// <param name="obj">Das zu vergleichende Objekt.</param>
        /// <returns>True, wenn die Objekte gleich sind, ansonsten False.</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(Exam))
            {
                return false;
            }
            else
            {
                var e = obj as Exam;
                return e?.Name == this.Name && e.Beschreibung == this.Beschreibung;
            }
        }

        /// <summary>
        /// Überschreibt die GetHashCode-Methode, um den Hashcode des Exam-Objekts zu berechnen.
        /// </summary>
        /// <returns>Der berechnete Hashcode.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Name, Beschreibung);
        }

        public override List<PropertyInfo> GetProperties()
        {
            var l = typeof(Exam).GetProperties().ToList();
            // Füge das letzt Element an erster Stelle ein.
            // Das letzte Element des Arrays ist die ID, da die ID in dem Konstruktor der Basisklasse,
            // nach dem Aufruf der vererbten Klasse, aufgerufen wird.            
            l.Insert(0, l[^1]);
            l.RemoveAt(l.Count - 1);

            return [.. l];
        }
    }
}
