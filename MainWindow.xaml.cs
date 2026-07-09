using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WinKitty;

public partial class MainWindow : Window
{
    private SpriteAnimator _animator;
    private CatBehavior _behavior;
    public CatSaveData SaveData { get; } = CatSaveData.Load();
    public CatStats Stats { get; } = new();

    public MainWindow()
    {
        InitializeComponent();

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
        var decayTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        decayTimer.Tick += (s, e) => Stats.Decay();
        decayTimer.Start();

        // grabbing logic
        this.MouseLeftButtonDown += (s, e) =>
        {
            var pos = e.GetPosition(Idle);
            if (IsPixelOpaque(pos))
            {
                _behavior.Pause();
                _animator.Play(Animations.Grabbed);
                this.DragMove();
                _animator.Play(Animations.Idle);
                _behavior.Resume();
            }
        };
    }

    public void PlayTimedAction(AnimationClip clip, TimeSpan duration, Action onComplete)
    {
        _behavior.Pause();
        _animator.Play(clip);

        var timer = new System.Windows.Threading.DispatcherTimer { Interval = duration };
        timer.Tick += (s, e) =>
        {
            timer.Stop();
            onComplete();
            _animator.Play(Animations.Idle);
            _behavior.Resume();
        };
        timer.Start();
    }

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
}
