using System;


namespace MatchMaker.Data
{

	public class PlayerRating
	{

		public PlayerRating()
		{
			Id = Guid.NewGuid();
			Timestamp = DateTime.Now;
		}

		// Required for MongoDB
		public Guid Id
		{
			get;
			set;
		}

		public DateTime Timestamp
		{
			get;
			set;
		}

		public Guid GameId
		{
			get;
			set;
		}

		public Guid PlayerId
		{
			get;
			set;
		}

		public Guid RatedByPlayerId
		{
			get;
			set;
		}

		public int? Rating
		{
			get;
			set;
		}
	 
	}

}


