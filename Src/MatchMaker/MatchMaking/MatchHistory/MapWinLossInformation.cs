using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatchMaker.Data;

namespace MatchMaker.MatchMaking.MatchHistory
{
	public class MapWinLossInformation
	{
		public Map Map { get; set; }
		public double Wins { get; set; }
		public double TimesPlayed { get; set; }
		public double Percentage { get; set; }
	}
}
