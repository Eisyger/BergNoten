namespace BergNoten.Helper;

public class DatabaseGarbageCollection
{
    /// <summary>
    /// Entfernt alle automatisch erzeugten Dateien mit dem Dateikürzel ".db" welche älter als daysBack sind.
    /// </summary>
    /// <param name="daysBack"></param>
    public static void Clean(int daysBack)
    {
        var files = Directory.GetFiles(FileSystem.Current.AppDataDirectory, "*.db");
        foreach (var file in files)
        {
            var date = File.GetCreationTime(file);
            if (date.Day + daysBack < DateTime.Today.Day)
            {
                File.Delete(file);
            }

        }
    }
}