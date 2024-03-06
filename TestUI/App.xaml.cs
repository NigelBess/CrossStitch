using System.Windows;

namespace TestUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void Start(object sender, StartupEventArgs e)
        {
            var main = new Main();
            main.Start();
        }
    }
}
