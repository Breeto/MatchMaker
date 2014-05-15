using System.Collections.Generic;
using MatchMaker.Data;


namespace MatchMaker.Web.Default
{
	public class SelectGameViewModel
	{
		public SelectGameViewModel( Game[] games )
		{
			Games = new List<Game>(games);
		}

		public List<Game> Games
		{
			get;
			set;
		}
	}
}