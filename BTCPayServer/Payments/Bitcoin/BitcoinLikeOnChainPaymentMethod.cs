﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTCPayServer.Services.Invoices;
using BTCPayServer.Services.Wallets;
using NBitcoin;
using NBXplorer.JsonConverters;
using Newtonsoft.Json;

namespace BTCPayServer.Payments.Bitcoin
{
    public class BitcoinLikeOnChainPaymentMethod : IPaymentMethodDetails
    {
        public PaymentType GetPaymentType() => PaymentTypes.BTCLike;

        public string GetPaymentDestination()
        {
            return DepositAddress;
        }

        public decimal GetNextNetworkFee()
        {
            return NextNetworkFee.ToDecimal(MoneyUnit.BTC);
        }

        public decimal GetFeeRate()
        {
            return FeeRate.SatoshiPerByte;
        }

        public void SetPaymentDestination(string newPaymentDestination)
        {
            DepositAddress = newPaymentDestination;
        }
        public Data.NetworkFeeMode NetworkFeeMode { get; set; }

        FeeRate _NetworkFeeRate;
        [JsonConverter(typeof(FeeRateJsonConverter))]
        public FeeRate NetworkFeeRate
        {
            get
            {
                // Some old invoices don't have this field set, so we fallback on FeeRate
                return _NetworkFeeRate ?? FeeRate;
            }
            set
            {
                _NetworkFeeRate = value;
            }
        }

        // Those properties are JsonIgnore because their data is inside CryptoData class for legacy reason
        [JsonIgnore]
        public FeeRate FeeRate { get; set; }
        [JsonIgnore]
        public Money NextNetworkFee { get; set; }
        [JsonIgnore]
        public String DepositAddress { get; set; }

        public PayJoinPaymentState PayJoin { get; set; } = new PayJoinPaymentState();
        
        

        public BitcoinAddress GetDepositAddress(Network network)
        {
            return string.IsNullOrEmpty(DepositAddress) ? null : BitcoinAddress.Create(DepositAddress, network);
        }
        ///////////////////////////////////////////////////////////////////////////////////////
    }

    public class PayJoinPaymentState
    {
        public bool Enabled { get; set; } = false;
        public uint256 ProposedTransactionHash { get; set; }
        public List<ReceivedCoin> CoinsExposed { get; set; }
        public decimal TotalOutputAmount { get; set; }
        public decimal ContributedAmount { get; set; }
        public uint256 OriginalTransactionHash { get; set; }
    }
}
