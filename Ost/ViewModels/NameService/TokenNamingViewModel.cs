
using Ost.Core.ViewModels;
using Ost.Services;
using Avalonia.Media.Imaging;
using ReactiveUI;
using Solnet.Programs.Clients;
using Solnet.Programs.Models.NameService;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using SkiaSharp;
using Svg.Skia;
using Solnet.Wallet;

namespace Ost.ViewModels.NameService
{
    public class TokenNamingViewModel : ViewModelBase
    {
        private IRpcClientProvider _rpcProvider;

        private NameServiceClient Client => new NameServiceClient(_rpcProvider.Client);

        public string Header => "Token Registry";

        public string SolDomainQuery { get; set; }

        private ReverseTokenNameRecord _reverseMintNameRecord;
        public ReverseTokenNameRecord ReverseMintNameRecord
        {
            get => _reverseMintNameRecord;
            set => this.RaiseAndSetIfChanged(ref _reverseMintNameRecord, value);
        }

        private Bitmap _tokenIcon;
        public Bitmap Icon
        {
            get => _tokenIcon;
            set => this.RaiseAndSetIfChanged(ref _tokenIcon, value);
        }

        public string TokenMintQuery { get; set; }

        private TokenNameRecord _mintNameRecord;
        public TokenNameRecord MintNameRecord
        {
            get => _mintNameRecord;
            set => this.RaiseAndSetIfChanged(ref _mintNameRecord, value);
        }

        public string TickerQuery { get; set; }

        private bool _loadingTwitter;
        public bool LoadingMint
        {
            get => _loadingTwitter;
            set => this.RaiseAndSetIfChanged(ref _loadingTwitter, value);
        }

        private bool _loadingTwitterHandle;
        public bool LoadingTicker
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

        public TokenNamingViewModel(IRpcClientProvider rpcProvider)
        {
            _rpcProvider = rpcProvider;
            _loadingTwitter = false;
        }

        public async void QueryTokenMint()
        {
            if(!PublicKey.IsValid(TokenMintQuery))
            {
                await ShowErrorMessage($"Please insert a valid token mint address.");
                return;
            }

            LoadingMint = true;
            var res = await Client.GetTokenInfoFromMintAsync(TokenMintQuery);

            if (res.WasSuccessful)
            {
                MintNameRecord = res.ParsedResult;
            }
            else
            {
                MintNameRecord = null;
            }

            Icon = await LoadIcon(MintNameRecord?.Value?.LogoUri);

            LoadingMint = false;
        }
        public async void QueryTokenName()
        {
            if(string.IsNullOrEmpty(TickerQuery))
            {
                await ShowErrorMessage($"Please insert a valid ticker.");
                return;
            }

            LoadingTicker = true;
            var res = await Client.GetMintFromTokenTickerAsync(TickerQuery);

            if (res.WasSuccessful)
            {
                ReverseMintNameRecord = res.ParsedResult;
            }
            else
            {
                ReverseMintNameRecord = null;
            }


            LoadingTicker = false;
        }

        public async Task<Bitmap> LoadIcon(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return null;

            try
            {
                using (var wc = new WebClient())
                {
                    var ico = await wc.DownloadDataTaskAsync(uri);

                    if(uri.EndsWith(".svg"))
                    {
                        ico = ConvertSvgToBitmap(ico);
                    }


                    Bitmap b = new Bitmap(new MemoryStream(ico));

                    return b;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private byte[] ConvertSvgToBitmap(byte[] ico)
        {
            using(var svg = new SKSvg())
            {
                svg.Load(new MemoryStream(ico));

                var res = new MemoryStream();

                svg.Save(res, SKColor.Empty);

                return res.ToArray();
            }
        }
    }
}
