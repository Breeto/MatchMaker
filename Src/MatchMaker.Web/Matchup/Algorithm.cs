using MatchMaker.MatchMaking;


namespace MatchMaker.Web.Matchup
{

	public class Algorithm
	{

		public Algorithm(IMatchupProposer proposer, bool selected = false ) : this(proposer.GetType().FullName, proposer.GetType().Name, selected)
		{
		}

		public Algorithm( string typeName, string name, bool selected = false )
		{
			TypeName = typeName;
			Name = name;
			IsSelected = selected;
		}

		public string TypeName
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public bool IsSelected
		{
			get;
			set;
		}

	}

}

