using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;
using MatchMaker.Data;
using MatchMaker.MatchMaking;
using MatchMaker.MatchMaking.MatchHistory;
using MatchMaker.MatchMaking.Ratings;
using MatchMaker.Web.Common;
using MatchMaker.Web.PlayerRatings;
using Nancy;

namespace MatchMaker.Web.DataVisualization
{
	public class DataVisualizationModule: GameModule
	{
		
		private readonly IDatabase database;
		private MatchupResult[] gameHistory;
		private RatingsHelper ratingsHelper;


		public DataVisualizationModule( IDatabase database ) : base( database )
		{
			this.database = database;
			ratingsHelper = new RatingsHelper(database, App.Settings);

			Get[Route.ViewDataVisualization] = o =>
				{
					var model = new ViewDataVisualizationViewModel();


					var linkProposer = new LinkMatchupProposer(database);

					model.LinkedPlayers = linkProposer.GetAllLinks(game.Id);


					return View[Route.ViewDataVisualization, model];
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

		private WinLossRecord GetWinLossRecord(Guid playerId)
		{
			return database.GetWinLossRecord(game.Id, playerId);
		}


		public double[] GetRatingHistory(Guid gameId, Guid playerId)
		{
			var ratings = database
				.GetPlayerRatings(gameId, playerId)
				.OrderBy(rating => rating.Timestamp);

			return ratings
				.Select(rating => ratingsHelper.CalculateAverageRating(ratings, rating.Timestamp))
				.Where(d => d != null)
				.Cast<double>()
				.Tail(App.Settings.MaxPlayerRatingsForTrendLine)
				.ToArray();
		}




	}
}


