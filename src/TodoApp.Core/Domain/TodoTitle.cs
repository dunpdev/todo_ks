namespace TodoApp.Core.Domain;

public sealed record TodoTitle
{
    public const int MaxLength = 120;

    public string Value { get; }

    public TodoTitle(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var normalizedValue = value.Trim();
        if (normalizedValue.Length > MaxLength)
        {
            throw new ArgumentException(
                $"Title must be {MaxLength} characters or fewer.",
                nameof(value));
        }

        Value = normalizedValue;
    }

    public override string ToString() => Value;
}
