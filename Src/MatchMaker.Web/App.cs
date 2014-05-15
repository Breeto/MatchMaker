using MatchMaker.MatchMaking;


namespace MatchMaker.Web
{
	public class App
	{

		static App()
		{
			InitSettings();
		}


		private static void InitSettings()
		{
			// TODO: Load from disk
			Settings = new Settings();
		}


		public static Settings Settings
		{
			private set;
			get;
		}


		public static IMatchupProposer MatchupProposer
		{
			get;
			set;
		}

	}

}
