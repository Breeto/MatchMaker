using System;


namespace MatchMaker.Data
{

	/// <summary>
	/// An association between a user and a game he/she actively plays.
	/// </summary>
	public class GamePlayer
	{

		public GamePlayer()
		{
			Id = Guid.NewGuid();
		}


		// Required for MongoDB
		public Guid Id
		{
			get;
			set;
		}

		public Guid UserId
		{
			get;
			set;
		}

		public Guid GameId
		{
			get;
			set;
		}

	}

}

