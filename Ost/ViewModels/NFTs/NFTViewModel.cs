using Ost.Core.ViewModels;
using Ost.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ReactiveUI;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ost.ViewModels.NFTs
{
    public class OwnerCountVM : ViewModelBase
    {
        private string _owner;

        public string Owner
        {
            get => _owner;
            set => this.RaiseAndSetIfChanged(ref _owner, value);
        }

        private int _count;

        public int Count
        {
            get => _count;
            set => this.RaiseAndSetIfChanged(ref _count, value);
        }

    }

    public class NFTViewModel : ViewModelBase
    {
        public string Header => "Find Holders";

        private string _searchString = string.Empty;
        private IRpcClient rpc => _rpcClientProvider.Client;

        public string SearchString
        {
            get => _searchString;
            set => this.RaiseAndSetIfChanged(ref _searchString, value);
        }


        private string _result;
        private ObservableCollection<OwnerCountVM> _pairs;
        private bool _isProcessing;
        private string auth;
        private IRpcClientProvider _rpcClientProvider;

        public string Result
        {
            get => _result;
            set => this.RaiseAndSetIfChanged(ref _result, value);
        }

        public ObservableCollection<OwnerCountVM> Pairs
        {
            get => _pairs;
            set => this.RaiseAndSetIfChanged(ref _pairs, value);
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set => this.RaiseAndSetIfChanged(ref _isProcessing, value);
        }


        private bool _loadingNFT = false;
        public bool Loading
        {
            get => _loadingNFT;
            set => this.RaiseAndSetIfChanged(ref _loadingNFT, value);
        }

        private bool _loadedNFT = false;
        public bool LoadedNFT
        {
            get => _loadedNFT;
            set => this.RaiseAndSetIfChanged(ref _loadedNFT, value);
        }

        private bool _showCollectionOwners = false;
        public bool ShowCollectionOwners
        {
            get => _showCollectionOwners;
            set => this.RaiseAndSetIfChanged(ref _showCollectionOwners, value);
        }


        private string _status;

        public string Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }


        public byte[] NftData { get; set; }



        private Bitmap _bmp;
        public Bitmap Bmp
        {
            get => _bmp;
            set => this.RaiseAndSetIfChanged(ref _bmp, value);
        }

        private bool _canQueryHolders = false;
        public bool CanQueryHolders
        {
            get => _canQueryHolders;
            set => this.RaiseAndSetIfChanged(ref _canQueryHolders, value);
        }


        private bool _canExportMints = false;
        public bool CanExportMints
        {
            get => _canExportMints;
            set => this.RaiseAndSetIfChanged(ref _canExportMints, value);
        }


        private bool _canExportHolders = false;
        public bool CanExportHolders
        {
            get => _canExportHolders;
            set => this.RaiseAndSetIfChanged(ref _canExportHolders, value);
        }



        private List<string> _mints;
        public List<string> Mints
        {
            get => _mints;
            set
            {
                this.RaiseAndSetIfChanged(ref _mints, value);
                CanQueryHolders = _mints != null && _mints.Count > 0;
            }
        }

        public NFTViewModel(IRpcClientProvider rpcClientProvider)
        {
            _rpcClientProvider = rpcClientProvider;
        }

        public async void Save()
        {
            var sfd = new SaveFileDialog()
            {
                Title = "Save NFT"
            };


            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var selected = await sfd.ShowAsync(desktop.MainWindow);
                if (selected == null) return;
                if (selected.Length > 0)
                {
                    using (var f = File.OpenWrite(selected))
                    {
                        await f.WriteAsync(NftData);
                    }
                }
            }
        }

        public async void SaveMetadata()
        {
            var sfd = new SaveFileDialog()
            {
                Title = "Save NFT Metadata",
                DefaultExtension = "json",
                Filters = new() { new FileDialogFilter() { Extensions = new() { "json" }, Name = "JSON Files" } },
                InitialFileName = SearchString,
                Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };


            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var selected = await sfd.ShowAsync(desktop.MainWindow);
                if (selected == null) return;
                if (selected.Length > 0)
                {
                    using (var s = new StreamWriter(selected))
                    {
                        await s.WriteAsync(Result);
                    }
                }
            }
        }

        public async void FindHolders()
        {
            Loading = true;

            Status = "Fetching Mints...";

            Mints = new();
            Pairs = new();

            CanExportHolders = false;
            CanExportMints = false;

            var metaAccs = await rpc.GetProgramAccountsAsync("metaqbxxUerdq28cj1RbAWkYQm3ybzjb6a8bt518x1s",
                memCmpList: new List<MemCmp>() { new MemCmp() { Bytes = auth, Offset = 326 } });

            List<string> mintList = new();
            foreach (var acc in metaAccs.Result)
            {
                var bytes = Convert.FromBase64String(acc.Account.Data[0]);
                var pkey = new ReadOnlySpan<byte>(bytes).GetPubKey(33);
                mintList.Add(pkey.Key);
            }

            Mints = mintList;
            CanExportMints = true;
            ShowCollectionOwners = true;

            Dictionary<string, OwnerCountVM> accounts = new();
            int tot = 0;
            await Task.Run(() =>
            {
                List<string> addresses = new();
                var batcher = new SolanaRpcBatchWithCallbacks(rpc);
                batcher.AutoExecute(Solnet.Rpc.Types.BatchAutoExecuteMode.ExecuteWithCallbackFailures, 100);

                foreach (var mint in Mints)
                {
                    batcher.GetTokenLargestAccounts(mint, callback: (res, e) =>
                    {
                        Status = $"Fetching Token Accs {tot}/{Mints.Count}...";
                        if (res.Value?.Count > 0)
                            lock (addresses)
                            {
                                addresses.Add(res.Value[0].Address);
                                tot++;
                            }
                    });
                }
                batcher.Flush();
                tot = 0;
                foreach (var acc in addresses)
                {
                    batcher.GetTokenAccountInfo(acc, callback: (a, e) =>
                    {
                        Status = $"Fetching Owners {tot}/{Mints.Count}...";

                        var owner = a.Value.Data.Parsed.Info.Owner;

                        lock (accounts)
                        {
                            if (!accounts.TryGetValue(owner, out OwnerCountVM vm))
                            {
                                vm = new()
                                {
                                    Owner = owner
                                };
                                accounts.Add(owner, vm);

                                Dispatcher.UIThread.Post(() => Pairs.Add(vm));
                            }

                            vm.Count++;
                            tot++;
                        }
                    });

                }

                batcher.Flush();

            });

            Status = "";

            Loading = false;
            CanExportHolders = true;
        }

        public async void ExportMints()
        {
            var sfd = new SaveFileDialog()
            {
                Title = "Save Mints",
                DefaultExtension = ".txt",
                Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };


            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var selected = await sfd.ShowAsync(desktop.MainWindow);
                if (selected == null) return;
                if (selected.Length > 0)
                {
                    using (var s = new StreamWriter(selected))
                    {
                        foreach (var mint in Mints)
                            await s.WriteLineAsync(mint);
                    }
                }
            }
        }

        public async void ExportOwners()
        {
            var sfd = new SaveFileDialog()
            {
                Title = "Export Owners",
                DefaultExtension = ".csv",
                Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filters = new List<FileDialogFilter>() { new FileDialogFilter() { Extensions = new List<string>() { "csv" }, Name = "Comma-Separated Values" } }
            };


            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var selected = await sfd.ShowAsync(desktop.MainWindow);
                if (selected == null) return;
                if (selected.Length > 0)
                {
                    using (var s = new StreamWriter(selected))
                    {
                        await s.WriteLineAsync("Owner,Count");
                        foreach (var o in Pairs)
                            await s.WriteLineAsync($"{o.Owner},{o.Count}");
                    }
                }
            }
        }

        public async void QueryByAddress()
        {
            if(!PublicKey.IsValid(SearchString))
            {
                await ShowErrorMessage($"'{SearchString}' is not a valid address");
                return;
            }

            Loading = true;
            Result = null;
            Bmp = null;
            NftData = null;


            List<byte[]> seeds = new List<byte[]>();
            var metaProgram = new PublicKey("metaqbxxUerdq28cj1RbAWkYQm3ybzjb6a8bt518x1s");
            seeds.Add(System.Text.Encoding.UTF8.GetBytes("metadata"));
            seeds.Add(metaProgram.KeyBytes);
            seeds.Add(new PublicKey(SearchString).KeyBytes);
            PublicKey.TryFindProgramAddress(seeds, metaProgram, out var pk, out _);

            var metaAcc = await rpc.GetAccountInfoAsync(pk);

            if(!metaAcc.WasSuccessful || metaAcc.Result.Value == null)
            {
                await ShowErrorMessage($"Unable to fetch metadata account for address '{SearchString}'.\n Are you sure this is the mint address for an NFT?");
                Loading = false;
                return;
            }


            var bytes = Convert.FromBase64String(metaAcc.Result.Value.Data[0]);

            var uri = ParseUri(bytes).Trim('\0');
            auth = GetAuthority(bytes);

            string json = "";
            try
            {
                using (var wc = new WebClient())
                {
                    var data = await wc.DownloadDataTaskAsync(uri);

                    json = Encoding.UTF8.GetString(data);
                }
            }
            catch(Exception) {}

            if(string.IsNullOrEmpty(json))
            {
                await ShowErrorMessage($"Unable to fetch json metadata from:\n{uri}");
                Loading = false;
                return;
            }

            string img = string.Empty, newJson = string.Empty;
            try
            {
                var jsonDoc = JsonDocument.Parse(json);
                newJson = JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions { WriteIndented = true });


                img = jsonDoc.RootElement.GetProperty("image").GetString();
            }
            catch(Exception){}


            if(string.IsNullOrEmpty(json))
            {
                await ShowErrorMessage($"Unable to retrieve 'image' property from the json.");
                Loading = false;
                return;
            }

            try
            {
                using (var wc = new WebClient())
                {
                    NftData = await wc.DownloadDataTaskAsync(img);

                    Bmp = new Bitmap(new MemoryStream(NftData));
                }
            }
            catch(Exception)
            {
                await ShowErrorMessage($"Unable to download NFT content.");
            }


            Result = newJson;

            Loading = false;
            LoadedNFT = true;
        }

        public async void Search()
        {
            IsProcessing = true;

            List<byte[]> seeds = new List<byte[]>();
            var metaProgram = new PublicKey("metaqbxxUerdq28cj1RbAWkYQm3ybzjb6a8bt518x1s");
            seeds.Add(System.Text.Encoding.UTF8.GetBytes("metadata"));
            seeds.Add(metaProgram.KeyBytes);
            seeds.Add(new PublicKey(SearchString).KeyBytes);
            PublicKey.TryFindProgramAddress(seeds, metaProgram, out var pk, out _);

            var tx = await rpc.GetTransactionAsync("45pGoC4Rr3fJ1TKrsiRkhHRbdUeX7633XAGVec6XzVdpRbzQgHhe6ZC6Uq164MPWtiqMg7wCkC6Wy3jy2BqsDEKf");

            var metaAcc = await rpc.GetAccountInfoAsync(pk);
            var bytes = Convert.FromBase64String(metaAcc.Result.Value.Data[0]);

            var uri = ParseUri(bytes).Trim('\0');
            auth = GetAuthority(bytes);

            string json = "";
            using (var wc = new WebClient())
            {
                var data = await wc.DownloadDataTaskAsync(uri);

                json = Encoding.UTF8.GetString(data);
            }

            var jsonDoc = JsonDocument.Parse(json);
            var newJson = JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions { WriteIndented = true });

            Result = newJson;
            IsProcessing = false;
        }

        private string ParseUri(byte[] data)
        {

            var span = new ReadOnlySpan<byte>(data);
            var len = span.GetBorshString(115, out var uri);
            var len2 = uri.Length;
            return uri;
        }

        private string GetAuthority(byte[] data)
        {
            var span = new ReadOnlySpan<byte>(data);
            return span.GetPubKey(326);
        }
    }
}
