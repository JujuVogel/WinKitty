using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WinKitty;

public class SpriteAnimator
{
    private readonly Image _target;
    private readonly DispatcherTimer _timer = new();
    private AnimationClip? _clip;
    private BitmapSource? _sheet;
    private int _frameIndex;

    public bool Finished { get; private set; }

    public SpriteAnimator(Image target) => _target = target;

    public void Play(AnimationClip clip)
    {
        _clip = clip;
        _frameIndex = 0;
        Finished = false;
        _sheet = new FormatConvertedBitmap(
            new BitmapImage(new Uri(clip.Path, UriKind.Relative)),
            PixelFormats.Bgra32, null, 0);
            int requiredWidth = clip.FrameWidth * (clip.StartFrame + clip.FrameCount);

        // checks if the format of the png is a valid one
        if (_sheet.PixelWidth < requiredWidth ||
            _sheet.PixelHeight < clip.FrameHeight)
        {
            throw new InvalidOperationException(
                $"Invalid sprite sheet '{clip.Path}': " +
                $"expected at least {requiredWidth}x{clip.FrameHeight}, " +
                $"got {_sheet.PixelWidth}x{_sheet.PixelHeight}.");
        }

        _timer.Stop();
        _timer.Interval = TimeSpan.FromSeconds(1.0 / clip.FrameDurationMs);
        _timer.Tick -= OnTick;
        _timer.Tick += OnTick;
        _timer.Start();
        DrawFrame();
    }

    // clock for animation
    private void OnTick(object? s, EventArgs e)
    {
        _frameIndex++;
        if (_frameIndex >= _clip!.FrameCount)
        {
            if (_clip.Loop) _frameIndex = 0;
            else { _frameIndex = _clip.FrameCount - 1; Finished = true; _timer.Stop(); }
        }
        DrawFrame();
    }

    private void DrawFrame()
    {
        int sourceFrameIndex = _clip!.StartFrame + _frameIndex;
        var rect = new Int32Rect(
            sourceFrameIndex * _clip.FrameWidth,
            0,
            _clip.FrameWidth,
            _clip.FrameHeight);
        var cropped = new CroppedBitmap(_sheet, rect);
        _target.Source = cropped;
        CurrentBitmap = cropped;
    }

    public BitmapSource? CurrentBitmap { get; private set; }
}
