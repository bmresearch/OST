using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ost.Views.NameService
{
    public partial class TokenNamingView : UserControl
    {
        public TokenNamingView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
