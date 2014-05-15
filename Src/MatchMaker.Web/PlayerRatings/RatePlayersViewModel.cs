using System;
using System.Collections.Generic;


namespace MatchMaker.Web.PlayerRatings
{

	public class RatePlayersViewModel
	{

		public Guid GameId
		{
			get;
			set;
		}

		public Guid UserId
		{
			get;
			set;
		}

		public RatePlayersViewModel()
		{
			Players = new List<RatePlayerViewModel>();
		}

		public List<RatePlayerViewModel> Players
		{
			get;
			set;
		}
	 
	}
}