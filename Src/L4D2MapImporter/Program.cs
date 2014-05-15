using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchMaker.MongoDB;

namespace L4D2MapImporter
{
	class Program
	{

		static void Main(string[] args)
		{
			var db = new Database();
			var l4d2 = db.GetGameByKey("l4d2");
			var l4d2Maps = db.GetMaps(l4d2.Id);
			var results = db.GetMatchupResultsByGame(l4d2.Id);
			var nameDictionary = l4d2Maps.ToDictionary(map => map.Name.ToUpper(), map => map);
			foreach(var result in results)
			{
				bool matched = false;
				foreach(var key in nameDictionary.Keys)
				{
					if (result.Comment == null || !result.Comment.ToUpper().Contains(key))
						continue;
					result.MapId = nameDictionary[key].Id;
					matched = true;
				}
				if(!matched)
				{
					result.MapId = nameDictionary["BLANK MAP"].Id;
				}
			}
			db.SaveMatchupResults(results);
		}
	}
}
