using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WinKitty;

public partial class MainWindow : Window
{
    private SpriteAnimator _animator;
    private CatBehavior _behavior;

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

        _animator = new SpriteAnimator(Idle);
        _animator.Play(Animations.Idle);

        _behavior = new CatBehavior(this, _animator);
        _behavior.Start();

        this.MouseLeftButtonDown += (s, e) =>
        {
            var pos = e.GetPosition(Idle);
            if (IsPixelOpaque(pos)) this.DragMove();
        };
    }

    // SUPPRIME le champ "private BitmapSource? _bitmap;" — plus utilisé, l'animator le remplace

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
