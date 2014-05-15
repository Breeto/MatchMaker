using System;
using System.Collections.Generic;
using MatchMaker.MatchMaking;


namespace MatchMaker.Data
{

	public interface IDatabase
	{

		UserCredentials GetUserCredentialsByUserId( Guid userId );

		UserCredentials GetUserCredentialsByAuthId( Guid authId );

		UserCredentials GetUserCredentialsByEmailAddress( string emailAddress );

		User GetUserById( Guid userId );

		User GetUserByName( string userName );

		void AddUser( User user, UserCredentials credentials );

		void UpdateUserCredentials( UserCredentials userCredentials );

		User[] GetUsers();

		void AddGame( Game game );

		Game[] GetGames();

		Game GetGameById( Guid gameId );

		Game GetGameByKey( string gameKey );

		Game[] GetGamesPlayedByUser( Guid userId );

		User[] GetPlayersForGame( Guid gameId );

		void SetGamesPlayedByUser( Guid userId, Guid[] gameIds );

		void SaveMatchupResult( MatchupResult result );

		WinLossRecord GetWinLossRecord(Guid gameId, Guid playerId);

		MatchupResult[] GetMatchupResultsByGame( Guid gameId );

		void AddPlayerRating( PlayerRating playerRating );

		PlayerRating[] GetPlayerRatings( Guid gameId, Guid playerId );

		PlayerRating[] GetMostRecentPlayerRatings(Guid gameId, Guid playerId);

		PlayerRating GetMostRecentPlayerRating(Guid gameId, Guid playerId, Guid ratedByPlayerId);

		void SetActivePlayersForGame( Guid gameId, Guid[] userIds );

		Map[] GetMaps(Guid gameId);

		Map GetMapById(Guid mapId);

		Map GetDefaultMap(Guid gameId);

		MatchupResult[] GetMatchupResultsByMap(Guid gameId, Guid mapId);

	}

}



