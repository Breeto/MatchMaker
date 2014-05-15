namespace MatchMaker.Web
{

	public class Route
	{

		public const string Home = "/";

		public const string SelectGame = "/selectgame";

		public const string Login = "/login";

		public const string Logout = "/logout";

		public const string UserProfile = "/userprofile";

		public const string ChangePassword = "/changepassword";

		public const string Register = "/register";

		public const string SelectGames = "/selectgames";

		public const string FindTeams = "findteams";
		public static string FindTeamsFor(string gameKey)
		{
			return GameUrl( gameKey, FindTeams );
		}

		public const string SelectPlayers = "selectplayers";

		private static string GameUrl( string gameKey, string route )
		{
			return string.Format( "/{0}/{1}", gameKey, route );
		}

		public const string GetProposedMatchups = "getproposedmatchups";

		public const string SetMatchupProposer = "setmatchupproposer";
		
		public const string PresentMatchupForSaving = "presentmatchupforsaving";
		
		public const string SaveMatchupResult = "savematchupresult";
		
		public const string ViewResults = "viewresults";
		public static string ViewResultsFor( string gameKey )
		{
			return GameUrl( gameKey, ViewResults );
		}

		public const string RatePlayers = "rateplayers";
		public static string RatePlayersFor( string gameKey )
		{
			return GameUrl( gameKey, RatePlayers );
		}

		public const string AddPlayerRating = "addplayerrating";
		
		public const string ViewPlayerRatings = "viewplayerratings";
		
		public static string ViewPlayerRatingsFor( string gameKey )
		{
			return GameUrl( gameKey, ViewPlayerRatings );
		}

		public const string ViewDataVisualization = "viewdatavisualization";
		public static string ViewDataVisualizationFor(string gameKey)
		{
			return GameUrl(gameKey, ViewDataVisualization);
		}

	}

}


