using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Material.Dialog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ost.Core.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {

        protected async Task ShowErrorMessage(string error)
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
                {
                    ContentHeader = "Error",
                    SupportingText = error,
                    DialogIcon = Material.Dialog.Icons.DialogIconKind.Error,
                    StartupLocation = WindowStartupLocation.CenterOwner,
                    Width = 500,
                    Borderless = true,

                });
                
                await dialog.ShowDialog(desktop.MainWindow);
            }
        }

    }
}
