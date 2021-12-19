using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OSTApp.Views
{
    public partial class TransactionSignView : UserControl
    {
        public TransactionSignView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
