using System.Collections.Generic;


namespace MatchMaker.Data
{
	public class VoterComparer : IEqualityComparer<PlayerRating>
	{

		public bool Equals( PlayerRating x, PlayerRating y )
		{
			return x.RatedByPlayerId == y.RatedByPlayerId;
		}


		public int GetHashCode( PlayerRating obj )
		{
			return obj.RatedByPlayerId.GetHashCode();
		}

	}
}