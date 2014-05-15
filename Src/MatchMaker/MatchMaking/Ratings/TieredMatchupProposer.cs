using System;
using System.Collections.Generic;
using MatchMaker.Data;
using MatchMaker.MatchMaking.MatchHistory;
using System.Linq;

namespace MatchMaker.MatchMaking.Ratings
{
	public class TieredMatchupProposer: IMatchupProposer
	{
		private const double STD_MULTIPLIER = .25;
		private readonly RatingsHelper ratingsHelper;

		public TieredMatchupProposer(IDatabase database, Settings settings)
		{
			ratingsHelper = new RatingsHelper(database, settings);
		}

		public ProposedMatchup[] GetMatchups(Game game, User[] players)
		{
			var teams = TeamGenerator.GenerateTeams(players);
			var averageRatings = ratingsHelper.CalculateAverageRatings(game, players);
			var playerList =
				new List<Player>(from id in averageRatings.Keys select new Player() {Id = id, Rating = averageRatings[id]});
			playerList.Sort();

			var standardDeviation = GetStandardDeviation(from player in playerList select player.Rating);
			
			var currentTier = 0;
			var lastRating = playerList[0].Rating;
			for(int index = 0; index<playerList.Count; index++)
			{
				var player = playerList[index];
				if (lastRating - player.Rating < standardDeviation * STD_MULTIPLIER)
				{
					player.Tier = currentTier;
					lastRating = player.Rating;
					continue;
				}
				currentTier++;
				player.Tier = currentTier;
				lastRating = player.Rating;
			}

			var totalTiers = currentTier+1;

			var playerDictionary = playerList.ToDictionary(player => player.Id, player => player.Tier);

			var matchups = new List<ProposedMatchup>();

			foreach (var teamPair in teams)
			{
				var team1 = teamPair.Item1;
				var team2 = teamPair.Item2;
				var team1Ratings = GetTeamRating(team1, playerDictionary, totalTiers) - GetStandardDeviation(from member in team1.Members select (double) playerDictionary[member.Id]);
				var team2Ratings = GetTeamRating(team2, playerDictionary, totalTiers) - GetStandardDeviation(from member in team2.Members select (double)playerDictionary[member.Id]);
				matchups.Add(new ProposedMatchup(game, team1, team2, team1Ratings / (team1Ratings + team2Ratings)));
			}

			return matchups.ToArray();
		}

		private double GetTeamRating(Team team, Dictionary<Guid, int> playerDictionary, int totalTiers)
		{
			return (from player in team.Members select GetTierValue(playerDictionary[player.Id], totalTiers)).Sum();
		}

		public double GetStandardDeviation(IEnumerable<double> numbers)
		{
			if (numbers == null || numbers.Count() <= 1)
				return 0;

			var mean = numbers.Average();
			var deviations = from n in numbers select n - mean;
			var squaresOfDeviations = from n in deviations select Math.Pow(n, 2);
			var sumOfSquares = squaresOfDeviations.Sum();
			return Math.Sqrt(sumOfSquares/(numbers.Count() - 1));
		}

		public int GetTierValue(int tierValue, int totalTiers)
		{
			return totalTiers - tierValue;
		}
		
	}

	public class Player:IComparable
	{
		public double Rating { get; set; }
		public Guid Id { get; set; }
		public int Tier { get; set; }
		public int CompareTo(object obj)
		{
			if (!(obj is Player))
				return -1;
			return this.Rating.CompareTo(((Player) obj).Rating) *-1;
		}
	}

}