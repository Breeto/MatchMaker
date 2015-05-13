using System;
using System.Collections.Generic;
using System.Linq;
using MatchMaker.Data;
using MatchMaker.MatchMaking;


namespace MatchMaker.Testing
{
	/// <summary>
	/// A simple, in-memory IDatabase implementation used for testing and development.
	/// </summary>
	public class TestDatabase : IDatabase
	{

		private readonly List<User> users = new List<User>();
		private readonly List<UserCredentials> userCredentials = new List<UserCredentials>();
		private readonly List<Game> games = new List<Game>();
		private readonly List<Map> maps = new List<Map>(); 
		private readonly List<GamePlayer> gamePlayers = new List<GamePlayer>();
		private readonly List<MatchupResult> matchupResults = new List<MatchupResult>();
		private readonly List<PlayerRating> playerRatings = new List<PlayerRating>(); 


		public TestDatabase( IPasswordEncryptor passwordEncryptor )
		{
			System.Console.WriteLine("Hi from TestDatabase");
			new TestDatabasePopulator(this, passwordEncryptor).Populate();
		}


		public User GetUserById( Guid userId )
		{
			return users.FirstOrDefault( user => user.Id == userId );
		}


		public User GetUserByName( string userName )
		{
			return users.FirstOrDefault( user => user.Name.Equals( userName, StringComparison.InvariantCultureIgnoreCase ) );
		}


		public UserCredentials GetUserCredentialsByUserId( Guid userId )
		{
			return userCredentials.FirstOrDefault( credentials => credentials.UserId == userId );
		}


		public UserCredentials GetUserCredentialsByAuthId( Guid authId )
		{
			return userCredentials.FirstOrDefault( user => user.AuthId == authId );
		}


		public UserCredentials GetUserCredentialsByEmailAddress( string emailAddress )
		{
			return userCredentials.FirstOrDefault( user => user.EmailAddress.Equals( emailAddress, StringComparison.InvariantCultureIgnoreCase ) );
		}


		public void AddUser( User user, UserCredentials credentials )
		{
			credentials.UserId = user.Id;

			this.users.Add( user );
			this.userCredentials.Add( credentials );
		}


		public void UpdateUserCredentials( UserCredentials userCredentials )
		{
			// Nothing to do.  In-memory object already updated.
		}


		public User[] GetUsers()
		{
			return users.ToArray();
		}


		public void AddGame( Game game )
		{
			this.games.Add(game);
		}


		public Game[] GetGames()
		{
			return this.games.ToArray();
		}


		public Game GetGameById( Guid gameId )
		{
			return games.FirstOrDefault( game => game.Id == gameId );
		}


		public Game GetGameByKey( string gameKey )
		{
			return games.FirstOrDefault( game => game.Key.ToLower() == gameKey.ToLower() );
		}


		public Game[] GetGamesPlayedByUser( Guid userId )
		{
			return gamePlayers
				.Where( player => player.UserId == userId )
				.Select( player => this.games.FirstOrDefault( game => game.Id == player.GameId ) )
				.ToArray();
		}


		public User[] GetPlayersForGame( Guid gameId )
		{
			return gamePlayers
				.Where( gamePlayer => gamePlayer.GameId == gameId )
				.Select( gamePlayer => this.users.FirstOrDefault( user => user.Id == gamePlayer.UserId ) )
				.ToArray();
		}


		public void SetGamesPlayedByUser( Guid userId, Guid[] gameIds )
		{
			gamePlayers.RemoveAll( player => player.UserId == userId );

			if ( null == gameIds )
			{
				return;
			}

			foreach ( var gameId in gameIds )
			{
				gamePlayers.Add(new GamePlayer(){GameId = gameId, UserId = userId});
			}
		}


		public void SaveMatchupResult( MatchupResult result )
		{
			matchupResults.Add( result );
		}


		public WinLossRecord GetWinLossRecord(Guid gameId, Guid playerId)
		{
			var gameHistory = GetMatchupResultsByGame(gameId);

			var winLossRecord = new WinLossRecord();

			foreach (var matchupResult in gameHistory)
			{
				if (matchupResult.Team1UserIds.Contains(playerId))
				{
					if (matchupResult.Winner == MatchupWinner.Team1)
					{
						winLossRecord.Wins++;
						continue;
					}

					if (matchupResult.Winner == MatchupWinner.Team2)
					{
						winLossRecord.Losses++;
						continue;
					}

					if (matchupResult.Winner == MatchupWinner.Tie)
					{
						winLossRecord.Ties++;
						continue;
					}
				}

				if (matchupResult.Team2UserIds.Contains(playerId))
				{
					if (matchupResult.Winner == MatchupWinner.Team1)
					{
						winLossRecord.Losses++;
						continue;
					}

					if (matchupResult.Winner == MatchupWinner.Team2)
					{
						winLossRecord.Wins++;
						continue;
					}

					if (matchupResult.Winner == MatchupWinner.Tie)
					{
						winLossRecord.Ties++;
						continue;
					}
				}
			}

			return winLossRecord;
		}


		public MatchupResult[] GetMatchupResultsByGame( Guid gameId )
		{
			return matchupResults.Where( result => result.GameId == gameId ).ToArray();
		}


		public void AddPlayerRating( PlayerRating playerRating )
		{
			playerRatings.Add( playerRating );
		}


		public PlayerRating[] GetPlayerRatings( Guid gameId, Guid playerId )
		{
			var ratings = playerRatings.Where( rating => rating.GameId == gameId && rating.PlayerId == playerId ).ToArray();

			return ratings;
		}


		public PlayerRating[] GetMostRecentPlayerRatings(Guid gameId, Guid playerId)
		{
			return playerRatings
				.Where(rating => rating.GameId == gameId)
				.Where(rating => rating.PlayerId == playerId)
				.OrderByDescending(rating => rating.Timestamp)
				.Distinct(new VoterComparer()).ToArray();
		}


		public PlayerRating GetMostRecentPlayerRating( Guid gameId, Guid playerId, Guid ratedByPlayerId )
		{
			return playerRatings
				.Where( rating => rating.GameId == gameId )
				.Where( rating => rating.PlayerId == playerId )
				.Where( rating => rating.RatedByPlayerId == ratedByPlayerId )
				.OrderByDescending(rating => rating.Timestamp)
				.FirstOrDefault();
		}


		public void SetActivePlayersForGame( Guid gameId, Guid[] userIds )
		{
			gamePlayers.RemoveAll( player => player.GameId == gameId );

			if ( null == userIds )
			{
				return;
			}

			foreach ( var userId in userIds )
			{
				gamePlayers.Add(new GamePlayer(){GameId = gameId, UserId = userId});
			}
		}

		public Map[] GetMaps(Guid gameId)
		{
			return maps.Where(map => map.GameId == gameId).ToArray();
		}

		public Map GetMapById(Guid mapId)
		{
			return maps.FirstOrDefault(map => map.Id == mapId);
		}

		public Map GetDefaultMap(Guid gameId)
		{
			var game = GetGameById(gameId);
			return GetMapById(game.DefaultMapId);
		}

		public MatchupResult[] GetMatchupResultsByMap(Guid gameId, Guid mapId)
		{
			return matchupResults.Where(result => (result.GameId == gameId && result.MapId == mapId)).ToArray();
		}
	}

}

