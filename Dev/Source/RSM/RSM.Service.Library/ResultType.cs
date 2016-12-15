using System;
using System.Runtime.Serialization;

namespace RSM.Service.Library
{
	[Flags]
	public enum ResultType
	{
		[EnumMember]
		Success = 2,

		[EnumMember]
		Warning = 4,

		[EnumMember]
		BusinessError = 8,

		[EnumMember]
		DataError = 16,

		[EnumMember]
		ValidationError = 32,

		[EnumMember]
		TechnicalError = 64,

		[EnumMember]
		SecurityFailure = 128
	}
}
