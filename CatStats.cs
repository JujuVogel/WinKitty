using System;

namespace WinKitty;

public class CatStats
{
    private readonly AppSettings _settings;

    public CatStats(AppSettings settings)
    {
        _settings = settings;
    }

    public double Hunger { get; private set; } = 100;      // 100 = repu, 0 = affamé
    public double Energy { get; private set; } = 100;      // 100 = reposé, 0 = épuisé
    public double Cleanliness { get; private set; } = 100; // 100 = propre, 0 = sale
    public double Happiness { get; private set; } = 100;   // 100 = heureux, 0 = triste

    public void Feed() =>
    Hunger = Math.Min(
        100,
        Hunger + 30 * _settings.StatIncreaseMultiplier);
    public void Clean() => 
    Cleanliness = Math.Min(100, Cleanliness + 35 * _settings.StatIncreaseMultiplier);
    public void Play() => 
    Happiness = Math.Min(100, Happiness + 25 * _settings.StatIncreaseMultiplier);
    
    public void GainEnergy(double amount) =>
        Energy = Math.Min(100, Energy + amount);
    public void Decay(TimeSpan elapsed)
{
    double minutes =
        elapsed.TotalMinutes * _settings.StatDecreaseMultiplier;

    Hunger = Math.Max(0, Hunger - 12.0 * minutes);
    Energy = Math.Max(0, Energy - 6.0 * minutes);
    Cleanliness = Math.Max(0, Cleanliness - 8.4 * minutes);
    Happiness = Math.Max(0, Happiness - 9.6 * minutes);
}
}
