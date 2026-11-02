namespace Domain.ValueObjects.Product
{
    public record Price
    {
        public decimal Amount { get; }
        public string Currency { get; }

        private Price(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Price Create(decimal amount, string currency)
        {
            if (amount is null || amount <= 0)
                throw new ArgumentException("Price amount must be greater than zero.", nameof(amount));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be null or whitespace.", nameof(currency));

            return new Price((decimal)amount, currency);
        }

        public Price ChangeAmount(decimal amount) => Create(amount, Currency);
    }
}
