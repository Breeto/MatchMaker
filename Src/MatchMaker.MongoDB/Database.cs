using System;
using System.ComponentModel;
using System.Linq;
using MatchMaker.Data;
using MatchMaker.MatchMaking;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;


namespace MatchMaker.MongoDB
{

	public class Database : IDatabase
	{

		public const string DatabaseName = "MatchMaker";
		public class Collections
		{
			public const string Games = "Games";
			public const string GamePlayers = "GamePlayers";
			public const string MatchupResults = "MatchupResults";
			public const string PlayerRatings = "PlayerRatings";
			public const string Users = "Users";
			public const string UserCredentials = "UserCredentials";
			public const string Maps = "Maps";
		}

	
		private static readonly MongoServer server;
		private static readonly MongoDatabase database;
		private static readonly VoterComparer voterComparer;

		static Database()
		{
			server = MongoServer.Create();
			database = server.GetDatabase( DatabaseName );

//			BsonSerializer.RegisterIdGenerator( typeof ( Guid ), CombGuidGenerator.Instance );
			
			FixTimezoneIssue();
		}


		/// <summary>
		/// We need to tell Mongo to use DateTimeKind.Local for all DateTime properties on our model.
		/// </summary>
		private static void FixTimezoneIssue()
		{
			BsonClassMap.RegisterClassMap<PlayerRating>( cm =>
			{
				cm.AutoMap();
				cm.GetMemberMap( rating => rating.Timestamp )
					.SetSerializationOptions( new DateTimeSerializationOptions {Kind = DateTimeKind.Local} );
			} );

			BsonClassMap.RegisterClassMap<MatchupResult>( cm =>
			{
				cm.AutoMap();
				cm.GetMemberMap( result => result.Timestamp )
					.SetSerializationOptions( new DateTimeSerializationOptions {Kind = DateTimeKind.Local} );
			} );
		}


		public UserCredentials GetUserCredentialsByUserId( Guid userId )
		{
//			CreateSampleGame();

			return database
				.GetCollection<UserCredentials>( Collections.UserCredentials )
				.AsQueryable<UserCredentials>()
				.FirstOrDefault( credentials => credentials.UserId == userId );
		}


		private void CreateSampleGame()
		{
			var game = new Game()
			{
				Key = "l4d2",
				Name = "Left 4 Dead 2",
			};

			AddGame(game);
		}


		public UserCredentials GetUserCredentialsByAuthId( Guid authId )
		{
			return database
				.GetCollection<UserCredentials>( Collections.UserCredentials )
				.AsQueryable<UserCredentials>()
				.FirstOrDefault( credentials => credentials.AuthId == authId );
		}


		public UserCredentials GetUserCredentialsByEmailAddress( string emailAddress )
		{
			return database
				.GetCollection<UserCredentials>( Collections.UserCredentials )
				.AsQueryable<UserCredentials>()
				.FirstOrDefault( credentials => credentials.EmailAddress.ToLower() == emailAddress );
		}


		public User GetUserById( Guid userId )
		{
			return database
				.GetCollection<User>( Collections.Users )
				.FindOneById( userId );
		}


		public User GetUserByName( string userName )
		{
			return database
				.GetCollection<User>( Collections.Users )
				.AsQueryable<User>()
				.FirstOrDefault( user => user.Name.ToLower() == userName.ToLower() );
		}


		public void AddUser( User user, UserCredentials credentials )
		{
			var users = database.GetCollection<User>( Collections.Users );
			users.Insert( user );

			credentials.UserId = user.Id;

			var userCredentials = database.GetCollection<UserCredentials>( Collections.UserCredentials );
			userCredentials.Insert( credentials );
		}


		public void UpdateUserCredentials( UserCredentials userCredentials )
		{
			database
				.GetCollection<UserCredentials>( Collections.UserCredentials )
				.Save( userCredentials );
		}


		public User[] GetUsers()
		{
			return database
				.GetCollection<User>( Collections.Users )
				.FindAll()
				.ToArray();
		}


		public void AddGame( Game game )
		{
			database
				.GetCollection<Game>( Collections.Games )
				.Insert( game );
		}


		// TODO: Extension methods for...  database.GetCollection<Game>( constant )
		public Game[] GetGames()
		{
			return database
				.GetCollection<Game>( Collections.Games )
				.FindAll()
				.ToArray();
		}


		public Game GetGameById( Guid gameId )
		{
			return database
				.GetCollection<Game>( Collections.Games )
				.FindOneById( gameId );
		}


		public Game GetGameByKey( string gameKey )
		{
			return database
				.GetCollection<Game>( Collections.Games )
				.AsQueryable<Game>()
				.FirstOrDefault( game => game.Key == gameKey );
		}


		public Game[] GetGamesPlayedByUser( Guid userId )
		{
			return database
				.GetCollection<GamePlayer>( Collections.GamePlayers )
				.AsQueryable<GamePlayer>()
				.Where( gamePlayer => gamePlayer.UserId == userId )
				.Select( gamePlayer => GetGameById( gamePlayer.GameId ) )
				.ToArray();
		}


		public User[] GetPlayersForGame( Guid gameId )
		{
			return database
				.GetCollection<GamePlayer>( Collections.GamePlayers )
				.AsQueryable<GamePlayer>()
				.Where( gamePlayer => gamePlayer.GameId == gameId )
				.Select( gamePlayer => GetUserById( gamePlayer.UserId ) )
				.ToArray();
		}


		public void SetGamesPlayedByUser( Guid userId, Guid[] gameIds )
		{
			var collection = database.GetCollection<GamePlayer>( Collections.GamePlayers );
			
			var query = Query<GamePlayer>.EQ( player => player.UserId, userId );

			collection.Remove( query );

			if ( null == gameIds || gameIds.Length == 0 )
			{
				return;
			}

			foreach ( var gameId in gameIds )
			{
				collection.Insert( new GamePlayer() {GameId = gameId, UserId = userId} );
			}
		}


		public void SaveMatchupResult( MatchupResult result )
		{
			database
				.GetCollection<MatchupResult>( Collections.MatchupResults )
				.Insert( result );
		}


		public WinLossRecord GetWinLossRecord(Guid gameId, Guid playerId)
		{
			var gameMatches = Query.EQ("GameId", gameId);
			var team1Win = Query.EQ("Winner", MatchupWinner.Team1);
			var team2Win = Query.EQ("Winner", MatchupWinner.Team2);
			var tie = Query.EQ("Winner", MatchupWinner.Tie);

			var onTeam1 = Query.EQ("Team1UserIds", playerId);
			var onTeam2 = Query.EQ("Team2UserIds", playerId);


			var team1Wins = Query.And(new[] { team1Win, onTeam1 });
			var team1Losses = Query.And(new[] { team2Win, onTeam1 });

			var team2Wins = Query.And(new[] { team2Win, onTeam2 });
			var team2Losses = Query.And(new[] { team1Win, onTeam2 });


			var wins = Query.And(new []{ Query.Or(new[] { team1Wins, team2Wins }), gameMatches});
			var losses = Query.And(new[] { Query.Or(new[] { team1Losses, team2Losses }), gameMatches });
			var ties = Query.And(new[] { tie, Query.Or(new[] { onTeam1, onTeam2 }), gameMatches });

			var winCount = database.GetCollection<MatchupResult>(Collections.MatchupResults).Find(wins).Count();
			var lossCount = database.GetCollection<MatchupResult>(Collections.MatchupResults).Find(losses).Count();
			var tieCount = database.GetCollection<MatchupResult>(Collections.MatchupResults).Find(ties).Count();

			return new WinLossRecord
			       {
					   Wins = (int)winCount,
					   Losses = (int)lossCount,
					   Ties = (int)tieCount,
			       };
		}


		public MatchupResult[] GetMatchupResultsByGame( Guid gameId )
		{
			return database
				.GetCollection<MatchupResult>( Collections.MatchupResults )
				.AsQueryable<MatchupResult>()
				.Where( result => result.GameId == gameId )
				.ToArray();
		}


		public void AddPlayerRating( PlayerRating playerRating )
		{
			database
				.GetCollection<PlayerRating>( Collections.PlayerRatings )
				.Insert( playerRating );
		}


		public PlayerRating[] GetPlayerRatings( Guid gameId, Guid playerId )
		{
			return database
				.GetCollection<PlayerRating>( Collections.PlayerRatings )
				.AsQueryable<PlayerRating>()
				.Where( rating => rating.GameId == gameId && rating.PlayerId == playerId )
				.ToArray();
		}


		public PlayerRating[] GetMostRecentPlayerRatings(Guid gameId, Guid playerId)
		{
			return database
				.GetCollection<PlayerRating>(Collections.PlayerRatings)
				.AsQueryable<PlayerRating>()
				.Where(rating => rating.GameId == gameId)
				.Where(rating => rating.PlayerId == playerId)
				.OrderByDescending(rating => rating.Timestamp)
				.ToArray()
				.Distinct(voterComparer)
				.ToArray();
		}


		public PlayerRating GetMostRecentPlayerRating( Guid gameId, Guid playerId, Guid ratedByPlayerId )
		{
			return database
				.GetCollection<PlayerRating>( Collections.PlayerRatings )
				.AsQueryable<PlayerRating>()
				.Where( rating => rating.GameId == gameId )
				.Where( rating => rating.PlayerId == playerId )
				.Where( rating => rating.RatedByPlayerId == ratedByPlayerId )
				.OrderByDescending( rating => rating.Timestamp )
				.FirstOrDefault();
		}


		public void SetActivePlayersForGame( Guid gameId, Guid[] userIds )
		{
			var collection = database.GetCollection<GamePlayer>( Collections.GamePlayers );
			
			var query = Query<GamePlayer>.EQ( player => player.GameId, gameId );

			collection.Remove( query );

			if ( null == userIds || userIds.Length == 0 )
			{
				return;
			}

			foreach ( var userId in userIds )
			{
				collection.Insert( new GamePlayer() {GameId = gameId, UserId = userId} );
			}
		}

		public Map[] GetMaps(Guid gameId)
		{
			var mapIds = GetGameById(gameId).MapIds;
			if(mapIds == null)
				return new Map[0];
			return (from mapId in mapIds select GetMapById(mapId)).ToArray();
		}

		public Map GetMapById(Guid mapId)
		{
			var allMaps = database.GetCollection<Map>(Collections.Maps).FindAll().ToArray();
			return database
				.GetCollection<Map>(Collections.Maps)
				.FindOneById(mapId);
		}

		public Map GetDefaultMap(Guid gameId)
		{
			var game = GetGameById(gameId);
			return GetMapById(game.DefaultMapId);
		}

		public MatchupResult[] GetMatchupResultsByMap(Guid gameId, Guid mapId)
		{
			return database
				.GetCollection<MatchupResult>(Collections.MatchupResults)
				.AsQueryable<MatchupResult>()
				.Where(result => result.GameId == gameId)
				.ToArray();
		}

		public void SaveMatchupResults(MatchupResult[] results)
		{
			foreach(var result in results)
			{
				database.GetCollection<MatchupResult>(Collections.MatchupResults).Save(result);
			}
			
		}
	}

}
