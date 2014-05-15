using MatchMaker.MatchMaking;


namespace MatchMaker.Web.PlayerRatings
{
	public class PlayerAverageRating
	{

		public PlayerAverageRating()
		{
			WinLossRecord = new WinLossRecord();
		}
		
		public WinLossRecord WinLossRecord
		{
			get;
			set;
		}

		public string PlayerName
		{
			get;
			set;
		}

		public double? AverageRating
		{
			get;
			set;
		}

		public double[] RatingHistory
		{
			get;
			set;
		}



	}

}
