using System;

namespace Soloco.Talks.PolyglotPersistence.A_IsolationLevels
{
    public class Account
    {
        public string Id { get; set; }
        public int Amount { get; private set; }

        public Account(string id, int amount)
        {
            Id = id;
            Amount = amount;
        }

        public void Substract(int amountToSubstract)
        {
            if (amountToSubstract > Amount) throw new InvalidOperationException("Insufficient funds");

            Amount -= amountToSubstract;
        }

        public override string ToString()
        {
            return $"Account '{Id}': {Amount} €";
        }
    }
}
