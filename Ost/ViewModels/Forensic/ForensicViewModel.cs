using Ost.Core.ViewModels;
using Ost.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ReactiveUI;
using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Ost.ViewModels.Forensic
{
    public class TxWrapper
    {
        public string Signature { get; set; }

        public TransactionMetaSlotInfo TransactionInfo { get; set; }

        public List<DecodedInstruction> DecodedInstructions { get; set; }
    }


    public class Node : ViewModelBase
    {
        private string _from;
        private string _to;
        private ulong _volume;
        private int _count;

        public List<string> Transactions { get; } = new();



        public string From { get => _from; set => this.RaiseAndSetIfChanged(ref _from, value); }

        public string To { get => _to; set => this.RaiseAndSetIfChanged(ref _to, value); }

        public ulong Volume 
        { 
            get => _volume; 
            set 
            { 
                this.RaiseAndSetIfChanged(ref _volume, value);
                this.RaisePropertyChanged("VolumeSOL");
            } 
        }
        public double VolumeSOL { get => _volume / 1_000_000_000.0; }

        public int Count { get => _count; set => this.RaiseAndSetIfChanged(ref _count, value); }
    }

    public class ForensicViewModel : ViewModelBase
    {
        public string Header => "Forensic";

        private IRpcClient rpc => _rpcClientProvider.Client;

        Dictionary<string, Node> sent = new(), received = new();

        private IRpcClientProvider _rpcClientProvider;
        private bool _loading = false;
        private string _searchString;
        private TxWrapper _selectedTransaction;

        public string SearchString
        {
            get => _searchString;
            set => this.RaiseAndSetIfChanged(ref _searchString, value);
        }


        public bool Loading
        {
            get => _loading;
            set => this.RaiseAndSetIfChanged(ref _loading, value);
        }

        public ObservableCollection<TxWrapper> Transactions { get; } = new();

        public ObservableCollection<Node> InboundTxs { get; } = new();
        public ObservableCollection<Node> OutboundTxs { get; } = new();


        public TxWrapper SelectedTransaction { get => _selectedTransaction; set => this.RaiseAndSetIfChanged(ref _selectedTransaction, value); }

        public ForensicViewModel(IRpcClientProvider rpcClientProvider)
        {
            _rpcClientProvider = rpcClientProvider;
        }


        public async void QueryByAddress()
        {
            Loading = true;

            InboundTxs.Clear();
            OutboundTxs.Clear();
            Transactions.Clear();


            if (!Ed25519Extensions.IsOnCurve(Encoders.Base58.DecodeData(_searchString)))
                return;

            string before = null;
            string wallet = _searchString;

            List<string> _signatures = new();

            var signatures = await rpc.GetSignaturesForAddressAsync(wallet, 1000, before);

            while (signatures.WasSuccessful && signatures.Result != null && signatures.Result.Count > 0)
            {
                foreach (var sig in signatures.Result)
                {
                    _signatures.Add(sig.Signature);
                }

                before = signatures.Result[signatures.Result.Count - 1].Signature;

                if (signatures.Result.Count == 1000)
                {
                    Thread.Sleep(1000);
                    signatures = await rpc.GetSignaturesForAddressAsync(wallet, 1000, before);
                }
                else
                {
                    break;
                }
            }

            await Task.Run(() =>
           {

               var batcher = new SolanaRpcBatchWithCallbacks(rpc);

               batcher.AutoExecute(Solnet.Rpc.Types.BatchAutoExecuteMode.ExecuteWithCallbackFailures, 100);

               foreach (var sig in _signatures)
               {
                   batcher.GetTransaction(sig, callback: (tx, e) => HandleTx(tx, e, wallet));
               }

               batcher.Flush();
           });

            Loading = false;
        }

        private async void HandleTx(TransactionMetaSlotInfo transaction, Exception e, string wallet)
        {
            if(transaction == null)
            {
                if(e != null)
                {
                    var dialog = Material.Dialog.DialogHelper.CreateAlertDialog(new Material.Dialog.AlertDialogBuilderParams()
                    {
                        ContentHeader = "Exception loading tx",
                        SupportingText = e.Message,
                        StartupLocation = WindowStartupLocation.CenterOwner,
                    });
                    if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                        await dialog.ShowDialog(desktop.MainWindow);
                }
                return;
            }

            var decoded = InstructionDecoder.DecodeInstructions(transaction);

            var wrapper = new TxWrapper() { DecodedInstructions = decoded, Signature = transaction.Transaction.Signatures[0], TransactionInfo = transaction };

            Dispatcher.UIThread.Post(() => Transactions.Add(wrapper));


            foreach(var tx in decoded)
            {
                if (tx.ProgramName == "System Program" && tx.InstructionName == "Transfer")
                {
                    string to = (PublicKey)tx.Values["To Account"];
                    string from = (PublicKey)tx.Values["From Account"];
                    var amount = (ulong)tx.Values["Amount"];

                    Node node = null;

                    if (wallet == to)
                    {
                        lock (received)
                        {
                            if (!received.TryGetValue(from, out node))
                            {
                                node = new Node()
                                {
                                    From = from,
                                    To = wallet,
                                    Volume = 0,
                                    Count = 0
                                };
                                received.Add(from, node);
                                Dispatcher.UIThread.Post(() => InboundTxs.Add(node));
                            }
                        }
                    }
                    else if (wallet == from)
                    {
                        lock (sent)
                        {
                            if (!sent.TryGetValue(to, out node))
                            {
                                node = new Node()
                                {
                                    From = wallet,
                                    To = to,
                                    Volume = 0,
                                    Count = 0
                                };
                                sent.Add(to, node);
                                Dispatcher.UIThread.Post(() => OutboundTxs.Add(node));
                            }
                        }
                    }

                    if (node != null)
                    {
                        node.Volume += amount;
                        node.Count++;
                    }
                }
            }
        }

        private void HandleProcessed()
        {

        }
    }
}
