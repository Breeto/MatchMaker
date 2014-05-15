using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.Json;
using Nancy.ViewEngines.Razor;


namespace MatchMaker.Web.Common
{
	public static class ExtensionMethods
	{

		public static string ToJson( this object o )
		{
			var serializer = new JavaScriptSerializer();
			return serializer.Serialize( o );
		}


		public static T FromJson<T>( this string json )
		{
			var serializer = new JavaScriptSerializer();
			return serializer.Deserialize<T>( json );
		}


		public static string ToShortString( this TimeSpan timeSpan )
		{
			if ( timeSpan < TimeSpan.FromMinutes(1) )
			{
				return FormatShortTimeSpan( timeSpan.TotalSeconds, "second" );
			}

			if ( timeSpan < TimeSpan.FromMinutes(60) )
			{
				return FormatShortTimeSpan( timeSpan.TotalMinutes, "minute" );
			}

			if ( timeSpan < TimeSpan.FromHours(24) )
			{
				return FormatShortTimeSpan( timeSpan.TotalHours, "hour" );
			}

			if ( timeSpan < TimeSpan.FromDays(30) )
			{
				return FormatShortTimeSpan( timeSpan.TotalDays, "day" );
			}

			if ( timeSpan < TimeSpan.FromDays(70) )
			{
				return FormatShortTimeSpan( timeSpan.TotalDays / 7, "week" );
			}

			if ( timeSpan < TimeSpan.FromDays(365) )
			{
				return FormatShortTimeSpan( timeSpan.TotalDays / ( 365 / 12 ), "month" );
			}

			return FormatShortTimeSpan( timeSpan.TotalDays / 365, "year" );
		}


		private static string FormatShortTimeSpan( double duration, string durationUnits )
		{
			var roundedValue = (int)Math.Round( duration );
			return string.Format( "{0} {1}{2}", 
				roundedValue, 
				durationUnits,
				roundedValue == 1 ? "" : "s");
		}


		public static void AddUserMessage(this NancyModule module, string message)
		{
			UserMessages.Add( module.Context, message );
		}


		public static string[] PopUserMessages<T>( this NancyRazorViewBase<T> razor )
		{
			var context = razor.Html.RenderContext.Context;

			return UserMessages.Pop( context );
		}


		public static IEnumerable<T> Tail<T>( this IEnumerable<T> enumerable, int count )
		{
			var array = enumerable as T[] ?? enumerable.ToArray();

			if ( array.Length > count )
			{
				return array.Skip( array.Length - count ).ToArray();
			}

			return array;
		}

	}

}
