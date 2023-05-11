namespace TaskSystem.Config;

public class Expiration
{
    public int RequestCodeTtl { get; set; }
    public int TokenTtl { get; set; }

    public Expiration(int requestCodeTtl, int tokenTtl)
    {
        RequestCodeTtl = requestCodeTtl;
        TokenTtl = tokenTtl;
    }
}