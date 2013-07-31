using System;
using System.Globalization;


namespace Time
{
	public sealed class TimeUtilities
	{
		private TimeUtilities()
		{
		}

		public static string IntervalToStringHHHMMSSDec(
			TimeSpan Interval
			)
		{
			const char SeparatorForHMS = ':';
			const int HoursPerDay = 24;
			const int MillisecondsPerSecond = 1000;

			int Hours = Interval.Days * HoursPerDay + Interval.Hours;
			int Minutes = Interval.Minutes;
			int SubSeconds = (int)Math.Round( ((double)Interval.Milliseconds) / MillisecondsPerSecond );
			int Seconds = Interval.Seconds + SubSeconds;
			string Interval_str = "";
			if ( Hours < 10 )
			{
				Interval_str += "0";
			}
			Interval_str += Hours.ToString(CultureInfo.InvariantCulture) + SeparatorForHMS;
			if ( Minutes < 10 )
			{
				Interval_str += "0";
			}
			Interval_str += Minutes.ToString(CultureInfo.InvariantCulture) + SeparatorForHMS;
			if ( Seconds < 10 )
			{
				Interval_str += "0";
			}
			Interval_str += Seconds.ToString(CultureInfo.InvariantCulture);
			return Interval_str;
		}

		public static string IntervalToStringHHHMMSSLLLDec(
			TimeSpan Interval
			)
		{
			const char SeparatorForHMS = ':';
			const char SeparatorForSL = '.';
			const int HoursPerDay = 24;

			int Hours = Interval.Days * HoursPerDay + Interval.Hours;
			int Minutes = Interval.Minutes;
			int Seconds = Interval.Seconds;
			int Milliseconds = Interval.Milliseconds;
			string Interval_str = "";
			if (Hours < 10)
			{
				Interval_str += "0";
			}
			Interval_str += Hours.ToString(CultureInfo.InvariantCulture) + SeparatorForHMS;
			if (Minutes < 10)
			{
				Interval_str += "0";
			}
			Interval_str += Minutes.ToString(CultureInfo.InvariantCulture) + SeparatorForHMS;
			if (Seconds < 10)
			{
				Interval_str += "0";
			}
			Interval_str += Seconds.ToString(CultureInfo.InvariantCulture) + SeparatorForSL;
			if (Milliseconds < 100)
			{
				Interval_str += "0";
			}
			if (Milliseconds < 10)
			{
				Interval_str += "0";
			}
			Interval_str += Milliseconds.ToString(CultureInfo.InvariantCulture);
			return Interval_str;
		}

	}
}
