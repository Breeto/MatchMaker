using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MatchMaker.Data;
using MatchMaker.MatchMaking;
using MatchMaker.MatchMaking.MatchHistory;
using MatchMaker.MatchMaking.Ratings;
using MatchMaker.Web.Authentication;
using MatchMaker.Web.Common;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using System.Runtime.Serialization.Json;


namespace MatchMaker.Web.PlayerRatings
{

	public class PlayerRatingsModule : GameModule
	{

		private readonly IDatabase database;
		private MatchupResult[] gameHistory;
		private RatingsHelper ratingsHelper;


		public PlayerRatingsModule( IDatabase database ) : base( database )
		{
			this.database = database;
			ratingsHelper = new RatingsHelper(database, App.Settings);


			Get[Route.RatePlayers] = o =>
			{
				this.RequiresAuthentication();

				// TODO: If player doesn't currently play this game, set them as actively playing this game?

				var model = new RatePlayersViewModel
				{
					GameId = game.Id, 
					UserId = ( (UserIdentity)Context.CurrentUser ).UserId
				};

				var players = database.GetPlayersForGame( game.Id ).OrderBy(user => user.Name);

				foreach ( var player in players )
				{
					var rating = database.GetMostRecentPlayerRating( game.Id, player.Id, model.UserId );

					model.Players.Add(new RatePlayerViewModel()
					{
						CurrentRating = null != rating && rating.Rating.HasValue ? rating.Rating.Value : 0,
						LastUpdate = null != rating ? rating.Timestamp : (DateTime?)null,
						PlayerId = player.Id,
						PlayerName = player.Name,
					});
				}

				return View[Route.RatePlayers, model];
			};


			Get[Route.AddPlayerRating] = o =>
			{
				this.RequiresAuthentication();

				try
				{
					var playerRating = this.Bind<PlayerRating>();
					playerRating.GameId = game.Id;
					playerRating.Rating = playerRating.Rating == 0 ? null : playerRating.Rating;

					database.AddPlayerRating( playerRating );

					return HttpStatusCode.OK;
				}
				catch ( Exception exception )
				{
					var response = new StringResponse(exception.Message);
					response.StatusCode = HttpStatusCode.InternalServerError;
					return response;
				}
			};


			Get[Route.ViewPlayerRatings] = o =>
			{
				var model = new ViewPlayerRatingsViewModel();
				var gameId = game.Id;

				var players = database.GetPlayersForGame( gameId );

				foreach ( var player in players )
				{
					var ratings = database.GetPlayerRatings( gameId, player.Id );

					var average = ratingsHelper.CalculateAverageRating( ratings, DateTime.Now );

					var winLossRecord = GetWinLossRecord( player.Id );

					model.PlayerAverageRatings.Add( new PlayerAverageRating()
					{
						PlayerName = player.Name,
						AverageRating = average,
						RatingHistory = GetRatingHistory(gameId, player.Id),
						WinLossRecord = winLossRecord,
					});
				}

				var linkProposer = new LinkMatchupProposer(database);

				model.LinkedPlayers = linkProposer.GetAllLinks(gameId);


				return View[Route.ViewPlayerRatings, model];
			};

			Get["getlinkedplayers"] = o =>
				{
					var linkProposer = new LinkMatchupProposer(database);
					var stream = new MemoryStream();
					var serializer = new DataContractJsonSerializer(typeof (List<LinkedPlayer>));
					serializer.WriteObject(stream, linkProposer.GetAllLinks(game.Id));
					stream.Position = 0;
					var reader = new StreamReader(stream);

					var json = reader.ReadToEnd();

					var response = (Response)json;

					response.ContentType = "application/json";

					return response;


				};


		}


		private WinLossRecord GetWinLossRecord( Guid playerId )
		{
			return database.GetWinLossRecord(game.Id, playerId);
		}




		public double[] GetRatingHistory( Guid gameId, Guid playerId )
		{
			var ratings = database
				.GetPlayerRatings( gameId, playerId )
				.OrderBy( rating => rating.Timestamp );

			return ratings
				.Select( rating => ratingsHelper.CalculateAverageRating( ratings, rating.Timestamp ) )
				.Where( d => d != null )
				.Cast<double>()
				.Tail( App.Settings.MaxPlayerRatingsForTrendLine )
				.ToArray();
		}

	}
}


