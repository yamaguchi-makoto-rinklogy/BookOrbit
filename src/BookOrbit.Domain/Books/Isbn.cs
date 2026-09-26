namespace BookOrbit.Domain.Books;

public sealed record Isbn
{
    public string Value { get; }
    
    public Isbn(string value) 
    {
        var normalized = value
            .Replace("-", "")
            .Replace(" ", "");

        if (normalized.Length is not 10 and not 13)
        {
            throw new ArgumentException(
                "ISBNは10桁または13桁で指定してください。",
                nameof(value));
        }
        
        Value = normalized;
    }

    public override string ToString()
        => Value;
}