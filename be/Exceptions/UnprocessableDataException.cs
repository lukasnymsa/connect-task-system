namespace TaskSystem.Exceptions;

public class UnprocessableDataException : Exception
{
    public UnprocessableDataException(string message)
        : base(message)
    {
    }
}