namespace BuildingBlocks.Error;

public sealed class Result<T>
{

    public Result(T value)
    {
        _value = value;
    }

    public Result(ResultError err)
    {
        ArgumentNullException.ThrowIfNull(err);
        _error = err;
    }
    
    public static implicit operator Result<T>(ResultError error) => new(error);
    public static implicit operator Result<T>(T value) => new(value);

    private readonly T? _value;

    private readonly ResultError? _error;

    public bool IsSuccess => _error is null;

    public T Value => IsSuccess 
        ? _value! 
        : throw new InvalidOperationException("Cannot read value of failed result. Check is success First."); 

    public ResultError Error => _error ?? throw new InvalidOperationException("Cannot read Error of success result. Check is success First.");
}

public sealed record ResultError(KindErr Kind, string Code, string ErrMessage);

public enum KindErr
{
    Validation,
    NotFound,
    NotOwned,
    Conflict,
    BusinessRule
}