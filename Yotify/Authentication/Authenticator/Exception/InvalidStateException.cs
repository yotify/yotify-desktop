using System;

namespace Yotify.Authentication.Authenticator.Exception
{
    [Serializable]
    public class InvalidStateException : System.Exception
    {
        public InvalidStateException() { }
        public InvalidStateException(string message) : base(message) { }
        public InvalidStateException(string message, System.Exception inner) : base(message, inner) { }
        protected InvalidStateException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
