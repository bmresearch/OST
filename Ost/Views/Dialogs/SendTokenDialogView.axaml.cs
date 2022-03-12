using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ost.Views.Dialogs
{
    public partial class SendTokenDialogView : UserControl
    {
        public SendTokenDialogView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
