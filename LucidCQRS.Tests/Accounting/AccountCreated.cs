using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LucidCQRS.Common;

namespace LucidCQRS.Tests.Accounting
{
    [Serializable]
    public class AccountCreated : Event
    {
        public AccountCreated(Guid id, string accountHolder)
            : base(id)
        {
            AccountHolder = accountHolder;
        }

        public string AccountHolder { get; private set; }
    }
}
