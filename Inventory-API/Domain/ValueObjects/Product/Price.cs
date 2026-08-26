namespace Domain.ValueObjects.Product
{
    public readonly record struct Price(decimal amount, string currency);
}
