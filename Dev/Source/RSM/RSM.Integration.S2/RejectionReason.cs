using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSM.Service.Library.Model
{
	public enum RejectionReason
	{
		CardNotInLocalDb = 1,
		CardNotInS2NCDb = 2,
		WrongTime = 3,
		WrongLocation = 4,
		CardMisread = 5,
		TailgateViolation = 6,
		AntiPassbackViolation = 7,
		WrongDay = 9,
		CardExpired = 14,
		CardBitLengthMismatch = 15,
		WrongDay2 = 16, // Keeping duplicate from S2 documentation
		ThreatLevel = 17 
	}
}
