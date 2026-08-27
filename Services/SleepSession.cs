using System.Diagnostics;

namespace WinKitty.Services;

public enum SleepState
{
    Inactive,
    Running,
    Paused
}

public sealed class SleepSession
{
    private readonly Stopwatch _clock = new();
    private TimeSpan _accumulated = TimeSpan.Zero;

    public SleepState State { get; private set; } = SleepState.Inactive;
    public TimeSpan Duration { get; private set; }

    public bool IsActive => State != SleepState.Inactive;

    public TimeSpan Elapsed
    {
        get
        {
            TimeSpan elapsed = _accumulated;

            if (State == SleepState.Running)
                elapsed += _clock.Elapsed;

            return elapsed < Duration ? elapsed : Duration;
        }
    }

    public TimeSpan Remaining =>
        Duration > Elapsed ? Duration - Elapsed : TimeSpan.Zero;

    public bool IsComplete =>
        IsActive && Elapsed >= Duration;

    public void Start(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(duration));

        if (IsActive)
            throw new InvalidOperationException("A sleep session is already active.");

        Duration = duration;
        _accumulated = TimeSpan.Zero;
        _clock.Restart();

        State = SleepState.Running;
    }

    public void Pause()
    {
        if (State != SleepState.Running)
            return;

        _accumulated += _clock.Elapsed;
        _clock.Reset();

        State = SleepState.Paused;
    }

    public void Resume()
    {
        if (State != SleepState.Paused)
            return;

        _clock.Restart();
        State = SleepState.Running;
    }

    public void Reset()
    {
        _clock.Reset();
        _accumulated = TimeSpan.Zero;
        Duration = TimeSpan.Zero;
        State = SleepState.Inactive;
    }
}