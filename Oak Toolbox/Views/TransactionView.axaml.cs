using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OSTApp.Views
{
    public partial class TransactionView : UserControl
    {
        public TransactionView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
