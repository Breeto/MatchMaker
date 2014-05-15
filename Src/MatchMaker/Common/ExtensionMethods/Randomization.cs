using System;


namespace MatchMaker.Common.ExtensionMethods
{

	public static class Randomization
	{

		private static Random rng = new Random();


		public static int Next( this Range<int> range )
		{
			return range.Min + rng.Next( range.Max - range.Min + 1 );
		}

	
		public static double Next( this Range<double> range )
		{
			return range.Min + ( rng.NextDouble() * ( range.Max - range.Min ) );
		}


		public static TimeSpan Next(this Range<TimeSpan> timeSpan)
		{
			var timeSpanRange = timeSpan.Max - timeSpan.Min;
			return timeSpan.Min + TimeSpan.FromSeconds(timeSpanRange.TotalSeconds * rng.NextDouble());
		}


		public static double Next( this double number )
		{
			return rng.NextDouble() * number;
		}

	
		public static TimeSpan Next( this TimeSpan timeSpan )
		{
			return TimeSpan.FromSeconds( timeSpan.TotalSeconds.Next() );
		}


		/// <summary>
		/// Returns true if a randomly generated number between 0.0 and 1.0 is less than or equal to this number (the probability).
		/// </summary>
		public static bool NextBool( this double probability )
		{
			return rng.NextDouble() <= probability;
		}

	}

}
