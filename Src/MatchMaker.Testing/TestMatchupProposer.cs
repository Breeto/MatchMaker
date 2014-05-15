using System;
using MatchMaker.Common;
using MatchMaker.Common.ExtensionMethods;
using MatchMaker.Data;
using MatchMaker.MatchMaking;


namespace MatchMaker.Testing
{

	public class TestMatchupProposer : IMatchupProposer
	{

		private Range<double> winRatioRange = new Range<double>( 0.3, 0.7 );


		public ProposedMatchup[] GetMatchups( Game game, User[] users )
		{
			if ( users.Length < 2 )
			{
				throw new ArgumentException( "At least two players are required" );
			}

			var team1 = new Team();
			var team2 = new Team();

			for ( int i = 0; i < users.Length; i++ )
			{
				if ( i % 2 == 0 )
				{
					team1.Members.Add( users[i] );
				}
				else
				{
					team2.Members.Add( users[i] );
				}
			}

			return new ProposedMatchup[]
			{
				new ProposedMatchup( game, team1, team2, winRatioRange.Next() ),
				new ProposedMatchup( game, team1, team2, winRatioRange.Next() ),
				new ProposedMatchup( game, team1, team2, winRatioRange.Next() ),
			};
		}

	}

}

