using System;


namespace MatchMaker.Web.UserProfile
{
	public class SelectableGame
	{
		public string GameName
		{
			get;
			set;
		}

		public Guid GameId
		{
			get;
			set;
		}

		public bool IsSelected
		{
			get;
			set;
		}
	}
}