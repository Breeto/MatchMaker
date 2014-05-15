using System.Collections.Generic;
using System.Linq;
using MatchMaker.Data;


namespace MatchMaker.MatchMaking.MatchHistory
{
    public class Link
    {
        public User OtherPlayer { get; set; }

        private List<MatchupResult> Games;

	    private double paddedGames;

        public Link(User otherPlayer)
        {
            OtherPlayer = otherPlayer;
            Games = new List<MatchupResult>();
	        paddedGames = 0;
        }

        public void AddGame(MatchupResult game)
        {
            if (game.Winner == MatchupWinner.Tie ||(!game.Team1UserIds.Contains(OtherPlayer.Id) && !game.Team2UserIds.Contains(OtherPlayer.Id)))
                return;
            Games.Add(game);
        }

        public int GetGameCount()
        {
            return Games.Count;
        }

        private double GetWins()
        {
            var winCount = 0.0;
            foreach (var game in Games)
            {
                var team1 = game.Team1UserIds.Contains(OtherPlayer.Id);
                if ((team1 && game.Winner == MatchupWinner.Team1) || (!team1 && game.Winner == MatchupWinner.Team2))
                    winCount++;
            }
            return winCount;
        }

        public double GetWinLoss()
        {
            if (!Games.Any())
            {
                return .5;
            }
	        var wins = GetWins();
	        var totalGames = Games.Count + paddedGames;
            return paddedGames == 0 ? wins/totalGames : (wins + (paddedGames*.5))/totalGames;
        }

		public void RemoveOldGames(int totalLinks)
		{
			if (Games.Count <= totalLinks)
				return;
			var numberToRemove = Games.Count - totalLinks;
			var orderedGames = new List<MatchupResult>(Games.OrderBy((g1) => g1.Timestamp));
			for(var x = 0; x<numberToRemove; x++)
			{
				Games.Remove(orderedGames[x]);
			}

		}

		public void BackfillOldGames(int totalLinks)
		{
			if (Games.Count >= totalLinks)
				return;
			paddedGames = totalLinks - Games.Count;
		}


    }
}
