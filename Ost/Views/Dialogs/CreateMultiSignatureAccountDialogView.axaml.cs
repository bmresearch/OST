using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ost.Views.Dialogs
{
    public partial class CreateMultiSignatureAccountDialogView : UserControl
    {
        public CreateMultiSignatureAccountDialogView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
