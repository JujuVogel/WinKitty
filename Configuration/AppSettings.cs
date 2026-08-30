using System;
using System.IO;
using System.Text.Json;
namespace WinKitty.Configuration;

public sealed class AppSettings
{
    private static string SettingsPath =>
    Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "WinKitty",
        "settings.json");

    public double StatIncreaseMultiplier { get; set; } = 1.0;
    public double StatDecreaseMultiplier { get; set; } = 1.0;

    public double SleepEnergyPerMinute { get; set; } = 2.0;

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                return JsonSerializer.Deserialize<AppSettings>(
                    File.ReadAllText(SettingsPath)) ?? new();
            }
        }
        catch
        {
            // Invalid settings file: fall back to defaults.
        }

        return new AppSettings();
    }
    public void Save()
{
    string directory = Path.GetDirectoryName(SettingsPath)!;
    Directory.CreateDirectory(directory);

    File.WriteAllText(
        SettingsPath,
        JsonSerializer.Serialize(this));
}
}