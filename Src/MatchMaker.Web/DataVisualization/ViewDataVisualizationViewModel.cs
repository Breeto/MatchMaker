using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MatchMaker.MatchMaking.MatchHistory;


namespace MatchMaker.Web.DataVisualization
{
	public class ViewDataVisualizationViewModel
	{

		public ViewDataVisualizationViewModel()
		{
			LinkedPlayers = new List<LinkedPlayer>();
		}
		

		public List<LinkedPlayer> LinkedPlayers { get; set; }
	}
}