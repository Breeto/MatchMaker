using System;
using System.Collections.Generic;
using MatchMaker.Data;
using MatchMaker.MatchMaking.MatchHistory;


namespace MatchMaker.MatchMaking.Ratings
{
	public class RatingsMatchupProposer : IMatchupProposer
	{
		private readonly RatingsHelper ratingsHelper;

		public RatingsMatchupProposer(IDatabase database, Settings settings)
		{
			ratingsHelper = new RatingsHelper(database, settings);
		}


		public ProposedMatchup[] GetMatchups(Game game, User[] players)
		{
			var averageRatings = ratingsHelper.CalculateAverageRatings(game, players);

			var teams = TeamGenerator.GenerateTeams(players);
			var matchups = new List<ProposedMatchup>();

			foreach (var teamPair in teams)
			{
			var team1 = teamPair.Item1;
				var team2 = teamPair.Item2;
				var team1Ratings = GetTeamRating(team1, averageRatings);
				var team2Ratings = GetTeamRating(team2, averageRatings);
				matchups.Add(new ProposedMatchup(game, team1, team2, team1Ratings / (team1Ratings + team2Ratings)));
			}

			return matchups.ToArray();
		}
		
		private double GetTeamRating(Team team, Dictionary<Guid, double> averageRatings)
		{
			double rating = 0.0;
			foreach (var player in team.Members)
			{
				double playerRating;
				if(averageRatings.TryGetValue(player.Id, out playerRating))
				{
					rating += playerRating;
				}
				else
				{
					rating += 1; // Assume rating of 1 when no ratings exist.
				}
			}
			return rating;
		}
	}
}
