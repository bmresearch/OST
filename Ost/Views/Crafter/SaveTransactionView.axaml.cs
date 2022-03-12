using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ost.Views.Crafter
{
    public partial class SaveTransactionView : UserControl
    {
        public SaveTransactionView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
