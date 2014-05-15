using System;
using System.Collections.Generic;


namespace MatchMaker.Data
{
	
	public class Game
	{

		// To support serialization
		public Game()
		{
			Id = Guid.NewGuid();
		}


		public string Key
		{
			get;
			set;
		}

		public Guid Id	
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public List<Guid> MapIds { get; set; }

		public Guid DefaultMapId { get; set; }

	}

}

