using System;
using System.Collections.Generic;
using System.Text;

namespace OSTApp.ViewModels
{
    public class TransactionViewModel : ViewModelBase
    {
        public string Header => "Transaction Crafting";


        public List<ViewModelBase> Tabs { get; set; }


        public TransactionViewModel()
        {
            Tabs = new List<ViewModelBase>()
            {
                new TransactionCraftViewModel(),
                new TransactionSendViewModel(),
                new TransactionSignViewModel()
            };
        }
    }
}
