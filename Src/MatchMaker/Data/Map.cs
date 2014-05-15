using System;

namespace MatchMaker.Data
{
	public class Map
	{
		public string Name { get; set; }
		public string ImagePath { get; set; }
		public Guid Id
		{
			get;
			set;
		}
		public Guid GameId { get; set; }

		public Map()
		{
			Id = Guid.NewGuid();
		}


	}
}
