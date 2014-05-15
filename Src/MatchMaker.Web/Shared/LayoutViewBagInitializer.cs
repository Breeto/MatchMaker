using System;
using MatchMaker.Data;
using MatchMaker.Web.Common;
using Nancy;
using Nancy.Bootstrapper;


namespace MatchMaker.Web.Shared
{

	/// <summary>
	/// Populates the ViewBag with objects needed by the shared layout.
	/// </summary>
	public class LayoutViewBagInitializer
	{

		private static IDatabase database;


		public static void Enable( IPipelines pipelines, IDatabase database )
		{
			LayoutViewBagInitializer.database = database;

			pipelines.AfterRequest.AddItemToEndOfPipeline(Handler);
		}


		private static void Handler( NancyContext context )
		{
			var sessionHelper = new SessionHelper( context );

			if ( null != context.CurrentUser )
			{
				var user = database.GetUserByName( context.CurrentUser.UserName );

				context.ViewBag[ViewBagKeys.CurrentUser] = user;
			}

			var currentGameId = sessionHelper.CurrentGameId;
			if ( null != currentGameId )
			{
				var currentGame = database.GetGameById( currentGameId.Value );
				context.ViewBag[ViewBagKeys.CurrentGame] = currentGame;
			}
		}

	}

}