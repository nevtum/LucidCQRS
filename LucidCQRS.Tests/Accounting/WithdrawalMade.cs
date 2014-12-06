using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LucidCQRS.Common;

namespace LucidCQRS.Tests.Accounting
{
    [Serializable]
    public class WithdrawalMade : Event
    {
        public WithdrawalMade(Guid Id, double amount)
            : base(Id)
        {
            Amount = amount;
        }

        public double Amount { get; private set; }
    }
}
