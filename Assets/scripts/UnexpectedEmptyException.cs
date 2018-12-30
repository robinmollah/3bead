using System;
using System.Runtime.Serialization;

[Serializable]
internal class UnexpectedEmptyException : Exception
{
    public UnexpectedEmptyException()
    {
    }

    public UnexpectedEmptyException(string message) : base(message)
    {
        
    }

    public UnexpectedEmptyException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected UnexpectedEmptyException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}