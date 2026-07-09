using System;
using System.IO;
using System.Text.Json;

namespace WinKitty;

public class CatSaveData
{
    public string Name { get; set; } = "Chat";

    private static string SavePath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinKitty", "save.json");

    public static CatSaveData Load()
    {
        try
        {
            if (File.Exists(SavePath))
                return JsonSerializer.Deserialize<CatSaveData>(File.ReadAllText(SavePath)) ?? new();
        }
        catch { /* fichier corrompu, on repart avec une valeur par défaut */ }
        return new CatSaveData();
    }

    public void Save()
    {
        var dir = Path.GetDirectoryName(SavePath)!;
        Directory.CreateDirectory(dir);
        File.WriteAllText(SavePath, JsonSerializer.Serialize(this));
    }
}
