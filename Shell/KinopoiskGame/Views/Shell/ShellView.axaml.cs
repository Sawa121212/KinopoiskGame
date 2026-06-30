using Avalonia;
using Avalonia.Controls;

namespace KinopoiskGame.Views.Shell
{
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }
    }
}