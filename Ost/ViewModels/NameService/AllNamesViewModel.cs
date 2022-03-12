
using Ost.Core.ViewModels;
using Ost.Services;
using ReactiveUI;
using Solnet.Programs.Clients;
using Solnet.Programs.Models.NameService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Solnet.Wallet;

namespace Ost.ViewModels.NameService
{
    public class AllNamesViewModel : ViewModelBase
    {
        private IRpcClientProvider _rpcProvider;

        private NameServiceClient Client => new NameServiceClient(_rpcProvider.Client);
        public string Header => "All";



        private IEnumerable<NameResultWrapper> _namesForAddress;
        public IEnumerable<NameResultWrapper> NamesForAddress
        {
            get => _namesForAddress;
            set => this.RaiseAndSetIfChanged(ref _namesForAddress, value);
        }

        public string AddressQuery { get; set; }

        private bool _loading;
        public bool Loading
        {
            get => _loading;
            set => this.RaiseAndSetIfChanged(ref _loading, value);
        }

        public AllNamesViewModel(IRpcClientProvider rpcProvider)
        {
            _rpcProvider = rpcProvider;
        }


        public async void QueryByAddress()
        {
            if(!PublicKey.IsValid(AddressQuery))
            {
                await ShowErrorMessage($"Please insert a valid address to query.");
                return;
            }
            
            Loading = true;

            NamesForAddress = (await Client.GetAllNamesByOwnerAsync(AddressQuery))
                .Where(x => x.Type == RecordType.ReverseRecord
                        || x.Type == RecordType.ReverseTwitterRecord
                        || x.Type == RecordType.ReverseTokenRecord).Select(x => new NameResultWrapper(x));

            Loading = false;
        }

    }
}
