using MatchMaker.Data;


namespace MatchMaker.Web.Matchup
{

	public class FindTeamsModel
	{

		public User[] AvailablePlayers
		{
			get;
			set;
		}

		public Game Game
		{
			get;
			set;
		}

		public string AlgorithmName
		{
			get;
			set;
		}

	}

}
