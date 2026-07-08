using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinKitty;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // loads sprite from spritesheet
        var sheet = new BitmapImage(new Uri("assets/cat_animations/Idle.png", UriKind.Relative));
        int frameWidth = 64;
        int frameIndex = 0;
        _bitmap = new CroppedBitmap(sheet, new Int32Rect(frameIndex * frameWidth, 0, frameWidth, 32));
        Idle.Source = _bitmap;

        // allows drag if pixel isn't transparent
        this.MouseLeftButtonDown += (s, e) =>
        {
            var pos = e.GetPosition(Idle);
            if (IsPixelOpaque(pos)) this.DragMove();
        };
    }
    private BitmapSource? _bitmap;

    // checks if the pixel is transparent or not
    private bool IsPixelOpaque(Point p)
    {
        if (_bitmap == null) return false;
        int x = (int)(p.X * _bitmap.PixelWidth / Idle.ActualWidth);
        int y = (int)(p.Y * _bitmap.PixelHeight / Idle.ActualHeight);
        if (x < 0 || y < 0 || x >= _bitmap.PixelWidth || y >= _bitmap.PixelHeight) return false;

        byte[] pixel = new byte[4];
        _bitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);
        return pixel[3] > 10; // alpha canal
    }
}
