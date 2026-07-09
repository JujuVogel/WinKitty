namespace WinKitty;

public record AnimationClip(string Path, int FrameWidth, int FrameHeight, int FrameCount, int Fps, bool Loop);

public static class Animations
{
    public static readonly AnimationClip Idle = new("assets/cat_animations/Idle.png", 32, 32, 4, 1000 / 150, true);
    public static readonly AnimationClip Idle2 = new("assets/cat_animations/Idle_2.png", 32, 32, 4, 1000 / 150, true);
    public static readonly AnimationClip Running = new("assets/cat_animations/Running.png", 32, 32, 8, 1000 / 80, true);
    public static readonly AnimationClip Sleeping = new("assets/cat_animations/Sleeping.png", 32, 32, 4, 1000 / 200, true);
    public static readonly AnimationClip Jumping = new("assets/cat_animations/Jumping.png", 32, 32, 8, 1000 / 90, false);
    public static readonly AnimationClip Stretching = new("assets/cat_animations/Stretching.png", 32, 32, 8, 1000 / 120, false);
    public static readonly AnimationClip Eating = new("assets/cat_animations/Eating.png", 32, 32, 4, 1000 / 130, true);
    public static readonly AnimationClip Cleaning = new("assets/cat_animations/Cleaning.png", 32, 32, 4, 1000 / 110, true);
    public static readonly AnimationClip Play = new("assets/cat_animations/Play.png", 32, 32, 6, 1000 / 100, true);
    public static readonly AnimationClip Attack = new("assets/cat_animations/Attack.png", 32, 32, 7, 1000 / 85, false);
    public static readonly AnimationClip Grabbed = new("assets/cat_animations/Stretching.png", 32, 32, 8, 1000 / 150, true);
}
