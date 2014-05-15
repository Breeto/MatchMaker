using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MatchMaker.Data;
using MatchMaker.MatchMaking;

namespace MatchMaker.Web.Matchup
{
	public class SaveMatchupResultViewModel
	{
		public ProposedMatchup Matchup { get; set; }
		public List<Map> Maps { get; set; }
		public SaveMatchupResultViewModel(ProposedMatchup matchup, IEnumerable<Map> maps)
		{
			Matchup = matchup;
			Maps = maps.ToList();
			Maps.Sort((map1, map2) => System.String.Compare(map1.Name, map2.Name, System.StringComparison.Ordinal));
		}
	}
}