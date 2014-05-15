using System;


namespace MatchMaker.Web.Matchup
{
	public class SelectPlayersRequest
	{
		public Guid[] UserIds
		{
			get;
			set;
		}
	}
}