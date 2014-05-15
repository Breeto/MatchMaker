using System;
using System.Collections.Generic;


namespace MatchMaker.Common.ExtensionMethods
{

	public static class IEnumerableExtensions
	{

		public static string Join<T>( this IEnumerable<T> enumerable, string separator )
		{
			return String.Join( separator, enumerable );
		}

	}

}
