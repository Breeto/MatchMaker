using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchMaker.Data;


namespace MatchMaker.MatchMaking
{
	public class ComboMatchupProposer : IMatchupProposer
	{
		private readonly IMatchupProposer[] proposers;
		private readonly double[] weights;


		public ComboMatchupProposer(IEnumerable<IMatchupProposer> proposers, IEnumerable<double> weights = null)
		{
			this.proposers = proposers.ToArray();
			this.weights = Enumerable.Repeat(1.0, this.proposers.Length).ToArray();

			if (weights == null) weights = new double[0];
			var weightArray = weights.ToArray();

			for (int i = 0; i < weightArray.Length && i < this.weights.Length; i++)
			{
				this.weights[i] = weightArray[i];
			}
		}


		public ProposedMatchup[] GetMatchups(Game game, User[] players)
		{
			var matchups = new List<ProposedMatchup>();
			var proposersMatchups = new ConcurrentQueue<ProposedMatchup[]>();
			Parallel.ForEach(proposers, proposer => proposersMatchups.Enqueue(proposer.GetMatchups(game, players).OrderBy(GetMatchupIdentifier).ToArray()));

			var proposersMatchupsArray = proposersMatchups.ToArray();
			int possibleMatchups =  0;
			for (int i = 0; i < proposersMatchupsArray.Length - 1; i++)
			{
				possibleMatchups = proposersMatchupsArray[i].Length;
				if(proposersMatchupsArray[i].Length != proposersMatchupsArray[i+1].Length)
				{
					throw new Exception("Multiple Proposer Matchups do not Match");
				}
			}

			for (int i = 0; i < possibleMatchups; i++)
			{
				double team1Value = 0d;
				double team2Value = 0d;
				for (int j = 0; j < proposersMatchupsArray.Length; j++)
				{
					team1Value += weights[j] * proposersMatchupsArray[j][i].Team1PredictedWinRatio;
					team2Value += weights[j] * proposersMatchupsArray[j][i].Team2PredictedWinRatio;
				}

				var team1 = proposersMatchupsArray[0][i].Team1;
				var team2 = proposersMatchupsArray[0][i].Team2;
				matchups.Add(new ProposedMatchup(game, team1, team2, team1Value / (team1Value + team2Value)));

			}

			return matchups.ToArray();
		}


		private object GetMatchupIdentifier(ProposedMatchup matchup)
		{
			return string.Concat(matchup.Team1.Members.Concat(matchup.Team2.Members).Select(m => m.Id.ToString()));
		}
	}
}
