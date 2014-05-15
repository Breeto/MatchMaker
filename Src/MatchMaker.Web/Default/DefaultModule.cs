using System;
using MatchMaker.Data;
using MatchMaker.Web.Authentication;
using Nancy;


namespace MatchMaker.Web.Default
{

	public class DefaultModule : NancyModule
	{

		public DefaultModule( IDatabase database )
		{

			Get[Route.Home] = o =>
			{
				return Response.AsRedirect( Route.SelectGame );
			};


			Get[Route.SelectGame] = o =>
			{
				var model = new SelectGameViewModel(database.GetGames());

				return View[Route.SelectGame, model];
			};

		}
	 
	}

}

