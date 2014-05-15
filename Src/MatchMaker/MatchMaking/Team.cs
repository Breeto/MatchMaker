using System.Collections.Generic;
using System.Linq;
using MatchMaker.Data;


namespace MatchMaker.MatchMaking
{

	public class Team
	{

		public Team()
		{
			Members = new List<User>();
		}


		public Team( IEnumerable<User> members )
		{
			Members = members.ToList();
		}


		public List<User> Members
		{
			get;
			set;
		}
	}
}