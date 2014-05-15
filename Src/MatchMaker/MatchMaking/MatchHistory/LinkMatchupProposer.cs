using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using MatchMaker.Data;


namespace MatchMaker.MatchMaking.MatchHistory
{
	public class LinkMatchupProposer : IMatchupProposer
	{
		private readonly IDatabase database;
		private readonly Random random = new Random();

		public LinkMatchupProposer( IDatabase database )
		{
			this.database = database;
		}

		#region IMatchupProposer Members

		public ProposedMatchup[] GetMatchups( Game game, User[] players )
		{
			Dictionary<Guid, Dictionary<Guid, Link>> links = GetLinks( database.GetMatchupResultsByGame( game.Id ), players );
			List<Tuple<Team, Team>> teams = TeamGenerator.GenerateTeams( players );

			var matchups = new List<ProposedMatchup>();

			foreach ( var teamPair in teams )
			{
				var team1Probability = CalculateTeamLinkAverage(links, teamPair.Item1);
				var team2Probability = CalculateTeamLinkAverage(links, teamPair.Item2); 
				matchups.Add( new ProposedMatchup( game, teamPair.Item1, teamPair.Item2, team1Probability/(team1Probability + team2Probability) ) );
			}

			return matchups.ToArray();
		}

		#endregion

		private List<int> GetRecentWinLosses(MatchupResult[] results, User user)
		{
			var winLosses = new List<int>();
			var streak = 0;
			foreach(var result in results)
			{
				var onTeam1 = result.Team1UserIds.Contains(user.Id);
				var onTeam2 = result.Team2UserIds.Contains(user.Id);
				if(!onTeam1 && !onTeam2)
				{
					continue;
				}
				var won = result.Winner == MatchupWinner.Team1 && onTeam1 || result.Winner == MatchupWinner.Team2 && onTeam2;
				if(streak>=0)
				{
					if(won)
					{
						streak++;
					}
					else
					{
						streak = -1;
					}
				}
				else
				{
					if(!won)
					{
						streak --;
					}
					else
					{
						streak = 1;
					}
				}

				winLosses.Add(streak);

			}

			if(winLosses.Count<=20)
				return winLosses;
			winLosses.RemoveRange(0,winLosses.Count-20);
			return winLosses;
		} 

		public List<LinkedPlayer> GetAllLinks(Guid gameid)
		{
			var game = database.GetGameById(gameid);
			var users = (from user in database.GetPlayersForGame(gameid) orderby user.Id select user).ToList();
			var games = database.GetMatchupResultsByGame(gameid);
			games = (from g in games orderby g.Timestamp select g).ToArray();
			users.RemoveAll(user => !GetRecentWinLosses(games, user).Any());
			var links = GetLinks(games, users.ToArray());
			var nameDictionary = users.ToDictionary(user => user.Id, user => user.Name);
			var players = new List<LinkedPlayer>();
			
			var gameMaps = database.GetMaps(gameid).ToList();
			gameMaps.RemoveAll(m => m.Id == game.DefaultMapId);

			foreach(var key in links.Keys)
			{
				var player = new LinkedPlayer();
				player.ID = key;
				player.Name = nameDictionary[key];
				player.Matrix = new List<double>();
				player.GameMatrix = new List<double>();
				var playerLinks = links[key];
				foreach(var linkKey in users)
				{
					if(!playerLinks.ContainsKey(linkKey.Id))
					{
						player.Matrix.Add(0);
						player.GameMatrix.Add(0);
						continue;
					}
					player.Matrix.Add(playerLinks[linkKey.Id].GetWinLoss());
					player.GameMatrix.Add(playerLinks[linkKey.Id].GetGameCount());
				}
				player.RecentWinsLosses = GetRecentWinLosses(games,
				                                             (from user in users where user.Id == key select user).First());

				player.MapWinPercentages = GetMapWinPercentages(games,
				                                                (from user in users where user.Id == key select user).First(), gameMaps);

				players.Add(player);
			}
			return players;
		}

		private Dictionary<Guid, Dictionary<Guid, Link>> GetLinks( MatchupResult[] PreviousGames, User[] players )
		{
			var result = new Dictionary<Guid, Dictionary<Guid, Link>>();
			var playerDictionary = new Dictionary<Guid, User>();
			foreach ( User user in players )
			{
				result.Add( user.Id, new Dictionary<Guid, Link>() );
				playerDictionary.Add( user.Id, user );
			}

			foreach ( MatchupResult matchup in PreviousGames )
			{
				foreach ( Guid id in matchup.Team1UserIds )
				{
					if ( !playerDictionary.Keys.Contains( id ) )
					{
						continue;
					}
					foreach ( Guid otherId in matchup.Team1UserIds )
					{
						if ( !playerDictionary.Keys.Contains( otherId ) || id == otherId || result[id] == null )
						{
							continue;
						}
						if ( !result[id].ContainsKey( otherId ) )
						{
							result[id].Add( otherId, new Link( playerDictionary[otherId] ) );
						}
						result[id][otherId].AddGame( matchup );
					}
				}
				foreach ( Guid id in matchup.Team2UserIds )
				{
					if ( !playerDictionary.Keys.Contains( id ) )
					{
						continue;
					}
					foreach ( Guid otherId in matchup.Team2UserIds )
					{
						if ( !playerDictionary.Keys.Contains( otherId ) || id == otherId || result[id] == null )
						{
							continue;
						}
						if ( !result[id].ContainsKey( otherId ) )
						{
							result[id].Add( otherId, new Link( playerDictionary[otherId] ) );
						}
						result[id][otherId].AddGame( matchup );
					}
				}
			}

			return result;
		}


		private double CalculateTeamLinkAverage( Dictionary<Guid, Dictionary<Guid, Link>> links, Team team )
		{
			var linkValues = new List<double>();

			foreach ( User player in team.Members )
			{
				foreach ( User otherPlayer in team.Members )
				{
					if ( player.Id == otherPlayer.Id )
					{
						continue;
					}
					if ( !links[player.Id].Keys.Contains( otherPlayer.Id ) )
					{
						linkValues.Add( .5 );
						continue;
					}
					linkValues.Add( links[player.Id][otherPlayer.Id].GetWinLoss() );
				}
			}

			if ( linkValues.Count == 0 )
			{
				return .5;
			}

			return Enumerable.Average( linkValues );
		}

		private List<MapWinLossInformation> GetMapWinPercentages(MatchupResult[] results, User user, List<Map> maps)
		{
			var countDictonary = maps.ToDictionary(map => map.Id, map => new MapWinLossInformation(){Map = map});
			foreach (var result in results)
			{
				var onTeam1 = result.Team1UserIds.Contains(user.Id);
				var onTeam2 = result.Team2UserIds.Contains(user.Id);
				if (!onTeam1 && !onTeam2 || !countDictonary.Keys.Contains(result.MapId))
				{
					continue;
				}
				var won = result.Winner == MatchupWinner.Team1 && onTeam1 || result.Winner == MatchupWinner.Team2 && onTeam2;
				countDictonary[result.MapId].TimesPlayed++;
				if (won)
				{
					countDictonary[result.MapId].Wins++;
				}

			}

			foreach (var key in countDictonary.Keys)
			{
				countDictonary[key].Percentage = countDictonary[key].TimesPlayed == 0 ? .5 :  countDictonary[key].Wins/countDictonary[key].TimesPlayed;
			}

			return (from kvpair in countDictonary select kvpair.Value).ToList();
		}
	}
}