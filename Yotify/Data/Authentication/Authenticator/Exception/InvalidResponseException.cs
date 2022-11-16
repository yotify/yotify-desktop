using System;

namespace Yotify.Data.Authentication.Authenticator.Exception
{
	[Serializable]
	public class InvalidResponseException : System.Exception
	{
		public InvalidResponseException() { }
		public InvalidResponseException(string message) : base(message) { }
        public InvalidResponseException(string message, System.Exception inner) : base(message, inner) { }
		protected InvalidResponseException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
