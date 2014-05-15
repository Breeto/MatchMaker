using System.Collections.Generic;
using System.Linq;
using MatchMaker.MatchMaking;


namespace MatchMaker.Web.Matchup
{

	public class SetMatchupProposerViewModel
	{

		public SetMatchupProposerViewModel( IEnumerable<IMatchupProposer> proposers, IMatchupProposer activeMatchupProposer )
		{
			Algorithms = proposers
				.Select( proposer => new Algorithm( proposer, proposer.GetType() == activeMatchupProposer.GetType() ) )
				.ToArray();
		}


		public Algorithm[] Algorithms
		{
			get;
			set;
		}
			
	}

}