
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
    public class SolNamingViewModel : ViewModelBase
    {
        private IRpcClientProvider _rpcProvider;

        private NameServiceClient Client => new NameServiceClient(_rpcProvider.Client);
        public string Header => "Sol Names";


        public string SolDomainQuery { get; set; }

        private NameRecord _solNameRecord;
        public NameRecord SolNameRecord
        {
            get => _solNameRecord;
            set => this.RaiseAndSetIfChanged(ref _solNameRecord, value);
        }

        private IEnumerable<ReverseNameRecord> _namesForAddress;
        public IEnumerable<ReverseNameRecord> NamesForAddress
        {
            get => _namesForAddress;
            set => this.RaiseAndSetIfChanged(ref _namesForAddress, value);
        }

        public string AddressQuery { get; set; }

        private bool _loadingDomain;
        public bool LoadingDomain
        {
            get => _loadingDomain;
            set => this.RaiseAndSetIfChanged(ref _loadingDomain, value);
        }

        private bool _loadingDomainReverse;
        public bool LoadingDomainReverse
        {
            get => _loadingDomainReverse;
            set => this.RaiseAndSetIfChanged(ref _loadingDomainReverse, value);
        }

        private string _data;
        public string Data
        {
            get => _data;
            set => this.RaiseAndSetIfChanged(ref _data, value);
        }
        public IEnumerable<string> ConversionTypes { get; } = new string[]
        {
            "Base 64",
            "UTF8",
            "Hex"
        };

        private string _currentConversionType = "Base 64";
        public string CurrentConversionType
        {
            get => _currentConversionType;
            set => this.RaiseAndSetIfChanged(ref _currentConversionType, value);
        }

        private string _nameContent;
        public string NameContent
        {
            get => _nameContent;
            set => this.RaiseAndSetIfChanged(ref _nameContent, value);
        }

        public SolNamingViewModel(IRpcClientProvider rpcProvider)
        {
            _rpcProvider = rpcProvider;
            _loadingDomain = false;
            this.WhenAny(x => x.CurrentConversionType, x => x).Subscribe(x => SetNameContent());
        }

        public async void QuerySolName()
        {
            if(string.IsNullOrEmpty(SolDomainQuery))
            {
                await ShowErrorMessage($"Please insert a valid query.");
                return;
            }

            LoadingDomain = true;
            var res = await Client.GetAddressFromNameAsync(SolDomainQuery);

            if(res.WasSuccessful)
            {
                SolNameRecord = res.ParsedResult;
                SetNameContent();
            }
            else
            {
                SolNameRecord = null;
                NameContent = null;
            }


            LoadingDomain = false;
        }

        public async void QuerySolReverse()
        {
            if(!PublicKey.IsValid(AddressQuery))
            {
                await ShowErrorMessage($"Please insert a valid address.");
                return;
            }
            LoadingDomainReverse = true;

            NamesForAddress = (await Client.GetNamesFromAddressAsync(AddressQuery)).OrderBy(x => x.Name);

            LoadingDomainReverse = false;
        }


        private void SetNameContent()
        {
            if (SolNameRecord == null) return;

            NameContent = CurrentConversionType switch
            {
                "UTF8" => System.Text.Encoding.UTF8.GetString(SolNameRecord.Value),
                "Base 64" => Convert.ToBase64String(SolNameRecord.Value),
                _ => BitConverter.ToString(SolNameRecord.Value).Replace("-", "")
            };
        }
    }
}
