using System;
using System.Collections.Generic;
using System.Linq;
using MatchMaker.Common;
using MatchMaker.Data;
using MatchMaker.Web.Common;


namespace MatchMaker.Web.Matchup
{

	public class MatchupResultViewModel
	{

		public MatchupResultViewModel( MatchupResult result, IdentityMap<User> userMap, Map gameMap )
		{
			Timestamp = result.Timestamp;
			HowRecent = ( DateTime.Now - Timestamp ).ToShortString() + " ago";
			Comment = result.Comment;
			Team1Score = result.Team1Score;
			Team2Score = result.Team2Score;
			Winner = result.Winner;
			this.Map = gameMap;

			Team1PlayerNames = result
				.Team1UserIds
				.Select( userMap.Lookup )
				.Select(user => user.Name)
				.ToList();

			Team2PlayerNames = result
				.Team2UserIds
				.Select( userMap.Lookup )
				.Select(user => user.Name)
				.ToList();
		}


		public DateTime Timestamp
		{
			get;
			set;
		}

		public string HowRecent
		{
			get;
			set;
		}

		public string Comment
		{
			get;
			set;
		}

		public List<string> Team1PlayerNames
		{
			get;
			set;
		}

		public List<string> Team2PlayerNames
		{
			get;
			set;
		}

		public MatchupWinner Winner
		{
			get;
			set;
		}

		public int? Team1Score
		{
			get;
			set;
		}

		public int? Team2Score
		{
			get;
			set;
		}

		public Map Map { get; set; }

	}

}