using System.Collections.Generic;
using MatchMaker.MatchMaking;


namespace MatchMaker.Web.Matchup
{

	public class GetProposedMatchupsResponse
	{

		public string ErrorMessage
		{
			get;
			set;
		}

		public List<ProposedMatchup> Type
		{
			get;
			set;
		}

		public ProposedMatchup[] ProposedMatchups
		{
			get;
			set;
		}

	}

}

