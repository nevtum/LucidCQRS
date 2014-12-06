using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LucidCQRS.Domain;

namespace LucidCQRS.Tests.Accounting
{
    public class Account : AggregateRoot
    {
        private string _name;
        private double _balance;

        public Account()
            : base(new Guid())
        {
        }

        public Account(Guid id, string accountHolder)
            : base(id)
        {
            ApplyNewChange(new AccountCreated(Id, accountHolder));
        }

        public string GetAccountHolder()
        {
            return _name;
        }

        public double GetCurrentBalance()
        {
            return _balance;
        }

        public void Deposit(double amount)
        {
            ApplyNewChange(new DepositMade(Id, amount));
        }

        public void Withdraw(double amount)
        {
            if (amount > _balance)
                throw new Exception("Cannot withdraw more than balance holds!");

            ApplyNewChange(new WithdrawalMade(Id, amount));
        }

        #region Handlers

        public void Apply(AccountCreated e)
        {
            _name = e.AccountHolder;
            _balance = 0.0;
        }

        public void Apply(WithdrawalMade e)
        {
            _balance -= e.Amount;
        }

        public void Apply(DepositMade e)
        {
            _balance += e.Amount;
        }

        #endregion
    }
}
