
using Ost.Services;
using Ost.Services.Network;
using Ost.Services.Wallets;
using System;
using ReactiveUI;
using Ost.Services.Network.Events;
using Ost.Core.ViewModels;
using Ost.ViewModels.Crafter;
using Ost.ViewModels.Wallet;
using Ost.Models;
using Ost.Services.Store;
using Ost.ViewModels.MultiSignatures;
using Solnet.Programs.Clients;
using Solnet.Programs.Models.NameService;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Ost.ViewModels.NameService
{
    public class NameServiceViewModel : ViewModelBase
    {
        private IRpcClientProvider _rpcProvider;

        public string Header => "Name Service";

        public List<ViewModelBase> Tabs { get; set; }




        public NameServiceViewModel(IRpcClientProvider rpcProvider)
        {
            _rpcProvider = rpcProvider;

            Tabs = new List<ViewModelBase>()
            {
                new SolNamingViewModel(rpcProvider),
                new TokenNamingViewModel(rpcProvider),
                new TwitterNamingViewModel(rpcProvider),
                new AllNamesViewModel(rpcProvider)
            };
        }

    }
}
