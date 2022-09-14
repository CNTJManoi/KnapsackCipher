using KnapsackCipher.Command;
using KnapsackCipher.Cryptographic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KnapsackCipher.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        private DelegateCommand _encodingClick;
        private DelegateCommand _decodingClick;
        Knapsack Knapsack { get; set; }
        public string EncodingText { get; set; }
        public string DecodingText { get; set; }
        public string ResultEncodingText { get; set; }
        public string ResultDecodingText { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public DelegateCommand EncodingClick => _encodingClick ?? (_encodingClick = new DelegateCommand(Encoding));

        public DelegateCommand DecodingClick => _decodingClick ?? (_decodingClick = new DelegateCommand(Decoding));
        public MainViewModel()
        {
            Knapsack = new Knapsack(10);
        }
        private void Encoding(object obj)
        {
            if (EncodingText != "" && EncodingText != null)
            {
                string result = Knapsack.Perform(EncodingText, Knapsack.Operation.Encrypt);
                ResultEncodingText = result;
                DecodingText = result;
                PublicKey = "";
                PrivateKey = "";
                foreach (var item in Knapsack.PublicKey)
                {
                    PublicKey += item + " ";
                }
                foreach (var item in Knapsack.PrivateKey)
                {
                    PrivateKey += item + " ";
                }
                OnPropertyChanged(nameof(DecodingText));
            }
            else ResultEncodingText = "Введите текст!";
            OnPropertyChanged(nameof(ResultEncodingText));
            OnPropertyChanged(nameof(PublicKey));
            OnPropertyChanged(nameof(PrivateKey));
        }
        private void Decoding(object obj)
        {
            if (DecodingText != "" && DecodingText != null)
            {
                string result = Knapsack.Perform(DecodingText, Knapsack.Operation.Decrypt);
                ResultDecodingText = result;
            }
            else ResultDecodingText = "Введите текст!";
            OnPropertyChanged(nameof(ResultDecodingText));
            OnPropertyChanged(nameof(PublicKey));
            OnPropertyChanged(nameof(PrivateKey));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
