using System;

namespace Soloco.Talks.PolyglotPersistence.TestData
{
    public class Account
    {
        public string ID { get; private set; }
        public int Amount { get; private set; }

        public Account(string id, int amount)
        {
            ID = id;
            Amount = amount;
        }

        public void Substract(int amountToSubstract)
        {
            if (amountToSubstract > Amount) throw new InvalidOperationException("Insufficient funds");

            Amount -= amountToSubstract;
        }

        public override string ToString()
        {
            return $"Account '{ID}': {Amount} €";
        }
    }
}
