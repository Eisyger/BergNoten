using Newtonsoft.Json;

namespace BergNoten.Helper;

/// <summary>
/// Verwaltet die Konfigurationseinstellungen der Anwendung.
/// </summary>
public class Config
{
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
            UpdateSettings();
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
            UpdateSettings();
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
            UpdateSettings();
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
            UpdateSettings();
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

        _pathToConfig = pathToConfig;
        LoadSettingsFromJson();
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
        // Serialisiert das Einstellungen-Objekt zu JSON und speichert es in einer Datei
        string json = JsonConvert.SerializeObject(settings);
        File.WriteAllText(PathToConfig, json);
    }

    /// <summary>
    /// Lädt die Konfigurationseinstellungen aus einer JSON-Datei.
    /// </summary>
    private void LoadSettingsFromJson()
    {
        // Überprüft, ob die Konfigurationsdatei existiert
        if (!File.Exists(PathToConfig))
        { return; }

        try
        {
            // Liest den Inhalt der Konfigurationsdatei
            var json = File.ReadAllText(PathToConfig);

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
            _pathToData = settings?.PathToData ?? string.Empty;
            _pathToConfig = settings?.PathToConfig ?? string.Empty;
            _username = settings?.Username ?? string.Empty;
            _fileName = settings?.FileName ?? string.Empty;
        }
        catch (Exception ex)
        {
            // Gibt eine Fehlermeldung aus, falls beim Laden der Konfiguration ein Fehler auftritt
            Console.WriteLine(ex.Message);
        }
    }
}