using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatchMaker.Data;

namespace MatchMaker.MatchMaking.MatchHistory
{
	public class LinkedPlayer
	{
		public string Name { get; set; }
		public Guid ID { get; set; }
		public List<double> Matrix { get; set; }
		public List<double> GameMatrix { get; set; }
		public List<int> RecentWinsLosses { get; set; }
		public List<MapWinLossInformation> MapWinPercentages { get; set; } 
	}
}
