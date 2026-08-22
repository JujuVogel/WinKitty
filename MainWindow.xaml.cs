using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Diagnostics;

namespace WinKitty;

public partial class MainWindow : Window
{
    private SpriteAnimator _animator;
    private CatBehavior _behavior;
    public CatSaveData SaveData { get; } = CatSaveData.Load();
    public AppSettings Settings { get; } = new();
    public CatStats Stats { get; } 
    private AnimationClip? _currentActionClip;
    public MainWindow()
    {
        InitializeComponent();
        Stats = new CatStats(Settings);
        // hide window
        this.SourceInitialized += (s, e) =>
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            int style = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, style | NativeMethods.WS_EX_TOOLWINDOW);
        };
        // launch animations
        _animator = new SpriteAnimator(Idle);
        _animator.Play(Animations.Idle);

        // launch behavior
        _behavior = new CatBehavior(this, _animator);
        _behavior.Start();

        // decay of stats
        var decayClock = Stopwatch.StartNew();

        var decayTimer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };

        decayTimer.Tick += (s, e) =>
        {
            TimeSpan elapsed = decayClock.Elapsed;
            decayClock.Restart();

            Stats.Decay(elapsed);
        };

        decayTimer.Start();

        // grabbing
        MouseLeftButtonDown += OnCatMouseLeftButtonDown;
    }
    private bool _onDesktopOnly = false;

    public void ToggleDesktopOnly()
{
    var helper = new WindowInteropHelper(this);

    if (!_onDesktopOnly)
    {
        IntPtr desktopOwner = DesktopManager.FindDesktopOwner();

        if (desktopOwner == IntPtr.Zero)
        {
            MessageBox.Show("Unable to find the Windows desktop.");
            return;
        }

        Topmost = false;
        helper.Owner = desktopOwner;

        _onDesktopOnly = true;
    }
    else
    {
        helper.Owner = IntPtr.Zero;
        Topmost = true;

        _onDesktopOnly = false;
    }
}
    public void PlaySleep(TimeSpan duration)
    {
        _behavior.Pause();
        _currentActionClip = Animations.Sleeping;
        _animator.Play(Animations.Sleeping);

        var gainPerSecond = 100.0 / duration.TotalSeconds; // remplit exactement à 100 en fin de durée
        var tick = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        tick.Tick += (s, e) => Stats.GainEnergy(gainPerSecond);
        tick.Start();

        var timer = new System.Windows.Threading.DispatcherTimer { Interval = duration };
        timer.Tick += (s, e) =>
        {
            timer.Stop();
            tick.Stop();
            _currentActionClip = null;
            _animator.Play(Animations.Idle);
            _behavior.Resume();
        };
        timer.Start();
    }

    // handles timer for sleeping
    public void PlayTimedAction(AnimationClip clip, TimeSpan duration, Action onComplete)
    {
        _behavior.Pause();
        _currentActionClip = clip;
        _animator.Play(clip);

        var timer = new System.Windows.Threading.DispatcherTimer { Interval = duration };
        timer.Tick += (s, e) =>
        {
            timer.Stop();
            onComplete();
            _currentActionClip = null;
            _animator.Play(Animations.Idle);
            _behavior.Resume();
        };
        timer.Start();
    }

    // prevents user to grab the transparent part of the frame
    private bool IsPixelOpaque(Point p)
    {
        var bmp = _animator.CurrentBitmap;
        if (bmp == null) return false;
        int x = (int)(p.X * bmp.PixelWidth / Idle.ActualWidth);
        int y = (int)(p.Y * bmp.PixelHeight / Idle.ActualHeight);
        if (x < 0 || y < 0 || x >= bmp.PixelWidth || y >= bmp.PixelHeight) return false;

        byte[] pixel = new byte[4];
        bmp.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);
        return pixel[3] > 10;
    }

    // handles grabbing
    private void OnCatMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
{
    Point position = e.GetPosition(Idle);

    if (!IsPixelOpaque(position))
        return;

    bool wasIdle = _currentActionClip is null;

    if (wasIdle)
        _behavior.Pause();

    try
    {
        _animator.Play(Animations.Grabbed);
        DragMove();
    }
    finally
    {
        if (wasIdle)
        {
            _animator.Play(Animations.Idle);
            _behavior.Resume();
        }
        else if (_currentActionClip is not null)
        {
            _animator.Play(_currentActionClip);
        }
    }
}
}
