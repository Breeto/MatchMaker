namespace MatchMaker.Web.PlayerRatings
{

	public class WinLossRecord
	{

		public double WinPercentage
		{
			get
			{
				if ( Wins == 0 && Losses == 0 )
				{
					return 0;
				}

				if ( Losses == 0 )
				{
					return 100;
				}

				return (double)Wins / ( Wins + Losses + Ties ) * 100;
			}
		}

		public int Wins
		{
			get;
			set;
		}

		public int Ties
		{
			get;
			set;
		}

		public int Losses
		{
			get;
			set;
		}

	}

}

