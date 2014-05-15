using System.Collections.Generic;
using MatchMaker.Data;
using MatchMaker.Web.Common;


namespace MatchMaker.Web.Matchup
{
	public class SelectPlayersViewModel
	{
		public SelectPlayersViewModel()
		{
			SelectableUsers = new List<Selectable<User>>();
		}
		public List<Selectable<User>> SelectableUsers
		{
			get;
			set;
		}
	}
}