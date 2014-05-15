using System;
using System.Collections.Generic;


namespace MatchMaker.Web.Matchup
{

	public class GetProposedMatchupsRequest
	{

		public List<Guid> PlayerIds
		{
			get;
			set;
		}

		public string GameKey
		{
			get;
			set;
		}

	}

}
