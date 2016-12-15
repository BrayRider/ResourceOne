using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSM.Service.Library
{
	public class Scheduler
	{
		public DateTime DailyStart { get; set; }
		public bool Repeat { get; set; }
		public TimeSpan RepeatInterval { get; set; }

		public Scheduler()
		{
			DailyStart = DateTime.Today;
			Repeat = false;
			RepeatInterval = TimeSpan.MinValue;
		}

		/// <summary>
		/// Calculates when the next event should occur based on a repeat interval starting from 12:00am daily.
		/// </summary>
		/// <returns></returns>
		public TimeSpan NextDueTime(long? iterations = null)
		{
			if (!Repeat && iterations != null && iterations > 1)
				return TimeSpan.FromMilliseconds(-1);

			var now = DateTime.Now;
			var elapsed = now.Subtract(now.Date);

			long remainder;
			var gap = Math.DivRem((long)elapsed.TotalSeconds, (long)RepeatInterval.TotalSeconds, out remainder);

			return remainder == 0 ? RepeatInterval : TimeSpan.FromSeconds(remainder);
		}

		/// <summary>
		/// Calculates when the next interval should be.
		/// </summary>
		/// <returns></returns>
		public TimeSpan NextInterval()
		{
			return Repeat ? RepeatInterval : TimeSpan.FromMilliseconds(-1);
		}
	}
}
