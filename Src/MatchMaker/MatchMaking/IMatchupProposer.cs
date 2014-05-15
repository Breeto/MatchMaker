using MatchMaker.Data;


namespace MatchMaker.MatchMaking
{

	public interface IMatchupProposer
	{

		ProposedMatchup[] GetMatchups( Game game, User[] players );

	}
}

