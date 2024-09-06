using Newtonsoft.Json;

namespace BergNoten.Helper;

/// <summary>
/// Verwaltet die Konfigurationseinstellungen der Anwendung.
/// </summary>
public class Config
{
    private readonly object _fileLock = new();

    private string _pathToData;
    private string _fileName;
    private string _pathToConfig;
    private string _username;

    /// <summary>
    /// Der Pfad zur Datei.
    /// </summary>
    public string PathToData
    {
        get => _pathToData;
        set
        {
            _pathToData = value;
            Task.Run(() => UpdateSettings());
        }
    }

    /// <summary>
    /// Der Name der Datei.
    /// </summary>
    public string FileName
    {
        get => _fileName;
        set
        {
            _fileName = value;
            Task.Run(() => UpdateSettings());
        }
    }

    /// <summary>
    /// Der Pfad zum Konfigurationsverzeichnis.
    /// </summary>
    public string PathToConfig
    {
        get => _pathToConfig;
        set
        {
            _pathToConfig = value;
            Task.Run(() => UpdateSettings());
        }
    }

    /// <summary>
    /// Der Benutzername.
    /// </summary>
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            Task.Run(() => UpdateSettings());
        }
    }



    /// <summary>
    /// Erstellt eine neue Konfigurationsinstanz.
    /// Lädt die Konfiguration aus einer JSON-Datei, wenn vorhanden, oder erstellt eine neue Konfigurationsdatei, wenn keine vorhanden ist.
    /// Werden keine Daten aus der Konfig geladen ist der Standardwert der string.Empty.
    /// </summary>
    /// <param name="pathToConfig">Der Pfad zum Konfigurationsverzeichnis.</param>
    public Config(string pathToConfig)
    {
        _pathToData = string.Empty;
        _fileName = string.Empty;
        _pathToConfig = string.Empty;
        _username = string.Empty;

        PathToConfig = pathToConfig;
        Task.Run(() => LoadSettingsFromJson());
    }

    /// <summary>
    /// Speichert die aktuellen Konfigurationseinstellungen in einer JSON-Datei.
    /// </summary>
    public void UpdateSettings()
    {
        // Erstellt ein anonymes Objekt mit den aktuellen Konfigurationseinstellungen
        var settings = new
        {
            PathToData,
            PathToConfig,
            Username,
            FileName
        };

        lock (_fileLock)
        {
            // Serialisiert das Einstellungen-Objekt zu JSON und speichert es in einer Datei
            string json = JsonConvert.SerializeObject(settings);
            File.WriteAllTextAsync(Path.Combine(PathToConfig, "config.json"), json);
        }
    }

    /// <summary>
    /// Lädt die Konfigurationseinstellungen aus einer JSON-Datei.
    /// </summary>
    private void LoadSettingsFromJson()
    {
        string path = Path.Combine(PathToConfig, "config.json");

        // Überprüft, ob die Konfigurationsdatei existiert
        if (!File.Exists(path))
        { return; }

        try
        {
            string json;

            lock (_fileLock)
            {
                // Liest den Inhalt der Konfigurationsdatei
                json = File.ReadAllText(path);
            }
            // Deserialisiert die JSON-Daten in ein anonymes Objekt
            var settings = JsonConvert.DeserializeAnonymousType(json, new
            {
                PathToData = "",
                PathToConfig = "",
                Username = "",
                FileName = ""
            }
                );

            // Weist die Werte aus der Konfigurationsdatei den entsprechenden Eigenschaften zu
            PathToData = settings?.PathToData ?? string.Empty;
            PathToData = settings?.PathToData ?? string.Empty;
            Username = settings?.Username ?? string.Empty;
            FileName = settings?.FileName ?? string.Empty;

        }
        catch (Exception ex)
        {
            // Gibt eine Fehlermeldung aus, falls beim Laden der Konfiguration ein Fehler auftritt
            Console.WriteLine(ex.Message);
        }
    }
}