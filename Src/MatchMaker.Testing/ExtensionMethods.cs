using MatchMaker.Data;


namespace MatchMaker.Testing
{

	public static class ExtensionMethods
	{
		
 		public static bool IsEven( this int i )
 		{
 			return i % 2 == 0;
 		}


		public static MatchupWinner GetWinner( this int team1Score, int team2Score )
		{
			if ( team1Score > team2Score )
			{
				return MatchupWinner.Team1;
			}

			if ( team2Score > team1Score )
			{
				return MatchupWinner.Team2;
			}

			return MatchupWinner.Tie;
		}

	}

}