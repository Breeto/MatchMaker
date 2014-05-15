using System;
using System.Collections.Generic;
using MatchMaker.Common.ExtensionMethods;
using MatchMaker.Data;


namespace MatchMaker.MatchMaking
{

	public class ProposedMatchup
	{

		// To support serialization
		private ProposedMatchup()
		{
		}


		public ProposedMatchup( Game game, IEnumerable<User> team1Players, IEnumerable<User> team2Players, double team1WinRatio ) 
			: this( game, new Team(team1Players), new Team(team2Players), team1WinRatio )
		{
		}


		public ProposedMatchup( Game game, Team team1, Team team2, double team1WinRatio )
		{
			if ( team1WinRatio < 0.0 || team1WinRatio > 1.0 )
			{
				throw new ArgumentException( "Win ratio must be between 0.0 and 1.0" );
			}

			Team1PredictedWinRatio = team1WinRatio;
			Team2PredictedWinRatio = 1.0 - team1WinRatio;

			Game = game;
			Team1 = team1;
			Team2 = team2;
		}


		public double Team1PredictedWinRatio
		{
			get;
			private set;
		}


		public double Team1PredictedWinPercentage
		{
			get
			{
				return Math.Round( Team1PredictedWinRatio * 100, 2 );
			}
		}


		public double Team2PredictedWinRatio
		{
			get;
			private set;
		}


		public double Team2PredictedWinPercentage
		{
			get
			{
				return 100.0 - Team1PredictedWinPercentage;
			}
		}

		/// <summary>
		/// The difference between the two teams' win ratios.
		/// 0.0 indicates perfectly balanced.
		/// Higher numbers indicate greater imbalance.
		/// </summary>
		public double Imbalance
		{
			get
			{
				return Math.Abs( Team1PredictedWinRatio - Team2PredictedWinRatio );
			}
		}


		public Game	Game
		{
			get;
			set;
		}


		public Team Team1
		{
			get;
			set;
		}


		public Team Team2
		{
			get;
			set;
		}


		public string ClipboardString
		{
			get
			{
				return string.Format( "[{0}] vs [{1}]", Team1.Members.Join( ", " ), Team2.Members.Join( ", " ) );
			}
		}


		public void SwapTeams()
		{
			var oldTeam1WinRatio = Team1PredictedWinRatio;
			var oldTeam1 = Team1;

			Team1 = Team2;
			Team2 = oldTeam1;

			Team1PredictedWinRatio = Team2PredictedWinRatio;
			Team2PredictedWinRatio = oldTeam1WinRatio;
		}

	}

}
