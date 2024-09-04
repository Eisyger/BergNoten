using Newtonsoft.Json;

namespace AusbilderAppViews.Model
{
    /// <summary>
    /// Verwaltet die Konfigurationseinstellungen der Anwendung.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Der Pfad zur Datenbank-Datei.
        /// </summary>
        public string PathToDatabase { get; set; } = string.Empty;

        /// <summary>
        /// Der Pfad zum Konfigurationsverzeichnis.
        /// </summary>
        public string PathToConfig { get; set; } = string.Empty;

        /// <summary>
        /// Der Benutzername.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Speichert oder gibt den Pfad zu der letzten bekannten .xls Datenbank zurück.
        /// </summary>
        public string PathToLastestDatabase { get; set; } = string.Empty;

        /// <summary>
        /// Erstellt eine neue Konfigurationsinstanz.
        /// Lädt die Konfiguration aus einer JSON-Datei, wenn vorhanden, oder erstellt eine neue Konfigurationsdatei, wenn keine vorhanden ist.
        /// </summary>
        /// <param name="pathToConfig">Der Pfad zum Konfigurationsverzeichnis.</param>
        public Config(string pathToConfig)
        {
            PathToConfig = pathToConfig;
            LoadSettingsFromJson(pathToConfig);
        }

        /// <summary>
        /// Speichert die aktuellen Konfigurationseinstellungen in einer JSON-Datei.
        /// </summary>
        public void SaveSettingsToJson()
        {
            // Erstellt ein anonymes Objekt mit den aktuellen Konfigurationseinstellungen
            var settings = new
            {
                PathToDatabase = PathToDatabase,
                PathToConfig = PathToConfig,
                Username = Username,
                PathToLastestDatabase = PathToLastestDatabase
            };

            // Serialisiert das Einstellungen-Objekt zu JSON und speichert es in einer Datei
            string json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(Path.Combine(PathToConfig, "config.json"), json);
        }

        /// <summary>
        /// Lädt die Konfigurationseinstellungen aus einer JSON-Datei.
        /// </summary>
        /// <param name="pathToConfig">Der Pfad zum Konfigurationsverzeichnis.</param>
        private void LoadSettingsFromJson(string pathToConfig)
        {
            string path = Path.Combine(PathToConfig, "config.json");

            // Überprüft, ob die Konfigurationsdatei existiert
            if (!File.Exists(path))
            { return; }

            try
            {
                // Liest den Inhalt der Konfigurationsdatei
                string json = File.ReadAllText(path);

                // Deserialisiert die JSON-Daten in ein anonymes Objekt
                var settings = JsonConvert.DeserializeAnonymousType(json, new
                {
                    PathToDatabase = "",
                    PathToConfig = "",
                    Username = "",
                    PathToLastestDatabase = ""
                }
                    );

                // Weist die Werte aus der Konfigurationsdatei den entsprechenden Eigenschaften zu
                PathToDatabase = settings.PathToDatabase;
                Username = settings.Username;
                PathToDatabase = settings.PathToDatabase;
                PathToConfig = settings.PathToConfig;
                PathToLastestDatabase = settings.PathToLastestDatabase;
            }
            catch (Exception ex)
            {
                // Gibt eine Fehlermeldung aus, falls beim Laden der Konfiguration ein Fehler auftritt
                Console.WriteLine(ex.Message);
            }
        }
    }
}