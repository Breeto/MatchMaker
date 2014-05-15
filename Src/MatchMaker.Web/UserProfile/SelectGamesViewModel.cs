using System.Collections.Generic;
using MatchMaker.Data;
using MatchMaker.Web.Common;


namespace MatchMaker.Web.UserProfile
{
	public class SelectGamesViewModel
	{
		public SelectGamesViewModel()
		{
			SelectableGames = new List<Selectable<Game>>();
		}
		public List<Selectable<Game>> SelectableGames
		{
			get;
			set;
		}
	}
}