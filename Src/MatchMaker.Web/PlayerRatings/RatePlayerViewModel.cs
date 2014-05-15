using System;
using MatchMaker.Web.Common;


namespace MatchMaker.Web.PlayerRatings
{
	public class RatePlayerViewModel
	{

		public Guid PlayerId
		{
			get;
			set;
		}

		public string PlayerName
		{
			get;
			set;
		}

		public int CurrentRating
		{
			get;
			set;
		}

		public DateTime? LastUpdate
		{
			get;
			set;
		}

		public string TimeSinceLastUpdate
		{
			get
			{
				if ( LastUpdate.HasValue )
				{
					return ( DateTime.Now - LastUpdate.Value ).ToShortString() + " ago";
				}
				else
				{
					return "(never rated)";
				}
			}
		}

	}

}

