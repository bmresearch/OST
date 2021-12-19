using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OSTApp.Views
{
    public partial class TransactionSendView : UserControl
    {
        public TransactionSendView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
