namespace WinKitty;

public record AnimationClip(string Path, int FrameWidth, int FrameHeight, int FrameCount, int Fps, bool Loop);

public static class Animations
{
    public static readonly AnimationClip Idle = new("assets/cat_animations/Idle.png", 64, 32, 4, 6, true);
    public static readonly AnimationClip Idle2 = new("assets/cat_animations/Idle_2.png", 64, 32, 4, 6, true);
    public static readonly AnimationClip Running = new("assets/cat_animations/Running.png", 64, 32, 6, 10, true);
    public static readonly AnimationClip Jumping = new("assets/cat_animations/Jumping.png", 64, 32, 6, 10, false);
    public static readonly AnimationClip Sleeping = new("assets/cat_animations/Sleeping.png", 64, 32, 4, 4, true);
    public static readonly AnimationClip Stretching = new("assets/cat_animations/Stretching.png", 64, 32, 4, 6, false);
    public static readonly AnimationClip Cleaning = new("assets/cat_animations/Cleaning.png", 64, 32, 4, 6, true);
    public static readonly AnimationClip Eating = new("assets/cat_animations/Eating.png", 64, 32, 4, 6, true);
    public static readonly AnimationClip Play = new("assets/cat_animations/Play.png", 64, 32, 4, 8, true);
    public static readonly AnimationClip Attack = new("assets/cat_animations/Attack.png", 64, 32, 4, 10, false);
}
