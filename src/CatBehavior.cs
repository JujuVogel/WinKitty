using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace WinKitty;

public class CatBehavior
{
    private readonly Window _window;
    private readonly SpriteAnimator _animator;
    private readonly DispatcherTimer _tick = new();
    private readonly Random _rng = new();

    private double _targetX;
    private bool _walking;
    private DateTime _stateUntil;
    private ScaleTransform _flip = new(1, 1);

    public CatBehavior(Window window, SpriteAnimator animator)
    {
        _window = window;
        _animator = animator;
        _window.RenderTransform = _flip;
        _window.RenderTransformOrigin = new Point(0.5, 0.5);
    }

    public void Start()
    {
        PickNextState();
        _tick.Interval = TimeSpan.FromMilliseconds(16);
        _tick.Tick += OnTick;
        _tick.Start();
    }

    private void PickNextState()
    {
        double roll = _rng.NextDouble();
        var screenWidth = SystemParameters.PrimaryScreenWidth;

        if (roll < 0.4)
        {
            // walk to a random point
            _walking = true;
            _targetX = _rng.NextDouble() * (screenWidth - _window.Width);
            _flip.ScaleX = _targetX < _window.Left ? -1 : 1;
            _animator.Play(Animations.Running);
            _stateUntil = DateTime.Now.AddSeconds(999); // fin gérée par arrivée, pas timer
        }
        else if (roll < 0.5)
        {
            _walking = false;
            _animator.Play(Animations.Jumping);
            _stateUntil = DateTime.Now.AddSeconds(1);
        }
        else if (roll < 0.65)
        {
            _walking = false;
            _animator.Play(Animations.Sleeping);
            _stateUntil = DateTime.Now.AddSeconds(_rng.Next(4, 10));
        }
        else if (roll < 0.75)
        {
            _walking = false;
            _animator.Play(Animations.Stretching);
            _stateUntil = DateTime.Now.AddSeconds(2);
        }
        else if (roll < 0.85)
        {
            _walking = false;
            _animator.Play(Animations.Cleaning);
            _stateUntil = DateTime.Now.AddSeconds(_rng.Next(2, 5));
        }
        else
        {
            _walking = false;
            _animator.Play(_rng.Next(2) == 0 ? Animations.Idle : Animations.Idle2);
            _stateUntil = DateTime.Now.AddSeconds(_rng.Next(2, 6));
        }
    }

    private void OnTick(object? s, EventArgs e)
    {
        if (_walking)
        {
            double speed = 2.0; // px par tick, ajuste la vitesse ici
            double dx = _targetX - _window.Left;
            if (Math.Abs(dx) < speed)
            {
                _window.Left = _targetX;
                PickNextState();
            }
            else
            {
                _window.Left += Math.Sign(dx) * speed;
            }
        }
        else if (DateTime.Now >= _stateUntil)
        {
            PickNextState();
        }
    }
}
