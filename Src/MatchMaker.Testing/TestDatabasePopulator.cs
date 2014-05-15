using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatchMaker.Common;
using MatchMaker.Common.ExtensionMethods;
using MatchMaker.Data;
using MatchMaker.MatchMaking;


namespace MatchMaker.Testing
{

	public class TestDatabasePopulator
	{

		public static string[] UserNames =
			{
				"Peter",
				"Lois",
				"Chris",
				"Meg",
				"Brian",
				"Stewie",
				"Quagmire",
				"Joe",
				"Cleveland",
				"Susie",
				"Mort",
				"Carter",
				"Barbara",
				"Angela",
				"Opie",
				"Ollie",
				"Tom Tucker",
				"Mayor West",
			};

		public static string[] GameNames =
			{
				"Modern Warfare 2",
				"Left 4 Dead 2",
				"Counter-Strike",
				"Call of Duty: Black Ops",
			};

		public static Map[] Maps = 
		{
			new Map(){Name = "Blood Harvest", ImagePath = "left4dead2/blood harvest.jpg"}, 
			new Map(){Name = "No Mercy", ImagePath = "left4dead2/no mercy.jpg"}
		};

		public const int MatchesPerGame = 50;
		public static readonly TimeSpan MatchHistoryLength = TimeSpan.FromDays( 100 );
		public static readonly Range<int> ScoreRange = new Range<int>( 0, 3000 );
		public static readonly TimeSpan PlayerRatingHistoryLength = TimeSpan.FromDays( 90 );
		public static readonly Range<int> RatingsPerPlayer = new Range<int>( 0, 30 );
		public static readonly Range<int> RatingRange = new Range<int>( 1, 10 );


		private readonly TestDatabase database;
		private readonly IPasswordEncryptor passwordEncryptor;


		public TestDatabasePopulator( TestDatabase database, IPasswordEncryptor passwordEncryptor )
		{
			this.database = database;
			this.passwordEncryptor = passwordEncryptor;
		}


		public void Populate()
		{
			CreateUsers();
			CreateGames();
			SelectGamesForUsers();
			CreateMatchHistory();
			CreatePlayerRatings();

			var zero = new User() {Id = Guid.NewGuid(), Name = "Zero"};
			database.AddUser(zero, new UserCredentials(zero));
			database.SetGamesPlayedByUser(zero.Id, database.GetGames().Select(g => g.Id).ToArray());
		}


		private void CreatePlayerRatings()
		{
			var games = database.GetGames();

			foreach ( var game in games )
			{
				CreateRatingsFor( game );
			}
		}


		private void CreateRatingsFor( Game game )
		{
			var players = database.GetPlayersForGame( game.Id );

			foreach ( var ratingPlayer in players )
			{
				foreach ( var ratedPlayer in players )
				{
					var numberOfRatings = RatingsPerPlayer.Next();

					for ( int i = 0; i < numberOfRatings; i++ )
					{
						var rating = new PlayerRating();
						rating.GameId = game.Id;
						rating.PlayerId = ratedPlayer.Id;
						rating.RatedByPlayerId = ratingPlayer.Id;
						rating.Timestamp = DateTime.Now - PlayerRatingHistoryLength.Next();

						if ( .8.NextBool() )
						{
							rating.Rating = RatingRange.Next();
						}

						database.AddPlayerRating( rating );
					}
				}
			}
		}


		private void CreateMatchHistory()
		{
			var games = database.GetGames();

			foreach ( var game in games )
			{
				CreateMatchHistoryFor( game );
			}
		}

		private static Random random = new Random();

		private Map GetRandomMap(Game game)
		{
			return Maps[random.Next(0, Maps.Length)];
		}


		private void CreateMatchHistoryFor( Game game )
		{
			var players = database.GetPlayersForGame( game.Id );

			if ( players.Length < 2 )
			{
				return;
			}

			var selectionPoolSizeRange = new Range<int>( 2, players.Length );

			for ( int i = 0; i < MatchesPerGame; i++ )
			{
				var selectionPoolSize = selectionPoolSizeRange.Next();
				var team1 = new Team();
				var team2 = new Team();

				for ( int j = 0; j < selectionPoolSize; j++ )
				{
					if ( j.IsEven() )
					{
						team1.Members.Add( players[j] );
					}
					else
					{
						team2.Members.Add( players[j] );
					}
				}

				var team1Score = ScoreRange.Next();
				var team2Score = ScoreRange.Next();

				var result = new MatchupResult
				{
					GameId = game.Id,
					Team1UserIds = team1.Members.Select( user => user.Id ).ToArray(),
					Team2UserIds = team2.Members.Select( user => user.Id ).ToArray(),
					Timestamp = DateTime.Now - MatchHistoryLength.Next(),
					Winner = team1Score.GetWinner(team2Score)
				};

				if ( 0.5.NextBool() )
				{
					result.Team1Score = team1Score;
					result.Team2Score = team2Score;
				}

				if ( 0.2.NextBool() )
				{
					result.Comment = "This is a sample comment";
				}

				result.MapId = GetRandomMap(game).Id;

				database.SaveMatchupResult(result);
			}
		}


		private void SelectGamesForUsers()
		{
			var games = database.GetGames();
			var users = database.GetUsers();
			foreach ( var user in users )
			{
				var gameIds = games
					.Where( game => 0.5.NextBool() )
					.Select( game => game.Id )
					.ToArray();

				database.SetGamesPlayedByUser( user.Id, gameIds );
			}
		}


		private void CreateUsers()
		{
			foreach ( var username in UserNames )
			{
				var user = CreateUser( username );
				var credentials = CreateCredentialsFor( user );

				database.AddUser( user, credentials );
			}
		}


		private void CreateGames()
		{
			foreach ( var gameName in GameNames )
			{
				var game = new Game() {Name = gameName, Key = CreateGameKeyFromName(gameName), MapIds = new List<Guid>(from m in Maps select m.Id)};

				database.AddGame(game);
			}
		}


		private string CreateGameKeyFromName( string gameName )
		{
			var stringBuilder = new StringBuilder();

			foreach ( var c in gameName.ToLower().ToCharArray() )
			{
				if ( Char.IsLetterOrDigit( c ) )
				{
					stringBuilder.Append( c );
				}
			}

			return stringBuilder.ToString();
		}


		private User CreateUser( string username )
		{
			return new User()
			{
				Name = username,
			};
		}


		private UserCredentials CreateCredentialsFor( User user )
		{
			return new UserCredentials()
			{
				AuthId = Guid.NewGuid(),
				EmailAddress = string.Format( "{0}@example.com", user.Name ),
				EncryptedPassword = passwordEncryptor.Encrypt( user.Name.ToLower() ),
			};
		}
	
	}

}

