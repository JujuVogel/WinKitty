using System.Windows;

namespace WinKitty;

public partial class ControlPanel : Window
{
    private MainWindow _cat;

    public ControlPanel()
    {
        InitializeComponent();
        _cat = new MainWindow();
        _cat.Show();
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

    private void OnSleep(object s, RoutedEventArgs e) =>
        _cat.PlayTimedAction(Animations.Sleeping, TimeSpan.FromSeconds(6), () => _cat.Stats.Sleep());

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
