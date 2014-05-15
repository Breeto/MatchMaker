using System;
using System.Collections.Generic;
using System.Linq;
using MatchMaker.Data;


namespace MatchMaker.MatchMaking.MatchHistory
{
	public class LimitedLinkMatchupProposer : IMatchupProposer
	{
		private readonly IDatabase database;
		private int minLinks;
		private int maxLinks;

		public LimitedLinkMatchupProposer( IDatabase database, Settings settings )
		{
			this.database = database;
			minLinks = settings.MinimumLinks;
			maxLinks = settings.MaximumLinks;
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

			foreach(var player in result.Keys)
			{
				foreach(var otherPlayer in result.Keys)
				{
					if (player.Equals(otherPlayer) || !result[player].Keys.Contains(otherPlayer))
						continue;
					if(result[player][otherPlayer].GetGameCount()>maxLinks)
					{
						result[player][otherPlayer].RemoveOldGames(maxLinks);
					}
					if (result[player][otherPlayer].GetGameCount() < minLinks)
					{
						result[player][otherPlayer].BackfillOldGames(minLinks);
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
	}
}