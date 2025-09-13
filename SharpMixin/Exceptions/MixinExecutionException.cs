namespace SharpMixin.Exceptions;

public class MixinExecutionException : Exception
{
    public MixinExecutionException(string message) : base(message) { }
    public MixinExecutionException(string message, Exception innerException) : base(message, innerException) { }
}