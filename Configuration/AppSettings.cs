namespace WinKitty.Configuration;

public sealed class AppSettings
{
    public double StatIncreaseMultiplier { get; set; } = 1.0;
    public double StatDecreaseMultiplier { get; set; } = 1.0;
    public double SleepEnergyPerMinute { get; set; } = 2.0;
}