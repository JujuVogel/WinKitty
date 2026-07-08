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
    }

    private void OnFeed(object s, RoutedEventArgs e) { /* ex: _cat.SetState("eating"); */ }
    private void OnSleep(object s, RoutedEventArgs e) { /* ... */ }
    private void OnPlay(object s, RoutedEventArgs e) { /* ... */ }
    private void OnChangeSkin(object s, RoutedEventArgs e) { /* ... */ }

    protected override void OnClosed(EventArgs e)
    {
        _cat.Close();
        base.OnClosed(e);
    }
}
