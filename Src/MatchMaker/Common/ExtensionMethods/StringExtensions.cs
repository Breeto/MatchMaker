using System;
using System.Linq;


namespace MatchMaker.Common.ExtensionMethods
{

	public static class StringExtensions
	{


		public static bool IsAlphanumeric( this string s )
		{
			return s.ToCharArray().All( Char.IsLetterOrDigit );
		}

	}

}

