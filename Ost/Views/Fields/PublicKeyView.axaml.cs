using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ost.Views.Fields
{
    public partial class PublicKeyView : UserControl
    {
        public PublicKeyView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
