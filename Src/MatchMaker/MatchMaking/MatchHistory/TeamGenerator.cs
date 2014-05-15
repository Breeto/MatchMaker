using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchMaker.Data;


namespace MatchMaker.MatchMaking.MatchHistory
{
    public static class TeamGenerator
    {
        public static List<Tuple<Team, Team>> GenerateTeams(User[] Players)
        {
            var result = new ConcurrentQueue<Tuple<Team,Team>>();
            var powerOfTwo = 1 << (Players.Count() - 1);
            var numberOfPlayers = Players.Count();

            Parallel.For(0, (int)powerOfTwo, counter =>
            {
                var ones = GetOnes(counter, numberOfPlayers, 0);
                if (ones == numberOfPlayers / 2 || (numberOfPlayers % 2 == 1 && ones == (numberOfPlayers / 2) + 1))
                {
                    result.Enqueue(CreateTeamFromInt(counter, Players));
                }
            });

            return new List<Tuple<Team,Team>>(result);
        }


        private static int GetOnes(int number, int places, int offset)
        {
            var smallest = places == 1;
            var mask = 1;
            for (int counter = 0; counter < places - 1; counter++)
            {
                mask = mask << 1;
                mask = mask | 1;
            }
            mask = mask << offset;
            if (smallest)
            {
                if ((number & mask) != 0)
                    return 1;
                return 0;
            }


            if (places % 2 == 1)
            {
                return GetOnes(number, places / 2, offset) + GetOnes(number, places / 2, places / 2 + offset) + GetOnes(number, 1, places - 1 + offset);
            }
            return GetOnes(number, places / 2, offset) + GetOnes(number, places / 2, places / 2 + offset);
        }

        private static Tuple<Team,Team> CreateTeamFromInt(int TeamNumber, User[] Players)
        {
            var currentPlayerMask = 1;
            var team1 = new Team();
            var team2 = new Team();
            for (int currentPlayer = 0; currentPlayer < Players.Count(); currentPlayer++)
            {
                if ((currentPlayerMask & TeamNumber) != 0)
                {
                    team1.Members.Add(Players[currentPlayer]);
                }
                else
                {
                    team2.Members.Add(Players[currentPlayer]);
                }
                currentPlayerMask = currentPlayerMask << 1;
            }

            return  new Tuple<Team, Team>(team1, team2);
        }

    }
}
