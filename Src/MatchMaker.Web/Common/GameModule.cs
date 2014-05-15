using MatchMaker.Data;
using Nancy;


namespace MatchMaker.Web.Common
{

	public abstract class GameModule : NancyModule
	{

		protected Game game;

		protected GameModule( IDatabase database ) : base("{gameKey}/")
		{

			Before.AddItemToStartOfPipeline( context =>
			{
				var gameKey = (string)context.Parameters.gameKey;

				game = database.GetGameByKey( gameKey );

				context.ViewBag[ViewBagKeys.CurrentGame] = game;

				return null;
			} );

		}

	}

}

