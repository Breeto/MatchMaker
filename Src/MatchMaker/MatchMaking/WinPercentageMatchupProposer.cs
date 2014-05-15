using System;
using System.Collections;
using System.Collections.Generic;
using MatchMaker.Data;
using System.Linq;
using MatchMaker.MatchMaking.MatchHistory;


namespace MatchMaker.MatchMaking
{
	public class WinPercentageMatchupProposer : IMatchupProposer
	{
		private readonly IDatabase database;
		private readonly Dictionary<Guid, MatchupResult[]> gameHistories = new Dictionary<Guid, MatchupResult[]>();

		public WinPercentageMatchupProposer(IDatabase database)
		{
			this.database = database;
		}


		public ProposedMatchup[] GetMatchups(Game game, User[] players)
		{
			var winLossRecords = new Dictionary<Guid, WinLossRecord>();
			foreach (var player in players)
			{
				winLossRecords[player.Id] = database.GetWinLossRecord(game.Id, player.Id);
			}

			var teams = TeamGenerator.GenerateTeams(players);
			var matchups = new List<ProposedMatchup>();
			foreach (var teamPair in teams)
			{
				var team1 = teamPair.Item1;
				var team2 = teamPair.Item2;
				var team1WinPercentage = GetTeamWinPercentage(team1, winLossRecords);
				var team2WinPercentage = GetTeamWinPercentage(team2, winLossRecords);
				matchups.Add(new ProposedMatchup(game, team1, team2, team1WinPercentage / (team1WinPercentage + team2WinPercentage)));
			}

			return matchups.ToArray();
		}


		private double GetTeamWinPercentage(Team team, Dictionary<Guid, WinLossRecord> winLossRecords)
		{
			// Option 1: Total wins/losses, giving players who have played more a higher weight in the rating
//			var record = new WinLossRecord();
//			foreach (var player in team.Members)
//			{
//				WinLossRecord winLossRecord;
//				if (winLossRecords.TryGetValue(player.Id, out winLossRecord))
//				{
//					record.Wins += winLossRecord.Wins;
//					record.Losses += winLossRecord.Losses;
//					record.Ties += winLossRecord.Ties;
//				}
//			}
//			return record.WinPercentage;


			// Option 2: Average win percentage, equally weighted
			double winPercentage = 0.0;
			foreach (var player in team.Members)
			{
				WinLossRecord winLossRecord;
				if (winLossRecords.TryGetValue(player.Id, out winLossRecord))
				{
					winPercentage += winLossRecord.WinPercentage;
				}
				else
				{
					winPercentage += 0.5;
				}
			}

			return winPercentage/team.Members.Count;
		}
	}
}
