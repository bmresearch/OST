using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ost.Views.Crafter
{
    public partial class MultiSignatureAccountView : UserControl
    {
        public MultiSignatureAccountView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
