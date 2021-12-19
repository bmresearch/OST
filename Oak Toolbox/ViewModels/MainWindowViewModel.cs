using System;
using System.Collections.Generic;
using System.Text;

namespace OSTApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public List<ViewModelBase> Tabs { get; set; }


        public MainWindowViewModel()
        {
            Tabs = new List<ViewModelBase>()
            {
                new TransactionViewModel()
            };
        }
    }
}
