
using Ost.Core.ViewModels;
using Ost.Services;
using ReactiveUI;
using Solnet.Programs.Clients;
using Solnet.Programs.Models.NameService;
using Solnet.Wallet;
using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;

namespace Ost.ViewModels.NameService
{
    public class TwitterNamingViewModel : ViewModelBase
    {
        private IRpcClientProvider _rpcProvider;

        private NameServiceClient Client => new NameServiceClient(_rpcProvider.Client);

        public string Header => "Twitter Names";

        public string SolDomainQuery { get; set; }

        private ReverseTwitterRecord _reverseTwitterNameRecord;
        public ReverseTwitterRecord ReverseTwitterNameRecord
        {
            get => _reverseTwitterNameRecord;
            set => this.RaiseAndSetIfChanged(ref _reverseTwitterNameRecord, value);
        }

        public string TwitterNameQuery { get; set; }

        private NameRecord _twitterNameRecord;
        public NameRecord TwitterNameRecord
        {
            get => _twitterNameRecord;
            set => this.RaiseAndSetIfChanged(ref _twitterNameRecord, value);
        }

        private string _nameContent;
        public string NameContent
        {
            get => _nameContent;
            set => this.RaiseAndSetIfChanged(ref _nameContent, value);
        }

        public string AddressQuery { get; set; }

        private bool _loadingTwitter;
        public bool LoadingTwitter
        {
            get => _loadingTwitter;
            set => this.RaiseAndSetIfChanged(ref _loadingTwitter, value);
        }
        private bool _loadingTwitterHandle;
        public bool LoadingTwitterHandle
        {
            get => _loadingTwitterHandle;
            set => this.RaiseAndSetIfChanged(ref _loadingTwitterHandle, value);
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

        public TwitterNamingViewModel(IRpcClientProvider rpcProvider)
        {
            _rpcProvider = rpcProvider;
            _loadingTwitter = false;
            this.WhenAny(x => x.CurrentConversionType, x => x).Subscribe(x => SetNameContent());
        }

        public async void QueryTwitter()
        {
            if(string.IsNullOrEmpty(TwitterNameQuery))
            {
                await ShowErrorMessage($"Please insert a non-empty twitter handle..");
                return;
            }

            LoadingTwitter = true;
            var res = await Client.GetAddressFromTwitterHandleAsync(TwitterNameQuery);

            if (res.WasSuccessful)
            {
                TwitterNameRecord = res.ParsedResult;
                SetNameContent();
            }
            else
            {
                TwitterNameRecord = null;
                NameContent = null;
            }


            LoadingTwitter = false;
        }
        public async void QueryTwitterHandle()
        {
            if(!PublicKey.IsValid(AddressQuery))
            {
                await ShowErrorMessage($"Please insert a valid address.");
                return;
            }

            LoadingTwitterHandle = true;
            var res = await Client.GetTwitterHandleFromAddressAsync(AddressQuery);

            if (res.WasSuccessful)
            {
                ReverseTwitterNameRecord = res.ParsedResult;
            }
            else
            {
                ReverseTwitterNameRecord = null;
            }


            LoadingTwitterHandle = false;
        }

        private void SetNameContent()
        {
            if (TwitterNameRecord == null) return;

            NameContent = CurrentConversionType switch
            {
                "UTF8" => System.Text.Encoding.UTF8.GetString(TwitterNameRecord.Value),
                "Base 64" => Convert.ToBase64String(TwitterNameRecord.Value),
                _ => BitConverter.ToString(TwitterNameRecord.Value)
            };
        }

    }
}
