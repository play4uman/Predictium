using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predictium.Models
{
    public enum CryptoCurrencyType
    {
        ETH,
        BTC,
        DOT
    }

    public class CryptoCurrencyFullName
    {
        public const string Etherium = "Ethereum";
        public const string Bitcoin = "Bitcoin";
        public const string Polkadot = "Polkadot";
        private readonly static Dictionary<CryptoCurrencyType, string> typeToNameMapping = new Dictionary<CryptoCurrencyType, string>
        {
            { CryptoCurrencyType.ETH, Etherium },
            { CryptoCurrencyType.BTC, Bitcoin },
            { CryptoCurrencyType.DOT, Polkadot}
        };

        public static string GetCryptoCurrencyNameByType(CryptoCurrencyType cryptoCurrencyType)
            => typeToNameMapping[cryptoCurrencyType];
    }
}
