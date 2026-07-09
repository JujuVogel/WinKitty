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
    private AnimationClip? _currentActionClip;
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
        bool wasIdleBehavior = _currentActionClip == null;
        if (wasIdleBehavior) _behavior.Pause();

        _animator.Play(Animations.Grabbed);
        this.DragMove();

        if (wasIdleBehavior)
        {
            _animator.Play(Animations.Idle);
            _behavior.Resume();
        }
        else
        {
            _animator.Play(_currentActionClip!);
        }
    }
};
    }
    private bool _onDesktopOnly = false;

    public void ToggleDesktopOnly()
    {
        var helper = new System.Windows.Interop.WindowInteropHelper(this);

        if (!_onDesktopOnly)
        {
            IntPtr progman = NativeMethods.FindWindow("Progman", null);
            NativeMethods.SendMessageTimeout(progman, 0x052C, IntPtr.Zero, IntPtr.Zero, 0, 1000, out _);

            IntPtr workerW = IntPtr.Zero;
            NativeMethods.EnumWindows((hWnd, lParam) =>
            {
                IntPtr shellView = NativeMethods.FindWindowEx(hWnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shellView != IntPtr.Zero)
                {
                    workerW = NativeMethods.FindWindowEx(IntPtr.Zero, hWnd, "WorkerW", null);
                }
                return true;
            }, IntPtr.Zero);

            // fallback si la structure classique n'est pas trouvée : cherche un WorkerW direct sous le bureau
            if (workerW == IntPtr.Zero)
            {
                workerW = NativeMethods.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "WorkerW", null);
            }

            if (workerW != IntPtr.Zero)
            {
                this.Topmost = false;
                NativeMethods.SetParent(helper.Handle, workerW);
                _onDesktopOnly = true;
            }
            else
            {
                MessageBox.Show("Impossible de trouver le WorkerW — hack non supporté sur cette version de Windows.");
            }
        }
        else
        {
            NativeMethods.SetParent(helper.Handle, IntPtr.Zero);
            this.Topmost = true;
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
