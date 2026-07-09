using System.Windows;

namespace WinKitty;

public partial class ControlPanel : Window
{
    private MainWindow _cat;
    private void RefreshStatsDisplay()
    {
        HungerBar.Value = _cat.Stats.Hunger;
        EnergyBar.Value = _cat.Stats.Energy;
        CleanlinessBar.Value = _cat.Stats.Cleanliness;
        HappinessBar.Value = _cat.Stats.Happiness;
    }

    public ControlPanel()
    {
        InitializeComponent();
        _cat = new MainWindow();
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
        _cat.PlayTimedAction(Animations.Sleeping, TimeSpan.FromMinutes(minutes), () => _cat.Stats.Sleep());
    }
    private void OnPlay(object s, RoutedEventArgs e) =>
        _cat.PlayTimedAction(Animations.Play, TimeSpan.FromSeconds(4), () => _cat.Stats.Play());
    private void OnClean(object s, RoutedEventArgs e) =>
_cat.PlayTimedAction(Animations.Cleaning, TimeSpan.FromSeconds(3), () => _cat.Stats.Clean());
    protected override void OnClosed(EventArgs e)
    {
        _cat.Close();
        base.OnClosed(e);
    }
}
