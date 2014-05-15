using System;


namespace MatchMaker.Web.UserProfile
{
	public class SelectGamesRequest
	{
		public Guid[] GameIds
		{
			get;
			set;
		}
	}
}