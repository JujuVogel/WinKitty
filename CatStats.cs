using System;

namespace WinKitty;

public class CatStats
{
    public double Hunger { get; private set; } = 100;      // 100 = repu, 0 = affamé
    public double Energy { get; private set; } = 100;      // 100 = reposé, 0 = épuisé
    public double Cleanliness { get; private set; } = 100; // 100 = propre, 0 = sale
    public double Happiness { get; private set; } = 100;   // 100 = heureux, 0 = triste

    public void Feed() => Hunger = Math.Min(100, Hunger + 30);
    public void Sleep() => Energy = Math.Min(100, Energy + 40);
    public void Clean() => Cleanliness = Math.Min(100, Cleanliness + 35);
    public void Play() => Happiness = Math.Min(100, Happiness + 25);

    public void Decay()
    {
        Hunger = Math.Max(0, Hunger - 1);
        Energy = Math.Max(0, Energy - 0.5);
        Cleanliness = Math.Max(0, Cleanliness - 0.7);
        Happiness = Math.Max(0, Happiness - 0.8);
    }
}
