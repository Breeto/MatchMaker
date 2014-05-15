using System;
using MatchMaker.Common;


namespace MatchMaker
{

	public class Settings
	{

		public Settings()
		{
			PlayerRatingExpiryTime = TimeSpan.FromDays( 30 );
			MinimumPasswordLength = 4;
			UserNameSizeRange = new Range<int>( 3, 20 );
			MaxPlayerRatingsForTrendLine = 100;
			MaxMatchupProposalsToPresent = 10;
			MinimumLinks = 10;
			MaximumLinks = 20;
		}

		public TimeSpan PlayerRatingExpiryTime
		{
			get;
			set;
		}

		public int MinimumPasswordLength
		{
			get;
			set;
		}

		public Range<int> UserNameSizeRange
		{
			get;
			set;
		}

		public int MaxPlayerRatingsForTrendLine
		{
			get;
			set;
		}

		public int MaxMatchupProposalsToPresent
		{
			get;
			set;
		}

		public int MinimumLinks { get; set; }

		public int MaximumLinks { get; set; }



	}

}