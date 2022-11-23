using System;

namespace Yotify.Authentication.Authenticator.Exception
{
	[Serializable]
	public class InvalidCodeException : System.Exception
	{
		public InvalidCodeException() { }
		public InvalidCodeException(string message) : base(message) { }
		public InvalidCodeException(string message, System.Exception inner) : base(message, inner) { }
		protected InvalidCodeException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
