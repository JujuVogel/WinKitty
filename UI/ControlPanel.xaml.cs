using System.Windows;
using WinKitty.Animation;
using WinKitty.Services;

namespace WinKitty.UI;

public partial class ControlPanel : Window
{
    private MainWindow _cat;
    private void RefreshStatsDisplay()
    {
        HungerBar.Value = _cat.Stats.Hunger;
        EnergyBar.Value = _cat.Stats.Energy;
        CleanlinessBar.Value = _cat.Stats.Cleanliness;
        HappinessBar.Value = _cat.Stats.Happiness;
        RefreshSleepDisplay();
    }
    private void RefreshSleepDisplay()
    {
        bool sleeping = _cat.Sleep.IsActive;
        bool busy = _cat.IsBusy;

        FeedButton.IsEnabled = !busy;
        SleepButton.IsEnabled = !busy;
        PlayButton.IsEnabled = !busy;
        CleanButton.IsEnabled = !busy;
        SleepMinutesBox.IsEnabled = !busy;

        PauseSleepButton.IsEnabled = sleeping;
        CancelSleepButton.IsEnabled = sleeping;

        PauseSleepButton.Content =
            _cat.Sleep.State == SleepState.Paused
                ? "Reprendre"
                : "Pause";

        SleepTimerText.Text = sleeping
            ? $"{FormatTime(_cat.Sleep.Elapsed)} / {FormatTime(_cat.Sleep.Duration)}"
            : "00:00 / 00:00";
    }

    private static string FormatTime(TimeSpan time)
    {
        return $"{(int)time.TotalMinutes:00}:{time.Seconds:00}";
    }

    public ControlPanel()
    {
        InitializeComponent();
        _cat = new MainWindow();
        IncreaseMultiplierBox.Text = _cat.Settings.StatIncreaseMultiplier.ToString();
        DecreaseMultiplierBox.Text = _cat.Settings.StatDecreaseMultiplier.ToString();
        SleepEnergyBox.Text = _cat.Settings.SleepEnergyPerMinute.ToString();
        _cat.Show();
        var refreshTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        refreshTimer.Tick += (s, e) => RefreshStatsDisplay();
        refreshTimer.Start();
        RefreshStatsDisplay();

        // save name
        CatNameBox.Text = _cat.SaveData.Name;
        CatNameBox.LostFocus += (s, e) =>
        {
            _cat.SaveData.Name = CatNameBox.Text;
            _cat.SaveData.Save();
        };
    }

    private void OnFeed(object s, RoutedEventArgs e) =>
        _cat.PlayTimedAction(Animations.Eating, TimeSpan.FromSeconds(3), () => _cat.Stats.Feed());

    private void OnSleep(object s, RoutedEventArgs e)
    {
        if (!double.TryParse(SleepMinutesBox.Text, out double minutes) || minutes <= 0)
        {
            MessageBox.Show("Entre un nombre de minutes valide.");
            return;
        }
        if (!_cat.PlaySleep(TimeSpan.FromMinutes(minutes)))
        {
            MessageBox.Show("Le chat est déjà occupé.");
        }
    }
    private void OnPauseSleep(object s, RoutedEventArgs e)
    {
        if (_cat.Sleep.State == SleepState.Running)
            _cat.PauseSleep();
        else if (_cat.Sleep.State == SleepState.Paused)
            _cat.ResumeSleep();

        RefreshSleepDisplay();
    }

    private void OnCancelSleep(object s, RoutedEventArgs e)
    {
        _cat.CancelSleep();
        RefreshSleepDisplay();
    }
    private void OnPlay(object s, RoutedEventArgs e) =>
        _cat.PlayTimedAction(Animations.Play, TimeSpan.FromSeconds(4), () => _cat.Stats.Play());
    private void OnClean(object s, RoutedEventArgs e) =>
_cat.PlayTimedAction(Animations.Cleaning, TimeSpan.FromSeconds(3), () => _cat.Stats.Clean());

    private void OnToggleDesktop(object s, RoutedEventArgs e) => _cat.ToggleDesktopOnly();

    protected override void OnClosed(EventArgs e)
    {
        _cat.Close();
        base.OnClosed(e);
    }
    private void OnApplyAdvancedSettings(object sender, RoutedEventArgs e)
    {
        if (!double.TryParse(IncreaseMultiplierBox.Text, out double increase))
            return;

        if (!double.TryParse(DecreaseMultiplierBox.Text, out double decrease))
            return;

        if (!double.TryParse(SleepEnergyBox.Text, out double sleepEnergy))
            return;
        _cat.Settings.StatIncreaseMultiplier = increase;
        _cat.Settings.StatDecreaseMultiplier = decrease;
        _cat.Settings.SleepEnergyPerMinute = sleepEnergy;

        _cat.Settings.Save();
    }
}
