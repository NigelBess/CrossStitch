using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace TestUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Enter)
            {
                var textbox = (TextBox)sender;
                var binding = textbox.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                    binding.UpdateSource();
                Keyboard.ClearFocus();
            }
        }

        private void MouseClickedOnBackground(object sender, MouseButtonEventArgs e)
        {
            Background.Focus();
        }
    }
}
