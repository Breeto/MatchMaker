using System;


namespace MatchMaker.Data
{

	/// <summary>
	/// Details about the result of a matchup.
	/// </summary>
	public class MatchupResult
	{

		public MatchupResult()
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

		public Guid GameId
		{
			get;
			set;
		}

		public DateTime Timestamp
		{
			get;
			set;
		}

		public MatchupWinner Winner
		{
			get;
			set;
		}

		public Guid[] Team1UserIds
		{
			get;
			set;
		}

		public Guid[] Team2UserIds
		{
			get;
			set;
		}

		public int? Team1Score
		{
			get;
			set;
		}

		public int? Team2Score
		{
			get;
			set;
		}

		public string Comment
		{
			get;
			set;
		}

		public Guid MapId { get; set; }

	}

}
