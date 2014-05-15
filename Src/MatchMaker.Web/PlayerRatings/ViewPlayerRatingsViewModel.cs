using System;
using System.Collections.Generic;
using MatchMaker.MatchMaking.MatchHistory;


namespace MatchMaker.Web.PlayerRatings
{

	public class ViewPlayerRatingsViewModel
	{

		public ViewPlayerRatingsViewModel()
		{
			PlayerAverageRatings = new List<PlayerAverageRating>();
			LinkedPlayers = new List<LinkedPlayer>();
		}
		
		public List<PlayerAverageRating> PlayerAverageRatings
		{
			get;
			set;
		}

		public List<LinkedPlayer> LinkedPlayers { get; set; }

	}

}


