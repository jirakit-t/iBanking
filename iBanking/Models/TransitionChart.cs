using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iBanking.Models
{
    public class TransitionChart
    {
        public int Date;
        public decimal? Deposit;
        public decimal? Withdraw;
        public decimal? Transfer;
        public decimal? Fee;
        public decimal? Balance;
        public decimal? NumberAccount;
        public decimal? NumberBanker;
    }
}