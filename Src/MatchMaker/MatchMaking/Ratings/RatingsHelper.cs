using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatchMaker.Data;

namespace MatchMaker.MatchMaking.Ratings
{
	public class RatingsHelper
	{
		private readonly IDatabase database;
		private readonly Settings settings;
		private readonly VoterComparer voterComparer = new VoterComparer();

		public RatingsHelper(IDatabase database, Settings settings)
		{
			this.database = database;
			this.settings = settings;
		}

		public Dictionary<Guid, double> CalculateAverageRatings(Game game, IEnumerable<User> players)
		{
			//var cutoff = DateTime.Now - settings.PlayerRatingExpiryTime;
			var averageRatings = new Dictionary<Guid, double>();
			foreach (var player in players)
			{
				var average = CalculateAverageRating(database.GetMostRecentPlayerRatings(game.Id, player.Id), DateTime.Now);
			
				averageRatings.Add(player.Id, average.HasValue ? average.Value : 1.0);
				/*double averageRating = 0.0;
				int ratingsCounted = 0;
				var ratings = database.GetMostRecentPlayerRatings(game.Id, player.Id);
				foreach (var rating in ratings)
				{
					if (!rating.Rating.HasValue || rating.Timestamp < cutoff) continue;

					ratingsCounted++;
					averageRating += rating.Rating.Value;
				}

				if (ratingsCounted > 0)
				{
					averageRatings[player.Id] = averageRating / ratingsCounted;
				}*/
			}
			return averageRatings;
		}



		public double? CalculateAverageRating(IEnumerable<PlayerRating> ratings, DateTime pointInTime)
		{
			var cutoff = pointInTime - settings.PlayerRatingExpiryTime;

			return ratings
				.OrderByDescending(rating => rating.Timestamp)
				.Where(rating => rating.Timestamp <= pointInTime)
				.Where(rating => rating.Timestamp >= cutoff)
				.Distinct(voterComparer)
				.Where(rating => rating.Rating.HasValue)
				.Average(rating => rating.Rating);
		}

	}
}
