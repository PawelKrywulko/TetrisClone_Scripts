using System;
using System.Runtime.Serialization;

[Serializable]
public class MissingSceneObjectException : Exception
{
    public MissingSceneObjectException() {}

    public MissingSceneObjectException(Type type) : base($"Couldn't find object of type {type.FullName} in the scene hierarchy") {}
    public MissingSceneObjectException(string message) : base(message) {}
    public MissingSceneObjectException(string message, Exception inner) : base(message, inner) {}
    private MissingSceneObjectException(SerializationInfo info, StreamingContext context) : base(info, context) {}
}